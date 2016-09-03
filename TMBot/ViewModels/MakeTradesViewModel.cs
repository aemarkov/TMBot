using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TMBot.API.Exceptions;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Utilities;
using TMBot.Utilities.CallWaiter;
using TMBot.Utilities.MVVM.AsyncCommand;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
    /// <summary>
    /// Модель вида для страницы выставления ордеров
    /// </summary>
    public class MakeTradesViewModel
    {
        //Список предметов в инвентаре стим
        public ObservableCollection<InventoryItemViewModel> InventoryItems { get; private set; }

        public IAsyncCommand UpdateInventoryCommand { get; private set; }
        public IAsyncCommand BeginCommand { get; private set; }


        // За какой процент цены выставлять
        public float PricePercentage { get; set; } = 1;

        public MakeTradesViewModel()
        {
            InventoryItems = new ObservableCollection<InventoryItemViewModel>();

            UpdateInventoryCommand = AsyncCommand.Create(update_inventory);
            BeginCommand = AsyncCommand.Create(begin);

            //load_inventory_items();
        }


        #region INVENTORY

        //Обновляет инвентарь
        private async Task update_inventory(object param)
        {
            await load_inventory_items();
        }


        //Загружает инвентарь стима
        private async Task load_inventory_items()
        {
            await load_inventory_game<CSSteamAPI, CSTMAPI>();
        }

        //Загружает инвентарь конкретной игры
        //Да, я понимаю, что надо было завернуть эту асинхронную операцию в 
        //INotifyPropertyChanged и биндится прямо к ней, а не
        //менять свойство из нее
        private async Task load_inventory_game<TSteamAPI, TTMAPI>() where TSteamAPI : ISteamAPI
                                                                     where TTMAPI : ITMAPI
        {
            try
            {

                Log.d("Получение инвентаря Steam...");

                ISteamAPI steamApi = SteamFactory.GetInstance<SteamFactory>().GetAPI<TSteamAPI>();
                ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

                //Получаем инвентарь стим
                var inventory = await steamApi.GetSteamInventoryAsync();

                //Получаем трейды
                IList<Trade> trades = tmApi.GetTrades();
                if (trades == null)
                    throw new Exception();

                InventoryItems.Clear();

                //Составляем список инвентаря
                foreach (var item in inventory.rgInventory)
                {
                    var rgItem = item.Value;
                    var description = inventory.rgDescriptions[rgItem.classid + "_" + rgItem.instanceid];

                    string imageUrl = "http://cdn.steamcommunity.com/economy/image/" + description.icon_url;

                    
                    //Определяем статус предмета
                    var trade_item = trades.FirstOrDefault(x=>x.i_classid == rgItem.classid && x.ui_real_instance == rgItem.instanceid);
                    
                    InventoryItemViewModel inventoryItemViewModel = new InventoryItemViewModel()
                    {
                        Name = description.market_name,
                        ImageUrl = imageUrl,
                        Status = trade_item == null ? ItemStatus.NOT_TRADING : UiStatusToStatusConverter.Convert(trade_item.ui_status),
                        ClassId = rgItem.classid,
                        IntanceId = rgItem.instanceid
                    };
                    InventoryItems.Add(inventoryItemViewModel);
                }

                Log.d($"Загружено {InventoryItems.Count} предметов");
            }
            catch (BadKeyException)
            {

                MessageBox.Show("Не удалось загрузить инвентарь: неверный API-key", "Не удалось загрузить инвентарь",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить инвентарь: неизвестная ошибка", "Не удалось загрузить инвентарь",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region SELLING

        //Начинает выставлять
        private async Task begin(object param)
        {
            await begin_sell_game<CSTMAPI>(InventoryItems);
        }

        //Начинает выставлять предметы определенной площадки
        private async Task begin_sell_game<TTMAPI>(ICollection<InventoryItemViewModel> items) where TTMAPI : ITMAPI
        {
            if (items == null)
            {
                Log.w("Предметы в инвентаре не загружены");
                return;
            }

            Log.d("Выставляются предметы...");

            if (items.Count == 0)
            {
                Log.w("Нет предметов для выставления: в инвентаре нет предметов");
                return;
            }


            var tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
            int count = 0;

            //Определяем цену предметов и выставляем
            foreach (var item in InventoryItems.Where(item => item.Status==ItemStatus.NOT_TRADING))
            {

                //Получаем цену предмета на ТМ
                int price;
                int? _price = PriceCounter.GetMinSellPrice<TTMAPI>(item.ClassId, item.IntanceId);

                
                if (_price == null)
                {
                    //Товар не найден, поиск на площадке стима
                    _price = PriceCounter.GetSteamMinSellPrice(item.ClassId, item.IntanceId);
                    if (_price != null)
                    {
                        //Товар найден на площадке стима
                        //Выставляем за 100% от цены стим
                        price = _price.Value;
                    }
                    else
                    {
                        //Товар не найден даже в стиме
                        Log.w($"Товар {item.ClassId}_{item.IntanceId} не найден ни на ТМ, ни в Steam. Не выставляется");
                        continue;
                    }
                }
                else
                {
                    //Выставляем за % от минимальной цены
                    price = _price.Value;
                    price = (int) (PricePercentage*price);
                }

                tmApi.SetNewItem(item.ClassId, item.IntanceId, (int)price);
                Log.d("Предмет {0}_{1} выставлен. за цену {2} коп.", item.ClassId, item.IntanceId, price);
                count++;

            }



            if (count == 0)
                Log.w("Нет предметов для выставления: все уже выставляются");
        }

        #endregion
    }
}
