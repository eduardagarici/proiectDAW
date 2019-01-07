using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace proiectDAW.Models
{
    public class Invite
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InviteId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        public int Availability { get; set; }
        [Required]
        public string OrganizerId { get; set; }
        public string InvitedId { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("OrganizerId")]
        public virtual ApplicationUser Organizer { get; set; }
        [ForeignKey("InvitedId")]
        public virtual ApplicationUser Invited { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project{ get; set; }
   
    }
}