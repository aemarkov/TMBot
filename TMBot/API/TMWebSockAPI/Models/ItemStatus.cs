namespace TMBot.API.TMWebSockAPI.Models
{
    /// <summary>
    /// Ответ при событии передачи предмета от продавца боту
    /// </summary>
    public class ItemStatus
    {
        public string id { get; set; }
        public int status { get; set; }
        public string bid { get; set; }
        public int left { get; set; }
    }


}