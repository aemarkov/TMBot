using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TMBot.Utilities.MVVM
{
	/// <summary>
	/// Особая асинхронная магия для того, чтобы делать
	/// Binding асинхронныз операций
	/// </summary>
	/// <typeparam name="TResult">Тип возвращаемого значения (Task TResult), void (Task)
	/// не поддерживается</typeparam>
	public class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public Task<TResult> Task { get; private set; }
		public Task TaskCompletion { get; private set; }


		public NotifyTaskCompletion(Task<TResult> task)
		{
			Task = task;
		    TaskCompletion = WatchTaskAsync(task);
		}

		private async Task WatchTaskAsync(Task task)
		{
            await Task;	

			var propertyChanged = PropertyChanged;
			if (propertyChanged == null)
				return;

			propertyChanged(this, new PropertyChangedEventArgs("Status"));
			propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
			propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
			if (task.IsCanceled)
			{
				propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
			}
			else if (task.IsFaulted)
			{
				propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
				propertyChanged(this, new PropertyChangedEventArgs("Exception"));
				propertyChanged(this,
				  new PropertyChangedEventArgs("InnerException"));
				propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
			}
			else
			{
				propertyChanged(this,
				  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
				propertyChanged(this, new PropertyChangedEventArgs("Result"));
			}


		}

		public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
		public TaskStatus Status => Task.Status;
		public bool IsCompleted => Task.IsCompleted;
		public bool IsNotCompleted => !Task.IsCompleted;

		public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
		public bool IsCanceled => Task.IsCanceled;
		public bool IsFaulted => Task.IsFaulted;
		public AggregateException Exception => Task.Exception;

		public Exception InnerException => Exception?.InnerException;

		public string ErrorMessage => InnerException?.Message;
	}
}