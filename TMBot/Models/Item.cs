using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMBot.Models
{
    /// <summary>
    /// Настраиваемые параметры для покупаемого\продаваемого предмета
    /// </summary>
    public class Item
    {
        [Key, Column(Order = 0)]
        [MaxLength(15)]
        public string ClassId { get; set; }
        [Key, Column(Order = 1)]
        [MaxLength(15)]
        public string InstanceId { get; set; }

        /// <summary>
        /// Максимальная\минимальная цена за которую не может
        /// выходить бот при торге
        /// </summary>
        [Required]
        public int PriceLimit { get; set; }

        /// <summary>
        /// Максимальное число предметов, которые бот
        /// покупает\продает (на продажу вряд ли распространяется)
        /// </summary>
        public int? CountLimit { get; set; }
    }
}