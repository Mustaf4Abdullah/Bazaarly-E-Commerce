using System;

namespace Bazaarly.Models
{
    public class Comments
    {
        public int CommentId { get; set; }
        public string UserName { get; set; } 
        public string Content { get; set; } 
        public DateTime DatePosted { get; set; }

        public int ProductId { get; set; }
    }
}
