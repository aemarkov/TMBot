using System.Linq;
using TMBot.Models;

namespace TMBot.Database
{
    /// <summary>
    /// Репозиторий предметов
    /// </summary>
    public class ItemsRepository:AbstractRepository<Item>
    {
        protected override bool ModelEqual(Item modelA, Item modelB)
        {
            return modelA.ClassId == modelB.ClassId && modelA.InstanceId == modelB.InstanceId;
        }

        public Item GetById(string classid, string instanceid)
        {
            return Context.Set<Item>().FirstOrDefault(x => x.InstanceId == instanceid && x.ClassId == classid);
        }
    }
}