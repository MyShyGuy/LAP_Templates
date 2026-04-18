using DB_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BLDAL.Repos
{
    public class DisplayRepo : IDisplayRepo
    {
        private readonly AppDBContext dbc;

        public DisplayRepo(AppDBContext dbc)
        {
            this.dbc = dbc;
        }

        public List<Advertisement> GetAdvertisements(bool onlyActive = false)
        {
            var query = dbc.Advertisements.AsQueryable();
            if (onlyActive)
            {
                query = query.Where(a => a.IsActive);
            }

            return query
                .OrderBy(a => a.DisplayOrder)
                .ThenByDescending(a => a.CreatedAt)
                .ToList();
        }

        public Advertisement AddAdvertisement(Advertisement advertisement)
        {
            dbc.Advertisements.Add(advertisement);
            dbc.SaveChanges();
            return advertisement;
        }

        public void UpdateAdvertisement(Advertisement advertisement)
        {
            dbc.Advertisements.Update(advertisement);
            dbc.SaveChanges();
        }

        public void DeleteAdvertisement(int adId)
        {
            var entity = dbc.Advertisements.FirstOrDefault(a => a.AdID == adId);
            if (entity == null)
            {
                return;
            }

            dbc.Advertisements.Remove(entity);
            dbc.SaveChanges();
        }

        public List<BarMenuItem> GetMenuItems(bool onlyActive = false)
        {
            var query = dbc.BarMenuItems.AsQueryable();
            if (onlyActive)
            {
                query = query.Where(m => m.IsActive);
            }

            return query
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Title)
                .ToList();
        }

        public BarMenuItem AddMenuItem(BarMenuItem menuItem)
        {
            dbc.BarMenuItems.Add(menuItem);
            dbc.SaveChanges();
            return menuItem;
        }

        public void UpdateMenuItem(BarMenuItem menuItem)
        {
            dbc.BarMenuItems.Update(menuItem);
            dbc.SaveChanges();
        }

        public void DeleteMenuItem(int menuItemId)
        {
            var entity = dbc.BarMenuItems.FirstOrDefault(m => m.MenuItemID == menuItemId);
            if (entity == null)
            {
                return;
            }

            dbc.BarMenuItems.Remove(entity);
            dbc.SaveChanges();
        }

        public List<BarAction> GetBarActions(bool onlyActive = false, DateTime? now = null)
        {
            var query = dbc.BarActions.AsQueryable();
            if (onlyActive)
            {
                var current = now ?? DateTime.Now;
                query = query.Where(a => a.IsActive && a.StartsAt <= current && a.EndsAt >= current);
            }

            return query
                .OrderBy(a => a.StartsAt)
                .ToList();
        }

        public BarAction AddBarAction(BarAction barAction)
        {
            dbc.BarActions.Add(barAction);
            dbc.SaveChanges();
            return barAction;
        }

        public void UpdateBarAction(BarAction barAction)
        {
            dbc.BarActions.Update(barAction);
            dbc.SaveChanges();
        }

        public void DeleteBarAction(int actionId)
        {
            var entity = dbc.BarActions.FirstOrDefault(a => a.ActionID == actionId);
            if (entity == null)
            {
                return;
            }

            dbc.BarActions.Remove(entity);
            dbc.SaveChanges();
        }
    }
}
