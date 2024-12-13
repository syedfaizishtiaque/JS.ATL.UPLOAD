using ATL_UPLOAD.Models;
using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL_UPLOAD.Repository
{
    public class serv_SFTP : IDisposable
    {
        public SFTPConfig SftpConfig { get; set; }
       
        public serv_SFTP()
        {
            try
            {
                var confiData = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
                SftpConfig = JsonConvert.DeserializeObject<SFTPConfig>(confiData);
            }
            catch (Exception ex)
            {
                 ErrorLog.WriteException(ex.ToString(), $"Contructor SFTP {ex.ToString()}");
            }
        }
        public async Task<bool> ReadSFTPConfigandProcess(string FileName)
        {
            bool result = true;
            try
            {
                string currentDateDir = string.Empty;
                string stream = string.Empty;
                foreach (var sourceDir in SftpConfig.Sources)
                {
                    if (sourceDir.SrcType == "local")
                    {
                        var destinationForLocalSrc = SftpConfig.Destinations.Where(s => s.SrcId == sourceDir.SrcId).FirstOrDefault();
                        foreach (var localFile in Directory.GetFiles(sourceDir.SrcFolder)
                                .Where(file => new FileInfo(file).Name == FileName+".csv"))
                        {


                            if (destinationForLocalSrc != null)
                            {
                                if (destinationForLocalSrc.DestType == "sftp")
                                {
                                    try
                                    {
                                        var sftpClientForUpload = await ConnectSftp(destinationForLocalSrc.DestEndPoint,
                                            22, destinationForLocalSrc.DestUsr,
                                                  destinationForLocalSrc.DestPwd);

                                        if (sftpClientForUpload != null)
                                        {

                                            await UploadFile(destinationForLocalSrc, localFile, sftpClientForUpload);
                                            stream += $"{Path.GetFileName(localFile)} | Uploaded on {destinationForLocalSrc.DestEndPoint} at {DateTime.Now}";
                                        }
                                        else
                                        {
                                            stream += $"{Path.GetFileName(localFile)} | Coudn't be Uploaded on {destinationForLocalSrc.DestEndPoint} at {DateTime.Now}";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        result = false;
                                         ErrorLog.WriteException(ex.ToString(), $"Sftp Upload from Local {Path.GetFileName(localFile)}");
                                    }

                                }
                            }
                            else
                            {
                                 ErrorLog.WriteException("", $"No Destination Found for {sourceDir.SrcEndPoint}");
                            }
                             //ErrorLog.WriteLog(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                 ErrorLog.WriteException($"Error Connecting {ex}", "ReadSFTPConfigandProcess");
            }
            return result;
        }
        private async Task UploadFile(Destination destination, string f, SftpClient sftpClient)
        {

            using (var fle = new FileStream(f, FileMode.Open))
            {
                try
                {
                    //string DestFolder = Path.Combine(destination.DestFolder, Program.DateFolder);
                    //await CreateDirSFTP(DestFolder, sftpClient);
                    FileInfo fi = new FileInfo(Path.Combine(f));
                    long size = fi.Length;
                    sftpClient.UploadFile(fle, Path.Combine(destination.DestFolder, Path.GetFileName(f)));
                    Console.WriteLine($"Upload file { Path.Combine(destination.DestFolder, Path.GetFileName(f))}");
                }
                catch (Exception ex)
                {
                     ErrorLog.WriteException(ex.ToString(), await Task.Run(() => $"UploadFile {f}"));
                    throw ex;
                }
            }
        }
        public async Task<SftpClient> ConnectSftp(string endPoint, int port, string user, string pwd)
        {
            try
            {
                var enc = new serv_EncDec();
                string username = await enc.DecryptString(user);
                string password = await enc.DecryptString(pwd);
                var sftpClient = new SftpClient(endPoint, port, await enc.DecryptString(user), await enc.DecryptString(pwd));
                sftpClient.Connect();
                if (sftpClient.IsConnected)
                {
                     ErrorLog.WriteLog($"Connected {endPoint} at {DateTime.Now}" ,"");
                    return sftpClient;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                ErrorLog.WriteException($"Error Connecting {ex}", "ConnectSftp");
                throw ex;
            }
        }
        public void Dispose()
        {
           // throw new NotImplementedException();
        }
    }
}
