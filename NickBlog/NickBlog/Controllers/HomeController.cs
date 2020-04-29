using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NickBlog.Data;
using NickBlog.Data.FileManager;
using NickBlog.Data.Repository;
using NickBlog.Models;
using NickBlog.Models.Comments;
using NickBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NickBlog.Controllers
{
    public class HomeController : Controller
    {
        //test
        //private AppDbContext _ctx;
        private IConfiguration _cfg;
        private IRepository _repo;
        private IFileManager _fileManager;

        public HomeController(IConfiguration cfg, IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _cfg = cfg;
            _fileManager = fileManager;

            var AllowedHosts = _cfg.GetValue<string>("AllowedHosts");
        }
        public IActionResult Index(int pageNumber, string category,string search)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, category });

            var vm = _repo.GetAllPosts(pageNumber, category, search);

            return View(vm);
            //if (pageNumber < 1)
            //    return RedirectToAction("Index", new { pageNumber = 1, category });

            //var vm = new IndexViewModel
            //{
            //    PageNumber = pageNumber,
            //    Posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts(pageNumber) : _repo.GetAllPosts(category)
            //};

            //return View(vm);
            //return View(string.IsNullOrEmpty(category) ? _repo.GetAllPosts(pageNumber) : _repo.GetAllPosts(category));

            //var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);

            //return View(posts);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);

            return View(post);
        }
        
        [HttpGet("/image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);

            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Post", new { id = vm.PostId});

            var post = _repo.GetPost(vm.PostId);
            if(vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                });

                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = vm.PostId });
        }
    }
}
