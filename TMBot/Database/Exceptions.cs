using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMBot.Database
{
    public class DatabaseException : Exception
    {
        public DatabaseException() : base("Exception in database") { }
        public DatabaseException(String message) : base(message) { }
    }

    public class EntityAlreadyExistsException:DatabaseException
    {
        public EntityAlreadyExistsException() : base("Entity already exists") { }
    }

    public class EntityNotFoundException:DatabaseException
    {
        public EntityNotFoundException():base("Entityt not found") { }
    }




	public class LogicException:Exception
	{
		public LogicException() : base("Logic exception") { }
		public LogicException(String message) : base(message) { }
	}

	public class EntityIsClosedException:LogicException
	{
		public EntityIsClosedException() : base("Enitity is already closed, you can't change it") { }
	}
}