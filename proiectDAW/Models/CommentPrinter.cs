using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proiectDAW.Models
{
    public class CommentPrinter
    {
        public int CommentId { get; set; }
        public string UserName { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string Text { get; set; }
    }
}