using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using proiectDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiectDAW.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        private UserManager<ApplicationUser> UserManager;
  
        // GET: Users

        public ActionResult Index()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            List<string> roles = new List<string>();
            foreach (var user in users)
            {
                roles.Add(UserManager.GetRoles(user.Id)[0]);
            }
            ViewBag.Roles = roles;
            if (TempData.ContainsKey("yourself"))
                ViewBag.Yourslef = TempData["yourself"];
            return View(users.ToList());
        }

        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            // user.AllRoles = GetAllRoles();
            //var userRole = user.Roles.FirstOrDefault();
            //ViewBag.userRole = userRole.RoleId;
            ViewBag.userRoleName = UserManager.GetRoles(user.Id)[0];
            return View(user);
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData, string selectedRole)
        {

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            try
            {
             
                var roleManager = new RoleManager<IdentityRole>(new
               RoleStore<IdentityRole>(db));
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(db));

                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(user))
                    {
                        user.UserName = newData.UserName;
                        user.Email = newData.Email;
                        user.PhoneNumber = newData.PhoneNumber;
                        var roles = from role in db.Roles select role;
                        foreach (var role in roles)
                        {
                            UserManager.RemoveFromRole(id, role.Name);
                        }
                        //var selectedRole =  db.Roles.Find(HttpContext.Request.Params.Get("selectedRole"));
                        UserManager.AddToRole(id, selectedRole);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return View(user);
            }

        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            if(id == User.Identity.GetUserId())
            {
                TempData["yourself"] = "You can not delete your own user";
                return RedirectToAction("Index");
            }
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = UserManager.FindById(id);
            foreach(var project in user.Projects)
            {
                project.TeamMembers.Remove(user);
            }
            foreach(var task in user.Tasks)
            {
                task.AssignedMembers.Remove(user);
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}