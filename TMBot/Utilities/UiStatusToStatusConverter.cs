using TMBot.ViewModels.ViewModels;

namespace TMBot.Utilities
{
    /// <summary>
    /// Преобразует ui_status ТМ в ItemStatus
    /// </summary>
    public static class UiStatusToStatusConverter
    {
        public static ItemStatus Convert(int ui_status)
        {
            if (ui_status == 1)
                return ItemStatus.TRADING;

            return ItemStatus.UNKNOWN;
        }
    }
}