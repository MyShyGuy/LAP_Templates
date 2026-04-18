using DB_Models.Models;

namespace BLDAL.Repos
{
    public interface IDisplayRepo
    {
        List<Advertisement> GetAdvertisements(bool onlyActive = false);
        Advertisement AddAdvertisement(Advertisement advertisement);
        void UpdateAdvertisement(Advertisement advertisement);
        void DeleteAdvertisement(int adId);

        List<BarMenuItem> GetMenuItems(bool onlyActive = false);
        BarMenuItem AddMenuItem(BarMenuItem menuItem);
        void UpdateMenuItem(BarMenuItem menuItem);
        void DeleteMenuItem(int menuItemId);

        List<BarAction> GetBarActions(bool onlyActive = false, DateTime? now = null);
        BarAction AddBarAction(BarAction barAction);
        void UpdateBarAction(BarAction barAction);
        void DeleteBarAction(int actionId);
    }
}
