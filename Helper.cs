using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite
{
    public class Helper
    {
        public static string ConnectionString { get; set; } = "";
        public static string PartnerNumber { get; set; } = "";

        public static async Task<List<string>> UploadAsync(List<IFormFile> files)
        {
            var ListResut = new List<string>();

            try
            {
                //upload nhiều file xài code này
                if (files.Count > 1)
                {
                    long size = files.Sum(f => f.Length);
                    var filePaths = new List<string>();
                    foreach (var formFile in files)
                    {
                        if (formFile.Length > 0)
                        {
                            // full path to file in temp location
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/_FileUpload/" + PartnerNumber);
                            filePaths.Add(filePath);
                            var fileNameWithPath = string.Concat(filePath, "\\", formFile.FileName);

                            if (Directory.Exists(fileNameWithPath))
                            {
                                Directory.Delete(fileNameWithPath);
                            }
                            else
                            {
                                Directory.CreateDirectory(filePath);
                            }

                            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                            await formFile.CopyToAsync(stream);

                            var webPath = "/_FileUpload/" + PartnerNumber + "/" + formFile.FileName;
                            ListResut.Add(webPath);
                        }
                    }
                }
                else
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/_FileUpload/" + PartnerNumber);
                    var fileNameWithPath = string.Concat(filePath, "\\", files[0].FileName);

                    if (Directory.Exists(fileNameWithPath))
                    {
                        Directory.Delete(fileNameWithPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                    await files[0].CopyToAsync(stream);

                    var webPath = "/_FileUpload/" + PartnerNumber + "/" + files[0].FileName;
                    ListResut.Add(webPath);
                }
            }
            catch (Exception ex)
            {

            }

            return ListResut;
        }
    }
}
