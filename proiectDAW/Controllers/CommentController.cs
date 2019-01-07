using Microsoft.AspNet.Identity;
using proiectDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiectDAW.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> UserManager;
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(int id)
        {
            Session["taskId"] = id;
            ProjectTask currentTask = db.Tasks.Find(id);
            ViewBag.Comments = currentTask.Comments;
            return View();
        }
        public ActionResult New()
        {
            //ViewBag.ProjectId = db.Projects;
            return View();
        }

        [HttpPost]
        public ActionResult New(int id, Comment comm)
        {

            ProjectTask currentTask = db.Tasks.Find(id);
            try
            {
                //ViewBag.ProjectId = db.Projects;
                comm.LastUpdateDate = DateTime.Now;
                comm.UserId = User.Identity.GetUserId();
                comm.TaskId = id;
                currentTask.Comments.Add(comm);
                db.Comments.Add(comm);
                db.SaveChanges();
                return RedirectToAction("Show/" + Session["taskId"], "Comment");
            }
            catch (Exception e)
            {
                return View();
            }
        }
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            return View(comm);
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComm)
        {
            // Comment currentProjectTask = db.Tasks.Find(id);
            try
            {
                Comment comm = db.Comments.Find(id);
                if (TryUpdateModel(comm))
                {
                    comm.Text = requestComm.Text;
                    comm.LastUpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
                return RedirectToAction("Show/" + Session["taskId"], "Comment");
            }
            catch (Exception e)
            {
                return View();
            }
         
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            ProjectTask projectTask = db.Tasks.Find(comm.TaskId);
                projectTask.Comments.Remove(comm);
                db.Comments.Remove(comm);
                db.SaveChanges();
                return RedirectToAction("Show/" + Session["taskId"], "Comment");
            
        }
    }
}