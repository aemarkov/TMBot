using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AutoMapper;
using TMBot.API.Exceptions;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Data;
using TMBot.Database;
using TMBot.Models;
using TMBot.Settings;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Workers
{
	/// <summary>
	/// Выполняет мониторинг и изменнеие цены продажи
	/// предметов в фоне
	/// </summary>
	/// <typeparam name="TTMAPI">Класс АПИ площадки</typeparam>
	public class SellWorker<TTMAPI,TSteamAPI> : BaseItemWorker<TTMAPI, TSteamAPI, Trade, TradeItemViewModel> where TTMAPI : ITMAPI where TSteamAPI : ISteamAPI
    {

	    public SellWorker(SynchronizedObservableCollection<ItemViewModel> items) : base(items)
        {
            //Загрузка порога цены
	        var settings = SettingsManager.LoadSettings();
	        PriceThreshold = settings.TradeMaxThreshold;
	    } 


        //Показывает сообщение об ошибке
	    protected override void ShowErrorMessage(string error_reason)
	    {
	        MessageBox.Show($"Не удалось начать продажу: {error_reason}", "Не удалось начать продажу", MessageBoxButton.OK,
	            MessageBoxImage.Warning);
	    }

        //Получает список трейдов
	    protected override ICollection<Trade> GetTMItems()
	    {
            //В трейдах еще указываются покупаемые предметы с разными статусами, поэтому
            //мы их игнорируем
	        return tmApi.GetTrades().Where(x=>x.ui_status==1 || x.ui_status==2).ToList();
        }

        //Находит в базе предмет, соответствующий трейду
	    protected override Item GetDbItem(ItemsRepository repository, Trade api_item)
	    {
            return repository.GetById(api_item.i_classid, api_item.ui_real_instance);
        }

        //Получает цену предмета
	    protected override int GetItemMyPrice(Trade api_item)
	    {
	        return (int)(api_item.ui_price*100);
	    }

        //Расчитываем новую стоимость
        protected override bool GetItemNewPrice(ItemViewModel item, int tm_price, ref int myNewPrice)
        {
            /* Если минимальная цена меньше текущей - делаем нашу меньше минимальной на 
             * 1 коп.
             * 
             * Если минимальная - наша, то увеличиваем цену (до минимальной - 1коп) только
             * если разница больше заданных %
             */
            if ((item.MyPrice!=tm_price-1) && ((tm_price < item.MyPrice) || (item.MyPrice < item.PriceLimit) || ((tm_price - item.MyPrice) / (float)tm_price > PriceThreshold)))
            {
                myNewPrice = tm_price - 1;
                return true;
            }

            //Цену менять не надо
            myNewPrice = item.MyPrice;
            return false;
        }

        //Расчитывает миимальную цену на TM
        protected override int? GetItemTMPrice(ItemViewModel item)
        {
            return PriceCounter.GetMinSellPrice<TTMAPI>(item.ClassId, item.IntanceId, item.PriceLimit);
        }

        //Обновление списка
        protected override void UpdateItems()
        {
            /* Сделаем запрос трейдов и сравним его с текущим списком
             * Новые предметы добавим
             * Отсутствующие удалим
             * Если статус изменился с TRADING на SOLD - меняем. Остальные
             * изменения статуса игнорируем */

            try
            {

                var repository = new ItemsRepository();

                var trades = GetTMItems();
                foreach (var trade in trades)
                {
                    var listItem = Items.FirstOrDefault(x => x.ItemId == trade.ui_id);
                    if (listItem == null)
                    {
                        //Добавляем 
                        Items.Add(CreateTradeItem(trade, repository));
                    }
                    else
                    {
                        //Проверяем статус
                        var newStatus = UiStatusToStatusConverter.Convert(trade.ui_status);
                        if (listItem.Status == ItemStatus.TRADING && newStatus == ItemStatus.SOLD)
                            listItem.Status = ItemStatus.SOLD;
                    }
                }

                //Удаление старых трейдов
                for (int i = 0; i < Items.Count; i++)
                {
                    var trade = trades.FirstOrDefault(x => x.ui_id == Items[i].ItemId);
                    if (trade == null)
                        Items.RemoveAt(i);
                }
            }
            catch (Exception exp)
            {
                Log.e($"Не удалось обновить список предметов: {exp}");
            }
        }

        //Обработка статуса
        protected override bool CheckStatusAndMakeRequest(ItemViewModel item)
        {
            {
                //Если предмет уже в состоянии продажи - не меняем его цену
                if (item.Status != ItemStatus.TRADING && item.Status!=ItemStatus.SOLD)
                    return false;


                //Если статус предмета - продано, но ItemRequest почему-то еще не сделан, то
                //надо сделать
                //ВНИМАНИЕ: может случится вызов этого дерьма в момент работы обработчика
                //вебсокета (т.е. все ОК, а не сломалось). надо как-то обработать
                if (item.Status == ItemStatus.SOLD)
                {
                    //ItemRequest
                    ItemRequestHelper.MakeSellItemRequest(tmApi, item.ItemId);
                    return false;
                }
                return true;
            }
        }

        

        //Остановка
        /* Немного говнокод. У меня Stop
         * вызывается из ViewModel при уничтожении (закрытии окна)
         * но не факт, что это всегда так может быть.
         * 
         * На самом деле говнокод в том, что настройка находится
         * в воркере и к ней прибинжено окно */
        public override void Stop()
	    {
	        base.Stop();
	        var settings = SettingsManager.LoadSettings();
	        settings.TradeMaxThreshold = PriceThreshold;
            SettingsManager.SaveSettings(settings);
	    }

	    /// <summary>
	    /// Обновляет цену
	    /// </summary>
	    /// <param name="itemid">ID предмета</param>
	    /// <param name="price">новая цена</param>
	    protected override void UpdatePrice(ItemViewModel item, int price)
	    {
            tmApi.SetPrice(item.ItemId, price);
        }
    }
}