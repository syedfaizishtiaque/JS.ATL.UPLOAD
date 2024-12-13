using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ATL_UPLOAD.Repository
{
    public class serv_EncDec
    {
        private string key = string.Empty;
        private string yekcne = string.Empty;
        public serv_EncDec()
        {
            GK();
        }

        public async Task<string> EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array = null;

            using (Aes aes = Aes.Create())
                try
                {
                    {

                        aes.Key = Encoding.UTF8.GetBytes(key);
                        aes.IV = iv;

                        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                                {
                                    streamWriter.Write(plainText);
                                }
                                array = memoryStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.WriteException(ex.ToString(), "EncryptString");
                }

            return Convert.ToBase64String(array);
        }

        public async Task<string> DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
                try
                {
                    {
                        var _sftpConfig = new serv_SFTP().SftpConfig;

                        yekcne = await DecryptKey(_sftpConfig.Ek);
                        aes.Key = Encoding.UTF8.GetBytes(yekcne);
                        aes.IV = iv;
                        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (MemoryStream memoryStream = new MemoryStream(buffer))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                                {
                                    return streamReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.WriteException(ex.ToString(), "DecryptString");
                    return null;
                }
        }

        public async Task<string> DecryptKey(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
                try
                {
                    {

                        aes.Key = Encoding.UTF8.GetBytes(key);
                        aes.IV = iv;
                        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (MemoryStream memoryStream = new MemoryStream(buffer))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                                {
                                    return streamReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                   ErrorLog.WriteException(ex.ToString(), "DecryptKey");
                    return null;
                }
        }

        private async Task GK()
        {
            try
            {
                //var rkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OurSettings");
                var rkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\OurSettings");

                if (rkey != null)
                {
                    //var setKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OurSettings");
                    //setKey.SetValue("EncKey", "123456ABCDFDF");
                    //setKey.Close();
                    key = rkey.GetValue("aek").ToString();
                    //await servErrorLog.WriteLog(key);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteException(ex.ToString(), "GetKey");
            }
        }
    }
}
