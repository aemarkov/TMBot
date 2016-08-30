using System.CodeDom;
using System.IO;
using Newtonsoft.Json;

namespace TMBot.Settings
{
    /// <summary>
    /// Управление настройками приложения
    /// </summary>
    public static class SettingsManager
    {
        private static readonly string _settingsName = "settings.json";

        //Загружает настройки или создает новый файл
        public static Settings LoadSettings()
        {
            if (!File.Exists(_settingsName))
            {
                var settings = new Settings();
                save(settings);
                return settings;
            }
            else
            {
                string json = File.ReadAllText(_settingsName);
                return JsonConvert.DeserializeObject<Settings>(json);
            }
        }

        //Сохраняет настройки
        public static void SaveSettings(Settings settings)
        {
            save(settings);
        }

        private static void save(Settings settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(_settingsName, json);
        }
    }
}