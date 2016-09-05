using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.Utilities;

namespace TMBot.Workers
{
    /// <summary>
    /// Раз в 3 минуты отправляет запрос ping 
    /// </summary>
    public class PingWorker : BaseWorker
    {
        private ITMAPI api;
        private CancellationTokenSource token;

        public PingWorker() : base()
        {
            api = TMFactory.GetInstance<TMFactory>().GetAPI<CSTMAPI>();
        }

        public override void Start()
        {
            RunThread();
        }

        public override void Stop()
        {
            base.Stop();
            token.Cancel();
        }

        protected override void worker_tread()
        {
            while (IsRunning)
            {
                try
                {
                    token = new CancellationTokenSource();
                    token.Token.WaitHandle.WaitOne(3*60*1000+10*1000);

                    var pong = api.PingPong();

                    if (pong.success)
                        Log.d("Успешный пинг");
                    else
                        Log.w($"Ошибка пинга: {pong.ping}");
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception exp)
                {
                    Log.e($"Ошибка пинга: {exp.Message}");
                }
                

            }
        }
    }
}