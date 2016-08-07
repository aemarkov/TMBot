using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.API.Factory
{
	/// <summary>
	/// Фабрика для создания АИ
	///  - Создает разные реализации интерфейса API
	///  - Хранит у себя
	///  - Выдает по требованию
	///  
	/// Чтобы существовала единственная реализациия АПИ
	/// </summary>
	public class AbstactAPIFactory<TAPI> where TAPI : IAbstractAPI
	{
		private static volatile AbstactAPIFactory<TAPI> instance;
		private static object sync_root = new object();

		//Список реализаций API
		protected Dictionary<Type, TAPI> apis;

		protected AbstactAPIFactory()
		{
			apis = new Dictionary<Type, TAPI>();
		}

		/// <summary>
		/// Возвращает экземпляр фабрики
		/// </summary>
		/// <typeparam name="TFactory">Тип факбрики</typeparam>
		/// <returns></returns>
		public static TFactory GetInstance<TFactory>() where TFactory : AbstactAPIFactory<TAPI>, new()
		{
			if (instance == null)
			{
				lock (sync_root)
				{
					if (instance == null)
						instance = new TFactory();
				}
			}

			return instance as TFactory;

		}

		/// <summary>
		/// Возвращает реализацию API заданного типа
		/// (предварительно, уже созданную)
		/// </summary>
		/// <typeparam name="T">Тип API</typeparam>
		/// <returns></returns>
		public TAPI GetAPI<T>() where T : TAPI
		{
			Type t = typeof(T);
			if (!apis.ContainsKey(t))
				throw new Exception("No such API");

			return apis[t];
		}
	}
}
