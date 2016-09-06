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
                case 3:
                    return ItemStatus.BOUGHT;
                case 4:
                    return ItemStatus.BOUGHT_TAKE;
                default:
                    return ItemStatus.UNKNOWN;
            }
        }
    }
}