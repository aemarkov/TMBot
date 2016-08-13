using System.Data.Entity;
using System.Linq;
using TMBot.Models;

namespace TMBot.Database
{
    /// <summary>
    /// Репозиторий предметов
    /// </summary>
    public class ItemsRepository:AbstractRepository<Item>
    {
        public Item GetById(string classid, string instanceid)
        {
            return Context.Set<Item>().FirstOrDefault(x => x.InstanceId == instanceid && x.ClassId == classid);
        }

        public override void Update(Item entity)
        {
            if (Context.Set<Item>().Any(x => x.ClassId==entity.ClassId && x.InstanceId==entity.InstanceId))
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
            }
            else
                throw new EntityNotFoundException();
        }

        public override void Create(Item entity)
        {

            if (!Context.Set<Item>().Any(x => x.ClassId==entity.ClassId && x.InstanceId==entity.InstanceId))
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
            }
            else
                throw new EntityAlreadyExistsException();
        }

        public override void Delete(Item entity)
        {
            DbSet<Item> set = Context.Set<Item>();
            Item m = set.FirstOrDefault(x => x.ClassId==entity.ClassId && x.InstanceId==entity.ClassId);
            if (m != null)
            {
                set.Remove(m);
                Context.SaveChanges();
            }
            else
                throw new EntityNotFoundException();
        }
    }
}