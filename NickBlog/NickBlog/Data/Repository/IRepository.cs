using NickBlog.Models;
using NickBlog.Models.Comments;
using NickBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NickBlog.Data.Repository
{
    public interface IRepository
    {
        public Post GetPost(int id);

        public List<Post> GetAllPosts();

        //public IndexViewModel GetAllPosts(int pageNumber);

        public IndexViewModel GetAllPosts(int pageNumber, string category, string search);

        public void AddPost(Post post);

        public void UpdatePost(Post post);

        public void RemovePost(int id);

        public void AddSubComment(SubComment comment);

        public Task<bool> SaveChangesAsync();

        
    }
}
