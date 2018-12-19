using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FileUploadControl
{
    public class uploadfilerepo : UploadInterface
    {
        private IHostingEnvironment hostingEnvironment;
        public uploadfilerepo(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;


        }
        public async void uploadfilemultiple(IList<IFormFile> files)
        {
            long totalBytes = files.Sum(f => f.Length);
            foreach (IFormFile item in files)
            {
                string filename = item.FileName.Trim('"');
                filename = this.EnsureFileName(filename);
                byte[] buffer = new byte[16 * 1024];
                using (FileStream output = System.IO.File.Create(this.GetpathAndFileName(filename)))
                {
                    using (Stream input = item.OpenReadStream())
                    {
                       
                        int readBytes;
                        while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            await output.WriteAsync(buffer, 0, readBytes);
                            totalBytes += readBytes;
                        }
                    }
                }
            }
        }

        private string EnsureFileName(string filename)
        {
            if (filename.Contains("\\"));
            filename = filename.Substring(filename.LastIndexOf("\\") + 1);
            return filename;
        }

        private string GetpathAndFileName(string filename)
        {
            string path = this.hostingEnvironment.WebRootFileProvider + "\\wwwroot\\uploads\\" + filename;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path + filename;


        }
    }
}
