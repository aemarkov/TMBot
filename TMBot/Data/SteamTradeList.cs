using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper.QueryableExtensions.Impl;
using TMBot.API.TMAPI.Models;

namespace TMBot.Data
{
    /// <summary>
    /// Реализует взаимодействие между мониторингом 
    /// предложений обмена в Steam и событиями
    /// веб-сокетов
    /// 
    /// Хранит список ников, id и фраз ботов для
    /// дальнейшей проверки.
    /// 
    /// Реализует потокобезопасный доступ
    /// </summary>
    public class SteamTradeList
    {
        private LinkedList<ItemRequestResponse> _trades;
        private object _lock;

        public SteamTradeList()
        {
            _trades = new LinkedList<ItemRequestResponse>();
            _lock = new object();
        }

        /// <summary>
        /// Добавление информации о запланированном обмене
        /// </summary>
        /// <param name="itemRequest">Ответ на запрос ItemRequest, который содержит
        /// ник бота, id бота и секретную фразу</param>
        public void PushTrade(ItemRequestResponse itemRequest)
        {
            lock (_lock)
            {
                _trades.AddLast(itemRequest);
                //Monitor.Pulse(_lock);
            }
        }

        /// <summary>
        /// Находит трейд по заданным параметрам. Если трайд найден - удаляет 
        /// его из списка и возвращает
        /// </summary>
        /// <param name="botid">ID учетной записи бота</param>
        /// <param name="nick">Ник бота</param>
        /// <param name="secret">Секретная фраза</param>
        /// <returns>Информацию о запрсое, null если по заданным данным предложения не найдено</returns>
        public ItemRequestResponse PopTrade(string botid, string nick, string secret)
        {
            ItemRequestResponse trade = null;

            lock (_lock)
            {
                trade = _trades.FirstOrDefault(x => x.botid == botid && x.nick == nick && x.secret == secret);
                if (trade != null)
                {
                    _trades.Remove(trade);
                }
            }

            return trade;
        }
    }
}