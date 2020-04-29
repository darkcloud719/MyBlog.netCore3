using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NickBlog.Data.FileManager
{
    public interface IFileManager
    {
        FileStream ImageStream(string image);
        public Task<string> SaveImage(IFormFile image);

        bool RemoveImage(string image);
    }
}
