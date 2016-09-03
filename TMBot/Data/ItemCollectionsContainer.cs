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
            container.Trades = new SynchronizedObservableCollection<TradeItemViewModel>();
            container.Orders = new SynchronizedObservableCollection<TradeItemViewModel>();
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
        public SynchronizedObservableCollection<TradeItemViewModel> Trades { get; set; }
        public SynchronizedObservableCollection<TradeItemViewModel> Orders { get; set; } 
    }
}
