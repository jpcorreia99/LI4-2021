using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PenedaVes.Data.FileManager
{
    public interface IFileManager
    {
        Task<string> SaveImage(IFormFile image);
        FileStream ImageStream(string image);
        void RemoveImage(string image);

    }
}