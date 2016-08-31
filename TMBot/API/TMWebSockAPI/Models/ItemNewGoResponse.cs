namespace TMBot.API.TMWebSockAPI.Models
{
    /// <summary>
    /// Ответ при событии покупки у нас предмета
    /// </summary>
    public class ItemNewGoResponse
    {
        public string ui_id { get; set; }
        public string i_name { get; set; }
        public string i_market_name { get; set; }
        public string i_name_color { get; set; }
        public string i_rarity { get; set; }
        public object i_descriptions { get; set; }
        public string ui_status { get; set; }
        public string he_name { get; set; }
        public double ui_price { get; set; }
        public string i_classid { get; set; }
        public string i_instanceid { get; set; }
        public string ui_real_instance { get; set; }
        public double i_market_price { get; set; }
        public int position { get; set; }
        public int min_price { get; set; }
        public string ui_bid { get; set; }
        public string ui_asset { get; set; }
        public string type { get; set; }
        public string ui_uid { get; set; }
        public string ui_price_text { get; set; }
        public bool min_price_text { get; set; }
        public string i_market_price_text { get; set; }
        public string placed { get; set; }
    }
}