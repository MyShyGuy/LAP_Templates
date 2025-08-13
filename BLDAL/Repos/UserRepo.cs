using DB_Models.DTO;
using DB_Models.Models;
using DB_Models.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDBContext dbc;
        private readonly PasswordService PwS;

        public UserRepo(AppDBContext dbContext)
        {
            this.dbc = dbContext;
            PwS = new PasswordService();
        }

        public User? RegisterUser(LoginDTO usr)
        {
            bool UserExist = dbc.Users.Where(x => x.UserName == usr.UserName).Any();
            if (!UserExist)
            {
                var newUserID = Guid.NewGuid().ToString();
                var InputPWHash = PwS.ComputeHash(usr.Password, newUserID);


                // Suche die "Guest"-Rolle in der Datenbank
                //var guestRole = dbc.Roles.FirstOrDefault(r => r.RoleName == "Admin");
                //var guestRole1 = dbc.Roles.FirstOrDefault(r => r.RoleName == "User");
                var guestRole2 = dbc.Roles.FirstOrDefault(r => r.RoleName == "Guest");
                var roles = new List<Role>();
                if (guestRole2 != null)
                {
                    //roles.Add(guestRole);
                    //roles.Add(guestRole1);
                    roles.Add(guestRole2);
                }

                User newUser = new User() { UserID = newUserID, UserName = usr.UserName, PasswordHash = InputPWHash, EntryDate = DateTime.Now, Roles = roles };
                dbc.Users.Add(newUser);
                dbc.SaveChanges();
                return newUser;
            }
            else
                return null;
            }
        public User? UserLogin(LoginDTO usr)
        {
            var user = dbc.Users.Include(u => u.Roles).SingleOrDefault(x => x.UserName == usr.UserName);
            if (user == null)
                return null;

            var UserID = user.UserID;
            var InputPWHash = PwS.ComputeHash(usr.Password, UserID);
            if (InputPWHash == user.PasswordHash)
                return user;
            else
                return null;
        }
    }
}
