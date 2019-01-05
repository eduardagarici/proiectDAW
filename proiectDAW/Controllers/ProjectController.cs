using Microsoft.AspNet.Identity;
using proiectDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiectDAW.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var projects = from project in db.Projects
                           where project.OwnerId == id
                           orderby project.Status
                           select project;
            List<Project> projectList = new List<Project>(projects);
            return View(projectList);
        }
        [Authorize(Roles = "Member,Organizer,Administrator")]
        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Member,Organizer,Administrator")]
        [HttpPost]
        public ActionResult New(Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    project.StartDate = DateTime.Now;
                    project.OwnerId = User.Identity.GetUserId();
                    db.Projects.Add(project);
                    db.SaveChanges();

                }
            }
            catch 
            {

            }
            return View();
        }


    }
}