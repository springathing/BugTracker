using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.Helpers
{
    public class UserRoleHelper
    {
        //instantiate usermanager class, requires userstore to pass in the constructor, so you have instance of userstore, but userstore constructor requires applicationdbcontext instance to pass
        private UserManager<ApplicationUser> userManager = 
            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>
                (new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();

        // now check if user is in role, if so, return true, if not, return false
        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }
        // return list of roles
        public ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }
        // add user to a specific role, if successful return true
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        // remove user from role
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }
        // bringing all the users per a certain role
        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            List<ApplicationUser> resultList = new List<ApplicationUser>(); // implicit declaration of variable with var, explicit is List<ApplicationUser> resultList
            List<ApplicationUser> List = userManager.Users.ToList();
            foreach(var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
        // bring all users not in a role
        public ICollection<ApplicationUser> UsersNotInRole(string roleName)
        {
            List<ApplicationUser> resultList = new List<ApplicationUser>();
            List<ApplicationUser> List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }
}