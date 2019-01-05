using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace proiectDAW.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime LastUpdateDate { get; set; }
        [Required]
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual ProjectTask Task { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }
}