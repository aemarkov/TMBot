using System;
using System.Windows.Input;
using TMBot.Utilities.MVVM;
using TMBot.Settings;

namespace TMBot.ViewModels
{
    /// <summary>
    /// Модель вида для настроек
    /// </summary>
    public class SettingsViewModel
    {
        /* Реализация диалогового окна
         * через MVVM какая-то анальная. Там еще Code Behind 
         * и прочие радости.
         * 
         * Нагуглено:
         *   1) Это  MVVM, детка, модель вид не должна знать о том, что это 
         *      диалоговое окно (что логично)
         *   2) Поэтому не парься, о диалоговом окне должна париться View
         *       - т.е. Code Behind (тоже логично)
         *   3) А связывать "Главную" модель вида и диалога можно событиями
         *   
         *   ПОГОДИТЕ-КА. Я нажрался диалогов которые возвращают все через
         *   события на андроиде.
         *   
         */ 

        public event Action<bool?> ActionSelected;

        public ICommand SaveCommand
        {
            get { return new RelayCommands(save); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommands(cancel); }
        }


        public Settings.Settings Settings { get; set; }

        public SettingsViewModel()
        {
            Settings = SettingsManager.LoadSettings();
        }

        //Сохранение настроек
        private void save(object param)
        {
            SettingsManager.SaveSettings(Settings);
            ActionSelected?.Invoke(true);
        }

        //Закрытие окна без сохранения
        public void cancel(object param)
        {
            ActionSelected?.Invoke(false);
        }
    }
}