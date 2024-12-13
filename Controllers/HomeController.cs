using ATL_UPLOAD.Models;
using ATL_UPLOAD.Repository;
using Microsoft.AspNetCore.Http.Internal;
using SessionMenu.Lib.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ATL_UPLOAD.Controllers
{
   [SessionAuth]
    public class HomeController : Controller
    {
        public HomeController()
        {
            var appSession = new AppSessionRepo();
           
           
        }
        public ActionResult Index()
        {
            try
            {
                if (AppSession.Session == null || AppSession.Session.SessionRepo.Count == 0)
                {
                    return RedirectToAction("UnAuthorized", "Search");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("UnAuthorized", "Search");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase fileUpload)
        {
            try
            {
                if (fileUpload == null)
                {
                    ViewBag.Message = "File is empty!";
                    return View("Index");
                }
                if (fileUpload.ContentType == "application/vnd.ms-excel" || fileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string fileName = fileUpload.FileName;
                    string rootPath = MvcApplication.destinationLocation;
                    string filePath = Path.Combine(rootPath, fileName);
                    fileUpload.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "Home.Post.UploadExcel");
            }
            
            return View("Index");


        }

        [HttpGet]
        public ActionResult ShowProcessedFile()
        {
            List<FileData> fileData = new List<FileData>();
            try
            {
                string processLocation = ConfigurationManager.AppSettings["ProcFileLocation"].ToString();
                string[] files = Directory.GetFiles(processLocation);
                if (files != null && files.Length > 0)
                {

                    foreach (var file in files)
                    {
                        FileData obj = new FileData();
                        obj.FileName = Path.GetFileNameWithoutExtension(file);
                        obj.FilePath = file;
                        if (obj.FileName.Length >= 8)
                        {
                            string last8Digits = obj.FileName.Substring(obj.FileName.Length - 8);
                            DateTime date = DateTime.ParseExact(last8Digits, "ddMMyyyy", null);
                            obj.FileDate = date.ToString("yyyy-MM-dd");
                        }
                        fileData.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "ShowProcessedFile");
            }
            
            var jsonresult = Json(data: fileData, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = 500000000;
            return jsonresult;
        }
        [HttpGet]
        public ActionResult ShowProcessedFileLog()
        {
            List<FileData> fileData = new List<FileData>();
            string processLocation = ConfigurationManager.AppSettings["logLocation"].ToString();
            try
            {
                string[] files = Directory.GetFiles(processLocation);
                if (files != null && files.Length > 0)
                {

                    foreach (var file in files)
                    {


                        string FileName = Path.GetFileNameWithoutExtension(file);
                        string FilePath = file;
                        if (FileName.Length >= 8)
                        {
                            string last8Digits = FileName.Substring(0 , 8);
                            DateTime date = DateTime.ParseExact(last8Digits, "ddMMyyyy", null);
                            string FileDate = date.ToString("yyyy-MM-dd");
                            if (FileDate == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                const Int32 BufferSize = 128;
                                using (var fileStream = System.IO.File.OpenRead(FilePath))
                                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                                {
                                    String line;
                                    while ((line = streamReader.ReadLine()) != null)
                                    {
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            FileData obj = new FileData();
                                            // Process line
                                            obj.FileName = FileName;
                                            obj.FileDate = FileDate;
                                            obj.ProcessLog = line;
                                            fileData.Add(obj);
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                //var jsonresult = Json(data: fileData, JsonRequestBehavior.AllowGet);
                //jsonresult.MaxJsonLength = 500000000;
                //return jsonresult;
            }
            catch (Exception ex)
            {
                fileData = new List<FileData>();
               ErrorLog.WriteException(ex.ToString() , "ShowProcessedFileLog");
            }
            var jsonresult = Json(data: fileData, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = 500000000;
            return jsonresult;
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFileByPath(string fileName)
        {
            fileName = fileName + ".csv";
            string processLocation = ConfigurationManager.AppSettings["ProcFileLocation"].ToString();
            string filePath = Path.Combine(processLocation, fileName);
            
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "DownloadFileByPath");
                throw ex;
            }
            return File(memory, GetContentType(filePath), fileName);
        }
        [HttpGet]
        public async Task<ActionResult> UploadFile(string file_name)
        {
            string stream = "File has been Uploaded Successfully";
            try
            {
                using (serv_SFTP sftp = new serv_SFTP())
                {
                    bool result = await sftp.ReadSFTPConfigandProcess(file_name);
                    if (!result)
                    {
                        stream = "Failed to Upload File";
                    }
                }
            }
            catch (Exception ex)
            {
                stream = "Failed to Upload File";
                ErrorLog.WriteException(ex.ToString(), "UploadFile");
            }
            var jsonresult = Json(data: stream, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = 500000000;
            return jsonresult;
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
 {
     { ".txt", "text/plain" },
     { ".pdf", "application/pdf" },
     { ".doc", "application/vnd.ms-word" },
     { ".docx", "application/vnd.ms-word" },
     { ".xls", "application/vnd.ms-excel" },
     { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
     { ".png", "image/png" },
     { ".jpg", "image/jpeg" },
     { ".jpeg", "image/jpeg" },
     { ".gif", "image/gif" },
     { ".csv", "text/csv" }
 };
        }
    }
}