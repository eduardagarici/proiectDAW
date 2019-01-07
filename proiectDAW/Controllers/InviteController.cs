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
    public class InviteController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        private UserManager<ApplicationUser> UserManager;
        // GET: Invite
        [Authorize(Roles="Member,Administrator,Organizer")]
        public ActionResult Index(string searchParam = "")
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (User.IsInRole("Administrator"))
            {
                var inviteList = (from invite in db.Invites
                                  orderby invite.Date
                                  select invite).ToList();

                var invites = inviteList.Where(e => Int32.Parse(DateTime.Now.Subtract(e.Date).Days.ToString()) + e.Availability > 0);
                if (searchParam != "")
                {
                    searchParam = searchParam.Trim();
                    invites = invites.Where(e => e.Project.Name.Contains(searchParam) || e.Organizer.Email.Contains(searchParam));
                }
                return View(invites);

            }
            else
            {
                var invites = user.Invites.Where(e => Int32.Parse(DateTime.Now.Subtract(e.Date).Days.ToString()) + e.Availability > 0);
                if (searchParam != "")
                {
                    searchParam = searchParam.Trim();
                    invites = invites.Where(e => e.Project.Name.Contains(searchParam) || e.Organizer.Email.Contains(searchParam));
                }
                return View(invites);
            }
            
        }

        [Authorize(Roles ="Member,Administrator,Organizer")]
        [HttpDelete]
        public ActionResult Decline (int id)
        {
            try
            {
                var invite = db.Invites.Find(id);
                var project = db.Projects.Find(invite.ProjectId);
                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = UserManager.FindById(invite.InvitedId);
                project.Invites.Remove(invite);
                user.Invites.Remove(invite);
                db.Invites.Remove(invite);
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Member,Administrator,Organizer")]
        [HttpDelete]
        public ActionResult Accept(int id)
        {
            try
            {
                var invite = db.Invites.Find(id);
                var project = db.Projects.Find(invite.ProjectId);
                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = UserManager.FindById(invite.InvitedId);
                project.Invites.Remove(invite);
                project.TeamMembers.Add(user);
                user.Projects.Add(project);
                user.Invites.Remove(invite);
                db.Invites.Remove(invite);
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Index");
        }
    }
}
