using System;
using System.Data.Entity;
using System.Linq;
namespace TMBot.Database
{
    /// <summary>
    /// Абстрактный класс репозитория для доступа к данным
    /// </summary>
    public abstract class AbstractRepository<TModel> where TModel:class 
    {
        protected DatabaseContext Context;

        protected AbstractRepository()
        {
            Context = new DatabaseContext(); ;
        }

        protected AbstractRepository(DatabaseContext context)
        {
            Context = context;
        }

        public IQueryable<TModel> Entities
        {
            get { return Context.Set<TModel>(); }
        }

        /// <summary>
        /// Обновляет сущность
        /// </summary>
        /// <param name="entity"></param>
        public void Update(TModel entity)
        {
            if (Context.Set<TModel>().Any(x=> ModelEqual(x,entity)))
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
            }
            else
                throw new EntityNotFoundException();
        }

        /// <summary>
        /// Создает сущность
        /// </summary>
        /// <param name="entity"></param>
        public void Create(TModel entity)
        {

            if (!Context.Set<TModel>().Any(x => ModelEqual(x,entity)))
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
            }
            else
                throw new EntityAlreadyExistsException();
        }

        /// <summary>
        /// Удаляет сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        public void Delete(TModel entity)
        {
            DbSet<TModel> set = Context.Set<TModel>();
            TModel m = set.FirstOrDefault(x => ModelEqual(x, entity));
            if (m != null)
            {
                set.Remove(m);
                Context.SaveChanges();
            }
            else
                throw new EntityNotFoundException();
        }

        protected abstract bool ModelEqual(TModel modelA, TModel modelB);
    }
}