using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PenedaVes.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private string _imagePath;

        public FileManager(IConfiguration config) // Ficheiro AppSettings.json
        {
            _imagePath = config["Path:Images"];
        }
        
        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {
                var savePath = Path.Combine(_imagePath);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                //Internet Explorer Error C:/User/Foo/image.jpg
                //var fileName = image.FileName;
                var fileName = $"img_{DateTime.Now:dd-MM-yyyy-HH-mm-ss}2.jpg";
                var filePath = Path.Combine(savePath, fileName);
                
                using var imageObject = await Image.LoadAsync(image.OpenReadStream());
                imageObject.Mutate(x => x.Resize(512, 512));
            
                //Encode here for quality
                var encoder = new JpegEncoder
                {
                    Quality = 22 //Use variable to set between 5-30 based on your requirements -> 30 best res
                };
                await imageObject.SaveAsync(filePath,encoder);
                    
                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "error";
            }
        }
        
        public void RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public FileStream ImageStream(string image)
        {
            return new(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

    }
}