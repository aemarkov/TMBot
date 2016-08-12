using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.API.Exceptions
{
	/// <summary>
	/// Общая ошибка API
	/// </summary>
	public class APIException : Exception
	{
		public APIException() : base() { }

		public APIException(string message) : base(message) { }
	}

    public class ItemNotFoundException : APIException
    {
   
    }
}
