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
    public class TaskController : Controller
    {
        // GET: Task
        private ApplicationDbContext db = new ApplicationDbContext();

        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }

        public string MockTasks()
        {

            ProjectTask pt = new ProjectTask();
            pt.Name = "task2";
            pt.Description = "text2";
            pt.ProjectId = 14;
            db.Tasks.Add(pt);
            db.SaveChanges();

            return "stefan";
        }

        public string Mock()
        {

            Project pt = new Project();
            pt.Name = "ProiectNou";
            pt.ProjectId = 1000;
            pt.OwnerId = User.Identity.GetUserId();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            pt.TeamMembers = new HashSet<ApplicationUser>();
            pt.TeamMembers.Add(UserManager.FindById(User.Identity.GetUserId()));
            db.Projects.Add(pt);
            db.SaveChanges();

            return "stefan";
        }

        public string Shows()
        {
            ProjectTask pp = db.Tasks.Find(16);
            foreach (ApplicationUser appUser in pp.AssignedMembers)
            {
                Response.Write(appUser.UserName);
            }
            return "avala";
        }

        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var tasks = from task in db.Tasks
                        where task.OwnerId == id
                        orderby task.Status
                        select task;
            List<ProjectTask> taskList = new List<ProjectTask>(tasks);
            return View(taskList);
        }

        public ActionResult New()
        {
            //ViewBag.ProjectId = db.Projects;
            return View();
        }

        [HttpPost]
        public ActionResult New(int id, ProjectTask task)
        {
            Project currentProject = db.Projects.Find(id);
            if (currentProject.OwnerId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                try
                {
                    //ViewBag.ProjectId = db.Projects;
                    task.PostDate = DateTime.Now;
                    task.ProjectId = id;
                    task.Status = 0;
                    task.OwnerId = User.Identity.GetUserId();



                    Project prj = db.Projects.Find(id);
                    prj.Tasks.Add(task);

                    db.Tasks.Add(task);
                    db.SaveChanges();
                    return RedirectToAction("Show/" + Session["projectId"], "Project");
                }
                catch (Exception e)
                {
                    return View();
                }
            else
                return View();
        }


        public ActionResult Edit(int id)
        {
            ProjectTask task = db.Tasks.Find(id);
            return View(task);
        }

        [HttpPut]
        public ActionResult Edit(int id, ProjectTask requestTask)
        {
            ProjectTask currentProjectTask = db.Tasks.Find(id);
            Project currentProject = db.Projects.Find(currentProjectTask.ProjectId);
            if (currentProject.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                try
                {
                    ProjectTask task = db.Tasks.Find(id);
                    if (TryUpdateModel(task))
                    {
                        task.Name = requestTask.Name;
                        task.Description = requestTask.Description;
                        task.DeadLine = requestTask.DeadLine;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Show/" + Session["projectId"], "Project");
                }
                catch (Exception e)
                {
                    return View();
                }
            else
                return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            ProjectTask currentProjectTask = db.Tasks.Find(id);
            Project currentProject = db.Projects.Find(currentProjectTask.ProjectId);
            if (currentProject.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ProjectTask task = db.Tasks.Find(id);
                Project projTaskDelete = db.Projects.Find(task.ProjectId);
                projTaskDelete.Tasks.Remove(task);
                db.Tasks.Remove(task);
                db.SaveChanges();
                return RedirectToAction("Show/" + Session["projectId"], "Project");
            }
            else 
                return RedirectToAction("Show/" + Session["projectId"], "Project");
        }

        public ActionResult AssignBox(int id)
        {
            ProjectTask currentProjectTask = db.Tasks.Find(id);
            Project currentProject = db.Projects.Find(currentProjectTask.ProjectId);
            if (currentProject.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ViewBag.TaskID = id;
                if (TempData.ContainsKey("validEmail"))
                    ViewBag.invalidUser = TempData["validEmail"];
                else if (TempData.ContainsKey("invalidUser"))
                    ViewBag.invalidUser = TempData["invalidUser"];
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult Assign(int id,string Name)
        {
            ProjectTask taskToAssign = db.Tasks.Find(id);
            Project currentProject = db.Projects.Find(taskToAssign.ProjectId);
            Name = Name.Trim();
            if (IsValidEmailAddress(Name) == false)
                TempData["validEmail"] = "Not an email adress";
            foreach( ApplicationUser appUser in currentProject.TeamMembers)
            {
                
                if(appUser.UserName==Name)
                {
                    taskToAssign.AssignedMembers.Add(appUser);
                    db.SaveChanges();
                    return RedirectToAction("Show/" + Session["projectId"], "Project");
                }
            }
            TempData["invalidUser"] = "The user is not part of Team members";
            return RedirectToAction("AssignBox/" + id, "Task");
        }
    }
}