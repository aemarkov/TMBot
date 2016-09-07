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
using TMBot.Utilities.MVVM;
using TMBot.Utilities.MVVM.AsyncCommand;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
    /// <summary>
    /// Модель вида для страницы выставления ордеров
    /// </summary>
    public class MakeTradesViewModel:PropertyChangedBase
    {
        //Список предметов в инвентаре стим
        public ObservableCollection<MakeTradesItemViewModel> InventoryItems { get; private set; }

        public IAsyncCommand UpdateInventoryCommand { get; private set; }
        public IAsyncCommand BeginCommand { get; private set; }

        //Переключение выделения всех элементов
        private bool _selectAll = true;

        public bool SelectAll
        {
            get { return _selectAll;}
            set
            {
                _selectAll = value;

                if(InventoryItems==null) return;
                foreach (var item in InventoryItems)
                {
                    item.ShouldSell = _selectAll;
                }

                NotifyPropertyChanged();
            }
        }

        // За какой процент цены выставлять
        public float PricePercentage { get; set; } = 1;

        public MakeTradesViewModel()
        {
            InventoryItems = new ObservableCollection<MakeTradesItemViewModel>();

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
                IList<Trade> trades = tmApi.GetTrades().Where(x=>x.ui_status==1 || x.ui_status==2).ToList();
                //if (trades.Count==0)
                //    throw new Exception();

                InventoryItems.Clear();

                /* Неообходимо определить, какие предметы выставляются, а какие - нет.
                 * Общего ID между ТМ и стимом нет
                 * Выставление идет по classid_instanceid, т.е. предметы в инвентаре не
                 * различаются. 
                 * 
                 * Поэтому просто посчитаем, сколько предметов с разными
                 * classid_instanceid выставляются, и столько первых предметов
                 * в инвентаре пометим, как выставляемые */

                //Подсчет количества трейдов по разным предметам
                var tradesCount = new Dictionary<string, int>();
                foreach (var trade in trades)
                {
                    string id = $"{trade.i_classid}_{trade.ui_real_instance}";
                    if (tradesCount.ContainsKey(id))
                        tradesCount[id]++;
                    else
                        tradesCount.Add(id, 1);
                }

                //Составляем список инвентаря
                foreach (var item in inventory.rgInventory)
                {
                    var rgItem = item.Value;
                    var description = inventory.rgDescriptions[rgItem.classid + "_" + rgItem.instanceid];

                    string imageUrl = "http://cdn.steamcommunity.com/economy/image/" + description.icon_url;


                    //Определяем статус предмета
                    var trade_item = trades.FirstOrDefault(x=>x.i_classid == rgItem.classid && x.ui_real_instance == rgItem.instanceid);

                    //Определяем, выставляется ли предмет
                    //Просто основываемся на количестве таких же выставляемых предметов
                    bool isSelling = false;
                    string id = $"{rgItem.classid}_{rgItem.instanceid}";
                    if (tradesCount.ContainsKey(id) && tradesCount[id]>0)
                    {
                        isSelling = true;
                        tradesCount[id]--;
                    }

                    var status = (trade_item == null) || !isSelling
                        ? ItemStatus.NOT_TRADING
                        : UiStatusToStatusConverter.Convert(trade_item.ui_status);

                    //Расчитываем цену предмета
                    //Если предмет не выставляется, то определяем его цену
                    //Если цена не найдена, то не выставляем
                    //Если предмет уже выставляетсЯ, то пишем цену, по которой
                    //Он выставляется


                    int price;
                    bool shouldSell;

                    if (status == ItemStatus.NOT_TRADING)
                    {
                        int? _price = await getPrice<TTMAPI>(rgItem.classid, rgItem.instanceid);

                        if (_price != null)
                        {
                            price = _price.Value;
                            shouldSell = SelectAll;
                        }
                        else
                        {
                            price = 0;
                            shouldSell = false;
                        }
                    }
                    else
                    {
                        price = (int)trade_item.ui_price;
                        shouldSell = false;
                    }
                    

                    //Заполняем поля
                    MakeTradesItemViewModel tradeItem = new MakeTradesItemViewModel()
                    {
                        Name = description.market_name,
                        ImageUrl = imageUrl,
                        Status = status,
                        ClassId = rgItem.classid,
                        IntanceId = rgItem.instanceid,
                        ShouldSell = shouldSell,
                        SellPrice = price
                    };
                    InventoryItems.Add(tradeItem);
                }

                Log.d($"Загружено {InventoryItems.Count} предметов");
            }
            catch (BadKeyException)
            {

                MessageBox.Show("Не удалось загрузить инвентарь: неверный API-key", "Не удалось загрузить инвентарь",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception exp)
            {
                MessageBox.Show($"Не удалось загрузить инвентарь: {exp.Message}", "Не удалось загрузить инвентарь",
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
        private async Task begin_sell_game<TTMAPI>(ICollection<MakeTradesItemViewModel> items) where TTMAPI : ITMAPI
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
            foreach (var item in InventoryItems.Where(item => item.Status==ItemStatus.NOT_TRADING && item.ShouldSell))
            {
                try
                {                    
                    tmApi.SetNewItem(item.ClassId, item.IntanceId, (int) item.SellPrice);
                    Log.d("Предмет {0}_{1} выставлен. за цену {2} коп.", item.ClassId, item.IntanceId, item.SellPrice);
                    count++;

                }
                catch (APIException exp)
                {
                }
                catch (Exception exp)
                {
                    
                }
            }



            if (count == 0)
                Log.w("Нет предметов для выставления: все уже выставляются");
        }

        #endregion

        //Расчитывает цену предмета
        private async Task<int?> getPrice<TTMAPI>(string classid, string instanceid) where TTMAPI : ITMAPI
        {
            //Получаем цену предмета на ТМ
            int price;
            int? _price = await Task.Run(()=> PriceCounter.GetMinSellPrice<TTMAPI>(classid, instanceid));


            if (_price == null)
            {
                //Товар не найден, поиск на площадке стима
                _price = await Task.Run(()=>PriceCounter.GetSteamMinSellPrice(classid, instanceid));
                if (_price != null)
                {
                    //Товар найден на площадке стима
                    //Выставляем за 100% от цены стим
                    return _price.Value;
                }
                else
                {
                    //Товар не найден даже в стиме
                    Log.w($"Товар {classid}_{instanceid} не найден ни на ТМ, ни в Steam");
                    return null;
                }
            }
            else
            {
                //Выставляем за % от минимальной цены
                price = _price.Value;
                return (int)(PricePercentage * price);
            }

        }
    }
}
