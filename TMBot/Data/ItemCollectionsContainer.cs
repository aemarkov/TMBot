using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.Factory;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Data
{
    /// <summary>
    /// Хранит и предоставляет доступ к 
    /// спискам предметов, которые обрабатываются 
    /// </summary>
    public class ItemCollectionsContainer
    {
        private static volatile ItemCollectionsContainer instance;
        private static object syncRoot = new object();
        public Dictionary<TradePlatform, TradePlatformConainer> Platforms { get; }

        private ItemCollectionsContainer()
        {
            Platforms = new Dictionary<TradePlatform, TradePlatformConainer>();
        }

        /// <summary>
        /// Возвращает ссылку на экземпляр
        /// </summary>
        /// <returns></returns>
        public static ItemCollectionsContainer GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ItemCollectionsContainer();
                }
            }

            return instance;
        }

        /// <summary>
        /// Создает списки предметов для заданной платформы
        /// </summary>
        /// <param name="platform"></param>
        public void CreateList(TradePlatform platform)
        {
            if(Platforms.ContainsKey(platform))
                throw new ArgumentException("This list is already created");

            TradePlatformConainer container = new TradePlatformConainer();
            container.Trades = new SynchronizedObservableCollection<ItemViewModel>();
            container.Orders = new SynchronizedObservableCollection<ItemViewModel>();
            Platforms.Add(platform, container);
        }

        /// <summary>
        /// Возвращает списки предметов для заданной платформы
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public TradePlatformConainer GetList(TradePlatform platform)
        {
            if(!Platforms.ContainsKey(platform))
                throw new ArgumentException("This list doesn't created");

            return Platforms[platform];
        }

        /// <summary>
        /// Находит предмет среди трейдов по его ID
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ItemViewModel FindTradeItem(string itemId)
        {
            return Platforms.Select(platform => platform.Value.Trades.FirstOrDefault(x => x.ItemId == itemId)).FirstOrDefault(item => item != null);
        }

        /// <summary>
        /// Находит МНОЖЕСТВО предметов по его classid, instanceid
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="instanceid"></param>
        /// <returns>Список подходящих предметов</returns>
        public IEnumerable<ItemViewModel> FindTradeItems(string classid, string instanceid)
        {
            return Platforms.Aggregate((IEnumerable<ItemViewModel>)new List<ItemViewModel>(), (list, item) => 
                        list.Concat(item.Value.Trades.Where(y => y.ClassId == classid && y.IntanceId == instanceid)));
        }

        /// <summary>
        /// Находит ордер по classid, instanceid. Ордер всегда единственный
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        public ItemViewModel FindOrderItem(string classid, string instanceid)
        {
            return Platforms.Select(platform => platform.Value.Orders.FirstOrDefault(x => x.ClassId == classid && x.IntanceId==instanceid)).FirstOrDefault(item => item != null);
        }

        public ItemViewModel FindOrderItem(string itemId)
        {
            return Platforms.Select(platform => platform.Value.Orders.FirstOrDefault(x => x.ItemId == itemId)).FirstOrDefault(item => item != null);
        }


        /// <summary>
        /// Находит и удаляет предмет среди ордеров и трейдов
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(ItemViewModel item)
        {
            foreach (var platform in Platforms)
            {
                if (platform.Value.Trades.Contains(item))
                {
                    platform.Value.Trades.Remove(item);
                    return;
                }

                if (platform.Value.Orders.Contains(item))
                {
                    platform.Value.Orders.Remove(item);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Тип торговой площадки
    /// </summary>
    public enum TradePlatform
    {
        CSGO,
        DOTA2,
        TF2,
        GIFTS
    }

    /// <summary>
    /// Контейнер для объединения продающийхся и покупающихся
    /// предметов
    /// </summary>
    public class TradePlatformConainer
    {
        public SynchronizedObservableCollection<ItemViewModel> Trades { get; set; }
        public SynchronizedObservableCollection<ItemViewModel> Orders { get; set; } 
    }
}
