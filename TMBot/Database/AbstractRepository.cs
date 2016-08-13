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
        public abstract void Update(TModel entity);

        /// <summary>
        /// Создает сущность
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Create(TModel entity);

        /// <summary>
        /// Удаляет сущность
        /// </summary>
        /// <param name="entity">Сущность</param>
        public abstract void Delete(TModel entity);
    }
}