﻿using NickBlog.Models.Comments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NickBlog.Models
{
    public class Post
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Title { get; set; } = "";

        public string Body { get; set; } = "";

        public string Description { get; set; } = "";

        public string Tags { get; set; } = "";

        public string Category { get; set; } = "";

        public string Image { get; set; } = "";
        public DateTime Created { get; set; } = DateTime.Now;

        public List<MainComment> MainComments { get; set; }
    }
}
