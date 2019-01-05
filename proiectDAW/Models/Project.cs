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
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }
        public ProjectStatus Status { get; set;}
        public string OwnerId { get; set; }


        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<ApplicationUser> TeamMembers { get; set; }
        public virtual ICollection<ProjectTask> Tasks { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }
    }
}