using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TMBot.Utilities.MVVM
{
	/// <summary>
	/// Бомж-версия Relay Command но с поддержкой асинхронности
	/// </summary>
	/// <typeparam name="TResult">Тип возвращаемого значения (Task TResult), void (Task)
	/// не поддерживается</typeparam>
	public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
	{
		private readonly Func<object, Task<TResult>> _command;
		private NotifyTaskCompletion<TResult> _execution;

		public AsyncCommand(Func<object, Task<TResult>> command)
		{
			_command = command;
		}

		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override Task ExecuteAsync(object parameter)
		{
			Execution = new NotifyTaskCompletion<TResult>(_command(parameter));
			return Execution.TaskCompletion;
		}

		public NotifyTaskCompletion<TResult> Execution
		{
			get { return _execution; }
			private set
			{
				_execution = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// Создает AsyncCommand, поддерживая void функции и функции с возвращаемым
	/// значением
	/// </summary>
	public static class AsyncCommand
	{
		public static AsyncCommand<object> Create(Func<object, Task> command)
		{
			return new AsyncCommand<object>(async (param) => { await command(param); return null; });
		}

		public static AsyncCommand<TResult> Create<TResult>(Func<object, Task<TResult>> command)
		{
			return new AsyncCommand<TResult>(command);
		}
	}
}