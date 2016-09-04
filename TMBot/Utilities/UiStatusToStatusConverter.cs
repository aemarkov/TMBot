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
            switch (ui_status)
            {
                case 1:
                    return ItemStatus.TRADING;
                case 2:
                    return ItemStatus.SOLD;
                default:
                    return ItemStatus.UNKNOWN;
            }
        }
    }
}