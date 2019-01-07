using proiectDAW.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace proiectDAW.Models
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        [MaxLength(128)]
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }
        public ProjectStatus Status { get; set;}
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { set; get; }
        public virtual HashSet<ApplicationUser> TeamMembers { get; set; }
        public virtual HashSet<ProjectTask> Tasks { get; set; }
        public virtual HashSet<Invite> Invites { get; set; }
    }
}