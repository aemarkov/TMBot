﻿using AutoMapper;
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
               MapTradeToTradeItemViewModel(cfg);
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
        }
    }
}