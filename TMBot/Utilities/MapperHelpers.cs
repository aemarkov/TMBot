using System;
using AutoMapper;
using TMBot.API.TMAPI.Models;
using TMBot.Models;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Utilities
{
    /// <summary>
    /// Мапперы данных
    /// </summary>
    public static class MapperHelpers
    {
        public static void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                //MapTradeToTradeItemViewModel(cfg);
                cfg.CreateMap<Trade, TradeItemViewModel>().ConvertUsing<TradeToTradeItemViewModelConverter>();
                cfg.CreateMap<Order, TradeItemViewModel>().ConvertUsing<OrderToTradeItemViewModelConverter>();
                //MapOrderToTradeItemViewModel(cfg);
                cfg.CreateMap<Item, TradeItemViewModel>();
            });
        }


        private static void MapTradeToTradeItemViewModel(IMapperConfigurationExpression cfg)
        {
            var map = cfg.CreateMap<Trade, TradeItemViewModel>();
            map.ForMember(d => d.ItemId, o => o.MapFrom(s => s.ui_id));
            map.ForMember(d => d.IntanceId, o => o.MapFrom(s => s.ui_real_instance));
            map.ForMember(d => d.ClassId, o => o.MapFrom(s => s.i_classid));
            map.ForMember(d => d.Name, o => o.MapFrom(s => s.i_name));
            map.ForMember(d => d.MyPrice, o => o.MapFrom(s => s.ui_price));
            //map.ForMember(d=>d.Status, o=>o.MapFrom(s=>s.ui_status)).ConvertUsing<StatusTypeConverter>();
        }

        private static void MapOrderToTradeItemViewModel(IMapperConfigurationExpression cfg)
        {
            var map = cfg.CreateMap<Order, TradeItemViewModel>();
            map.ForMember(d => d.IntanceId, o => o.MapFrom(s => s.i_instanceid));
            map.ForMember(d => d.ClassId, o => o.MapFrom(s => s.i_classid));
            map.ForMember(d => d.Name, o => o.MapFrom(s => s.i_market_name));
            map.ForMember(d => d.MyPrice, o => o.MapFrom(s => s.o_price));
        }
    }

    class TradeToTradeItemViewModelConverter : ITypeConverter<Trade, TradeItemViewModel>
    {
        public TradeItemViewModel Convert(Trade source, TradeItemViewModel destination, ResolutionContext context)
        {
            var dest = new TradeItemViewModel();
            dest.ItemId = source.ui_id;
            dest.BotId = source.ui_bid;
            dest.IntanceId = source.ui_real_instance;
            dest.ClassId = source.i_classid;
            dest.Name = source.i_name;
            dest.MyPrice = (int)source.ui_price;
            dest.Status = UiStatusToStatusConverter.Convert(source.ui_status);

            return dest;

        }
    }

    class OrderToTradeItemViewModelConverter : ITypeConverter<Order, TradeItemViewModel>
    {
        public TradeItemViewModel Convert(Order source, TradeItemViewModel destination, ResolutionContext context)
        {
            var dest = new TradeItemViewModel();

            dest.IntanceId = source.i_instanceid;
            dest.ClassId = source.i_classid;
            dest.Name = source.i_market_name;
            dest.MyPrice = source.o_price;
            dest.Status = ItemStatus.ORDERING;

            return dest;
        }
    }
}