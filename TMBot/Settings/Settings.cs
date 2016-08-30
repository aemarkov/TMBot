using System.ComponentModel.DataAnnotations;

namespace TMBot.Settings
{
    /// <summary>
    /// Представляет настройки приложения
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// API-key ТМ
        /// </summary>
        [NeedReload]
        [Required]
        public string TMApiKey { get; set; }

        /// <summary>
        /// API-key Steam
        /// </summary>
        [NeedReload]
        [Required]
        public string SteamApiKey { get; set; }
        
        /// <summary>
        /// ID пользователя Steam
        /// .../profiles/xxxxxxx/
        /// </summary>
        [NeedReload]
        [Required]
        public string SteamProfileId { get; set; }

        /// <summary>
        /// Максимальная разница между нашей_максимальной и следующей ценой, при
        /// который наша цена не опускается
        /// </summary>
        public float TradeMaxThreshold { get; set; } = 0.1f;

        /// <summary>
        /// Максимальная разница между нашей_минимальной и следующей ценой, при
        /// который наша цена не поднимается
        /// </summary>
        public float OrderMinThreshold { get; set; } = 0.1f;
    }
}