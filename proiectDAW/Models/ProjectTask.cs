using proiectDAW.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace proiectDAW.Models
{
    [Table("Tasks")]
    public class ProjectTask
    {
        [Key]
        public int ProjectTaskId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime PostDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime DeadLine { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { set; get; }


        public virtual Project Project { get; set; }
        public virtual ICollection<ApplicationUser> AssignedMembers { get; set; }
        public virtual HashSet<Comment> Comments { get; set; }
    }
}