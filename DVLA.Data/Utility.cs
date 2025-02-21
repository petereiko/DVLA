using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data
{
    public static class Utility
    {
        public static string Encrypt(string plainText, string password="Securityr&d1", string salt="HEFRA")
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            using (Aes aes = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cs))
                        {
                            writer.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText, string password = "Securityr&d1", string salt = "HEFRA")
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using (Aes aes = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static void ResizePicture(Stream sourcePath, string targetPath)
        {

            try
            {
                using (var source =Image.FromStream(sourcePath))
                {
                    int newWidth = 207;
                    int newHeight = 230;

                    var len = sourcePath.Length / 1024 / 1024;

                    int pixSize = Image.GetPixelFormatSize(source.PixelFormat) / 8;
                    var msize = source.Width * source.Height / pixSize / 1024;
                    if (msize > 100 || (source.Height > 150 && source.Width > 300))
                    {
                        Image thumbnail = source.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
                        //var thumbGraph = Graphics.FromImage(thumbnail);
                        //thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                        //thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                        //thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                        //thumbGraph.DrawImage(thumbnail, imageRectangle);
                        thumbnail.Save(targetPath, ImageFormat.Png);
                    }
                    else
                    {
                        source.Save(targetPath, ImageFormat.Png);
                    }



                }



            }
            catch (Exception ex)
            {

            }
        }

    }
}
