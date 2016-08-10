using System.Threading.Tasks;
using System.Windows.Input;

namespace TMBot.Utilities.MVVM
{
	/// <summary>
	/// Аснихронный интерфейс ICommand
	/// </summary>
	public interface IAsyncCommand : ICommand
	{
		Task ExecuteAsync(object parameter);
	}
}