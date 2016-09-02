using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMBot.API.SteamAPI;
using TMBot.Data;
using TMBot.Models.Steam;
using TMBot.Settings;
using TMBot.Utilities;

namespace TMBot.Workers
{
    /// <summary>
    /// Мониторит предложения обмена в стиме
    /// </summary>
    public class SteamWorker : BaseWorker
    {
        private ISteamAPI _api;

        public SteamWorker(ISteamAPI api)
        {
            _api = api;
        }

        public override void Start()
        {
            RunThread();
        }

        protected override async void worker_tread()
        {
            while (IsRunning)
            {
                var trades = await _api.GetSteamTradesAsync();
                var offersSent = trades.response.trade_offers_sent;
                acceptReceiveOffers(trades.response.trade_offers_received);
            }
        }
        
        //Проверяет, какие из входящих предложений были отправлены ботом ТМ
        private void acceptReceiveOffers(IList<SteamTrades.TradeOffer> offers)
        {
            foreach (var offer in offers)
            {
                var trade = SteamTradeContainer.Trades.PopTrade(offer.accountid_other, offer.message);
                if (trade != null)
                {
                    //Это предложение отправлено ботом ТМ - подтверждаем его
                    Log.d($"Подтверждаем предложение от {trade.nick}");

                    //TODO: подтверждение обмена
                }
            }

            Thread.Sleep(SettingsManager.LoadSettings().SteamCheckInterval);
        }
    }
}