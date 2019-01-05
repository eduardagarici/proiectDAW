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
        public int InviteId { get; set; }
        public string OrganizerId { get; set; }
        public string InvitedId { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey("OrganizerId")]
        public virtual ApplicationUser Organizer { get; set; }
        [ForeignKey("InvitedId")]
        public virtual ApplicationUser Invited { get; set; }
        [Required]
        [ForeignKey("ProjectId")]
        public virtual Project Project{ get; set; }
   
    }
}