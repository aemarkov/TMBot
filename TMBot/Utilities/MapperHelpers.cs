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
                cfg.CreateMap<Trade, TradeItemViewModel>().ConvertUsing<TradeToTradeItemViewModelConverter>();
                cfg.CreateMap<Order, OrderItemViewModel>().ConvertUsing<OrderToOrderItemViewModelConverter>();
                //MapOrderToTradeItemViewModel(cfg);
                cfg.CreateMap<Item, TradeItemViewModel>()
                    .ForMember(o => o.PriceLimit, m => m.MapFrom(x => x.MinPrice));
                cfg.CreateMap<Item, OrderItemViewModel>()
                    .ForMember(o => o.PriceLimit, m => m.MapFrom(x => x.MaxPrice));
            });
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

    class OrderToOrderItemViewModelConverter : ITypeConverter<Order, OrderItemViewModel>
    {
        public OrderItemViewModel Convert(Order source, OrderItemViewModel destination, ResolutionContext context)
        {
            var dest = new OrderItemViewModel();

            dest.IntanceId = source.i_instanceid;
            dest.ClassId = source.i_classid;
            dest.Name = source.i_market_name;
            dest.MyPrice = source.o_price;
            dest.Status = ItemStatus.ORDERING;

            return dest;
        }
    }

    class ItemToTradeItemViewModelConverter
    {
        
    }
}