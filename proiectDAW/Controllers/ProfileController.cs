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
    [Authorize(Roles ="Member,Organizer,Administrator")]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        private UserManager<ApplicationUser> UserManager;
        // GET: Profile
        public ActionResult Index()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = UserManager.FindById(User.Identity.GetUserId());
            return View(user);
        }

        public ActionResult Edit(string id)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = UserManager.FindById(User.Identity.GetUserId());
            return View(user);
        }

        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {

            ApplicationUser user = db.Users.Find(id);
            try
            { 
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(db));

                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(user))
                    {
                        user.UserName = newData.UserName;
                        user.Email = newData.Email;
                        user.PhoneNumber = newData.PhoneNumber;
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
    }
}