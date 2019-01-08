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

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            if (currentTask.AssignedMembers.Contains(currentUser) || User.IsInRole("Administrator") )
            {
                List<CommentPrinter> printer = new List<CommentPrinter>();
                foreach (Comment comm in currentTask.Comments)
                {
                    CommentPrinter obj = new CommentPrinter();
                    obj.CommentId = comm.CommentId;
                    obj.LastUpdateDate = comm.LastUpdateDate;
                    obj.Text = comm.Text;
                    ApplicationUser appUser = db.Users.Find(comm.UserId);
                    obj.UserName = appUser.UserName;
                    printer.Add(obj);
                }
                ViewBag.Comments = printer;

                if (TempData.ContainsKey("noPerm"))
                    ViewBag.noPermission = TempData["noPerm"];
                return View();
            }
            else
                return RedirectToAction("Show/" + Session["projectId"], "Project");

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
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                return View(comm);
            else
            {
                TempData["noPerm"] = "No permission to modify other member's comment";
                return RedirectToAction("Show/" + Session["taskId"], "Comment");
            }
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
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                projectTask.Comments.Remove(comm);
                db.Comments.Remove(comm);
                db.SaveChanges();
                return RedirectToAction("Show/" + Session["taskId"], "Comment");
            }
            else
            {
                TempData["noPerm"] = "No permission to delete other member's comment";
                return RedirectToAction("Show/" + Session["taskId"], "Comment");
            }
        }
    }
}