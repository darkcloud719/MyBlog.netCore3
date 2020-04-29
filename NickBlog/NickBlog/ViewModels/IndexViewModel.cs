using NickBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NickBlog.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public string Search { get; set; }
        public string Category { get; set; }
        public IEnumerable<Post> Posts { get; set; }

        public IEnumerable<int> Pages { get; set; }

    }
}
