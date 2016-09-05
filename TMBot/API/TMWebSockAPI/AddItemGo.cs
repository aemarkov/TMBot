using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Data;
using TMBot.Utilities;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Событие получения данных по каналу additem_go
    /// Возникает когда предмет появляется на странице Sell:
    /// 
    /// - выставление нового предмета
    /// - куплен предмет ордера
    /// 
    /// Поэтому надо обрабатывать статус
    /// (модель такая же, как и trade)
    /// </summary>
    public class AddItemGo<TTMAPI> : BaseEvent<TTMAPI> where TTMAPI : ITMAPI
    {
        private ITMAPI api;

        public AddItemGo()
        {
            api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
        }

        public override async Task HandleEvent(string data)
        {
            try
            {
                Log.d("AddItemGo");
                var additem_go = JsonConvert.DeserializeObject<Trade>(data);

                if (additem_go.ui_status == 3)
                {
                    //Ожидание, когда продавец передаст предмет

                    //Смена статуса предмета
                    //Все зависист от того, находится ли уже предмет в списке трейдов или нет
                    //Скорее всего нет. Если нет - то присвоим полученный ID предмету
                    var item = ItemCollectionsContainer.GetInstance().FindOrderItem(additem_go.ui_id);
                    if (item != null)
                    {
                        item.Status = UiStatusToStatusConverter.Convert(additem_go.ui_status);
                        return;
                    }

                    item = ItemCollectionsContainer.GetInstance().FindOrderItem(additem_go.i_classid, additem_go.ui_real_instance);
                    if (item != null)
                    {
                        item.Status = UiStatusToStatusConverter.Convert(additem_go.ui_status);
                        item.ItemId = additem_go.ui_id;
                        return;
                    }

                    Log.e($"Ордер {additem_go.ui_id} не найден");


                }
                if (additem_go.ui_status == 4)
                {
                    //Можно забрать предмет
                    //Такого В ЭТОМ событии быть не может, но мало ли

                    ItemRequestHelper.MakeBuyItemRequest(api, additem_go.ui_bid, additem_go.ui_id);
                }
            }
            catch (Exception exp)
            {
                Log.e($"Не удалось обработать событие AddItem_Go: {exp.Message}");
            }
        }
    }
}