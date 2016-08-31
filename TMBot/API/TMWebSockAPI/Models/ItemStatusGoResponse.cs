namespace TMBot.API.TMWebSockAPI.Models
{
    /// <summary>
    /// Ответ при событии передачи предмета от продавца боту
    /// </summary>
    /// <summary>
    /// Ответ на itemstatus_go (уведомление о покупке)
    /// </summary>
    public class ItemStatusGoResponse
    {
        /// <summary>
        /// ID предмета
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Статус предмета
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// ID бота, с которым мы должны совершить ItemRequest
        /// </summary>
        public string bid { get; set; }
        public int left { get; set; }
    }


}