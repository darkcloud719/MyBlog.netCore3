using Microsoft.EntityFrameworkCore;
using NickBlog.Helpers;
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
    public class Repository : IRepository
    {
        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        //public IndexViewModel GetAllPosts(int pageNumber)
        //{
        //    /*int pageSize = 5;
        //    return _ctx.Posts
        //        .Skip(pageSize * (pageNumber - 1))
        //        .Take(pageSize)
        //        .ToList();*/
        //    int pageSize = 5;
        //    int skipAmount = pageSize * (pageNumber - 1);
        //    int postsCount = _ctx.Posts.Count();

        //    return new IndexViewModel
        //    {
        //        PageNumber = pageNumber,
        //        NextPage = postsCount > skipAmount + pageSize,
        //        Posts = _ctx.Posts
        //            .Skip(skipAmount)
        //            .Take(pageSize)
        //            .ToList()
        //    };
        //}

        private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            /*List<int> pages = new List<int>();
            // range of 5
            // +2 from border  or -2 from right border
            int midPoint = pageNumber < 3 ? 3 : pageNumber > pageCount - 2 ? pageCount - 2 : pageNumber;
            
            for (int i = midPoint - 2; i <= midPoint + 2; i++)
            {
                pages.Add(i);
            }

            if (pages[0] != 1)
            {
                pages.Insert(0, 1);
                if (pages[1] - pages[0] > 1)
                {
                    pages.Insert(1, -1);
                }
            }

            if (pages[pages.Count - 1] != pageCount)
            {
                pages.Insert(pages.Count, pageCount);
                if (pages[pages.Count - 1] - pages[pages.Count - 2] > 1)
                {
                    pages.Insert(pages.Count -1, -1);
                }
            }

            return pages;*/

            if(pageCount <= 5)
            {
                for(int i = 1; i<= pageCount; i++)
                {
                    yield return i;
                }
            }
            else
            {
                int midPoint = pageNumber < 3 ? 3 : pageNumber > pageCount - 2 ? pageCount - 2 : pageNumber;

                int lowerBound = midPoint - 2;
                int upperBound = midPoint + 2;

                if(lowerBound != 1)
                {
                    yield return 1;
                    if (lowerBound - 1 > 1)
                        yield return -1;
                }

                for(int i = midPoint -2; i<=upperBound; i++)
                {
                    yield return i;
                }

                if(upperBound != pageCount)
                {
                    if (pageCount - upperBound > 1)
                        yield return -1;
                    yield return pageCount;

                }
            }

            
        }


        public IndexViewModel GetAllPosts(int pageNumber, string category, string search)
        {
            //return _ctx.Posts.Where(post => post.Category.ToLower().Equals(category.ToLower())).ToList();

            //Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            int pageSize = 2;
            int skipAmount = pageSize * (pageNumber - 1);

            var query = _ctx.Posts.AsNoTracking().AsEnumerable();
            //_ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //var query = _ctx.Posts
                    //.Skip(skipAmount)
                    //.Take(pageSize);

            if (!String.IsNullOrEmpty(category))
                query = query.Where(x => InCategory(x));

            if (!String.IsNullOrEmpty(search))
                //query = query.Where(x => x.Title.Contains(search) || x.Body.Contains(search) || x.Description.Contains(search));
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%") || EF.Functions.Like(x.Body, $"%{search}%") || EF.Functions.Like(x.Description, $"%{search}%"));
            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);
            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageHelper.PageNumbers(pageNumber,pageCount).ToList(),
                Category = category,
                Search = search,
                Posts = query.Skip(skipAmount).Take(pageSize).ToList()
            };
        }

        public Post GetPost(int id)
        {
            //return _ctx.Posts.FirstOrDefault(p => p.Id == id);

            return _ctx.Posts.Include(p => p.MainComments).ThenInclude(mc => mc.SubComments).FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if(await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }
    }
}
