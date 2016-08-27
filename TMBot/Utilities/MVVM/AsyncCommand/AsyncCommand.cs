using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TMBot.Utilities.MVVM.AsyncCommand
{
	/// <summary>
	/// Бомж-версия Relay Command но с поддержкой асинхронности
	/// </summary>
	/// <typeparam name="TResult">Тип возвращаемого значения (Task TResult), void (Task)
	/// не поддерживается</typeparam>
	public class AsyncCommand<TResult> : INotifyPropertyChanged, IAsyncCommand
	{
		private readonly Func<object, Task<TResult>> _command;
		private NotifyTaskCompletion<TResult> _execution;

        public AsyncCommand(Func<object, Task<TResult>> command)
        {
            _command = command;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
		{
			return true;
		}


        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object parameter)
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