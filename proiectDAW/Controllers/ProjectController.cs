using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using proiectDAW.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiectDAW.Controllers
{
    public class ProjectController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        private UserManager<ApplicationUser> UserManager;
        public static int inviteAvailability = 7;

        // GET: Project
        [Authorize(Roles = ("Member,Organizer,Administrator"))]
        public ActionResult Index(string searchParam = "")
        {
            if (TempData.ContainsKey("edit"))
            {
                ViewBag.edit = TempData["edit"].ToString();
            }
            if (TempData.ContainsKey("delete"))
            {
                ViewBag.delete = TempData["delete"].ToString();
            }
            if (TempData.ContainsKey("access"))
            {
                ViewBag.delete = TempData["access"].ToString();
            }
            var id = User.Identity.GetUserId();
            if (User.IsInRole("Administrator"))
            {
                var projects = (from project in db.Projects
                                orderby project.Status
                                select project).ToList();
                if (searchParam != "")
                {
                    var projectList = projects.Where(e => e.Name.Contains(searchParam) || e.Owner.Email.Contains(searchParam));
                    return View(projectList.ToList());
                }
                return View(projects);
            }
            else
            {
                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = UserManager.FindById(User.Identity.GetUserId());
                var projects = user.Projects.ToList();
                if (searchParam != "")
                {
                    var projectList = projects.Where(e => e.Name.Contains(searchParam) || e.Owner.Email.Contains(searchParam));
                    return View(projectList.ToList());
                }
                return View(projects);
            }
        }
        [Authorize(Roles = "Member,Organizer,Administrator")]
        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Member,Organizer,Administrator")]
        [HttpPost]
        public ActionResult New(Project project, string Member1, string Member2, string Member3)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var oldUser = UserManager.FindById(User.Identity.GetUserId());
                    var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
                    var oldRoleName = db.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                    if (oldRoleName != "Administrator" && oldRoleName != "Organizer")
                    {
                        UserManager.RemoveFromRole(User.Identity.GetUserId(), oldRoleName);
                        UserManager.AddToRole(User.Identity.GetUserId(), "Organizer");
                    }
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    project.StartDate = DateTime.Now;
                    project.EndDate = DateTime.Now;
                    project.UserId = User.Identity.GetUserId();
                    project.Owner = user;
                    project.Status = Enums.ProjectStatus.Incompleted;
                    if (project.TeamMembers == null)
                        project.TeamMembers = new HashSet<ApplicationUser>();
                    project.TeamMembers.Add(user);
                    db.Projects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(project);
                }
            }
            catch (Exception e)
            {

                return Content(e.InnerException.Message);
            }
        }
        [Authorize(Roles = "Organizer,Administrator")]
        public ActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                return View(project);
            TempData["access"] = "You do not have permission to edit the selected project";
            return RedirectToAction("Index");
        }

        [HttpPut]
        [Authorize(Roles = "Organizer,Administrator")]
        public ActionResult Edit(int id, Project requestProject)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    Project project = db.Projects.Find(id);
                    if (TryUpdateModel(project))
                    {
                        project.Name = requestProject.Name;
                        project.Deadline = requestProject.Deadline;
                        project.Description = requestProject.Description;
                        project.Status = requestProject.Status;
                        db.SaveChanges();
                        TempData["edit"] = "Project " + project.Name + " was edited succesfully";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestProject);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Organizer,Administrator")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                try
                {

                    TempData["delete"] = "Project " + project.Name + " was successfully deleted";
                    var user = UserManager.FindById(project.UserId);
                    user.Projects.Remove(project);
                    db.Projects.Remove(project);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {

                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Member,Organizer,Administrator")]
        [HttpGet]
        public ActionResult Show(int id)
        {
            Session["projectName"] = db.Projects.Find(id).Name;
            Session["projectId"] = id;
            var proj = db.Tasks;
            var tasks = (from p in proj
                         where p.ProjectId == id
                         select p).ToList();
            return View(tasks);
        }

        [Authorize(Roles = "Organizer,Administrator")]
        public ActionResult ViewTeam(int id)
        {
            Project project = db.Projects.Find(id);
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ViewBag.ProjectId = id;
                if (TempData.ContainsKey("access"))
                    ViewBag.Access = TempData["access"];
                if (TempData.ContainsKey("email"))
                    ViewBag.Email = TempData["email"];
                if (TempData.ContainsKey("noUser"))
                    ViewBag.NoUser = TempData["noUser"];
                if (TempData.ContainsKey("success"))
                    ViewBag.Success = TempData["success"];
                if (TempData.ContainsKey("already"))
                    ViewBag.Already = TempData["already"];
                if (TempData.ContainsKey("alreadyInvited"))
                    ViewBag.AlreadyInvited = TempData["alreadyInvited"];
                return View(project.TeamMembers);
            }
            else
            {
                TempData["access"] = "You do not have permission to see the team project";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Organizer,Administrator")]
        [HttpDelete]
        [Route("Project/{projectId}/DeleteMember/{userId}")]
        public ActionResult DeleteMember(int projectId, string userId)
        {
            Project project = db.Projects.Find(projectId);
            try
            {
                if (project.UserId != User.Identity.GetUserId() && !User.IsInRole("Administrator"))
                {
                    TempData["access"] = "You are not allowed to delete members";
                    return RedirectToAction("ViewTeam", new { id = projectId });
                }
                UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = UserManager.FindById(userId);
                user.Projects.Remove(project);
                project.TeamMembers.Remove(user);
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("ViewTeam", new { id = projectId });
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Administrator")]
        [Route("Project/{projectId}/AddMember")]
        public ActionResult AddMember(int projectId, string member)
        {
            member = member.Trim();
            Project project = db.Projects.Find(projectId);
            try
            {
                if (project.UserId != User.Identity.GetUserId() && !User.IsInRole("Administrator"))
                {
                    TempData["access"] = "You are not allowed to add members";
                    return RedirectToAction("ViewTeam", new { id = projectId });
                }
                if (IsValidEmailAddress(member))
                {
                    UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    var invitedMember = UserManager.FindByEmail(member);
                    if (project.TeamMembers.Contains(invitedMember))
                    {
                        TempData["already"] = "The provided person is already a member in the project";
                        return RedirectToAction("ViewTeam", new { id = projectId });
                    }
                    foreach (var inv in project.Invites)
                    {
                        if (inv.Invited == invitedMember)
                        {
                            TempData["alreadyInvited"] = "The provided person is already invited in the project";
                            return RedirectToAction("ViewTeam", new { id = projectId });
                        }
                    }

                    if (invitedMember == null)
                    {
                        TempData["noUser"] = "There is no user associated with the provided email";
                        return RedirectToAction("ViewTeam", new { id = projectId });
                    }
                    Invite invite = new Invite();
                    invite.Availability = inviteAvailability;
                    invite.Date = DateTime.Now;
                    invite.InvitedId = invitedMember.Id;
                    invite.Invited = invitedMember;
                    invite.OrganizerId = User.Identity.GetUserId();
                    invite.Organizer = user;
                    invite.Project = project;
                    db.Invites.Add(invite);
                    invitedMember.Invites.Add(invite);
                    project.Invites.Add(invite);
                    db.SaveChanges();
                    TempData["success"] = "User succesfully invited";
                }
                else
                {
                    TempData["email"] = "Invalid email";
                    return RedirectToAction("ViewTeam", new { id = projectId });

                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("ViewTeam", new { id = projectId });
        }
        [NonAction]
        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }
    }
}

