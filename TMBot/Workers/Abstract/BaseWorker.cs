using System.Threading;
using System.Windows.Documents;
using TMBot.Utilities.MVVM;

namespace TMBot.Workers
{
    public abstract class BaseWorker : PropertyChangedBase
    {
        /// <summary>
        /// Запущен ли поток обновления
        /// </summary>
        #region IsRunning
        public bool IsRunning
        {
            get { return _isRunning; }
            protected set { _isRunning = value; NotifyPropertyChanged(); }
        }
        private volatile bool _isRunning;
        #endregion


        //Фоновый поток обновления цен
        protected Thread _workThread;

        /// <summary>
        /// Запуск потока
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Остановка потока
        /// </summary>
        public virtual void Stop()
        {
            IsRunning = false;
        }

        protected virtual void RunThread()
        {
            IsRunning = true;
            _workThread = new Thread(worker_tread);
            _workThread.Start();
        }

        protected abstract void worker_tread();
    }

}