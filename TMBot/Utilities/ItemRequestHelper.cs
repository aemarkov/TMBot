using System;
using System.Threading.Tasks;
using TMBot.API.TMAPI;
using TMBot.Data;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Utilities
{
    /// <summary>
    /// Выполняет ItemRequest и все сопутствующие дела
    /// </summary>
    public static class ItemRequestHelper
    {
        public static async Task MakeItemRequest(ITMAPI api, string itemId)
        {
            //Меняем статус
            var item = ItemCollectionsContainer.GetInstance().FindTradeItem(itemId);
            item.Status = ItemStatus.SOLD;

            try
            {
                //Делаем ItemRequest, чтобы бот ТМ инициировал обмен в Стим
                string botid = "1";

                //Выполняем асинхронно, чтобы как можно скорее возвратиться в вызывающий метод
                //и он продолжил следить за сокетами
                var itemrequest = await Task.Run(() => api.ItemRequest(ItemRequestDirection.IN, botid));

                if (itemrequest == null || !itemrequest.success)
                {
                    Log.e($"Произошла ошибка при выполнении ItemRequest");
                    return;
                }

                Log.d($"Выполнен запрос на обмен, бот: {itemrequest.nick}, сообщение: {itemrequest.secret}");

                //Необходимо поместить ID бота и сообщение в список, который будет использоваться при поиске трейдов в стиме
                SteamTradeContainer.Trades.PushTrade(itemrequest);

                //Необходимо пометить это трейд, чтобы он не продавался
                item.Status = ItemStatus.SOLD_REQUEST;

            }
            catch (Exception exp)
            {
                Log.e($"Произошла ошибка при выполнении ItemRequest: {exp.Message}");
            }
        }

    }
}