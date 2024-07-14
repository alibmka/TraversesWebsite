using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TraversesService.Interfaces;
using TraversesService.Utilities.Helpers;

namespace TraversesService
{
    public class FileService : IFileServices
    {

        public Task SaveFileAsync(string filename, string content, string filePath)
        {
            var temp = Directory.CreateDirectory("Download");
            string fullPath = Path.Combine(temp.FullName, filename);
            return File.WriteAllTextAsync(fullPath, content);
        }

        public void SaveFileFromUrlAsync(Uri address)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var tempPath = Path.Combine("Download", address.LocalPath.TrimStart('/').Replace("/", "\\"));
                    var tempDirectory = Path.GetDirectoryName(tempPath);

                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }
                    client.DownloadFileAsync(address, Path.Combine(Directory.GetCurrentDirectory(), tempPath));
                }
            }
            catch (Exception exp)
            {
                // Log Error Issues
            }
        }

    }
}

