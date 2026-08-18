using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DVLA.Data.Models.Enumerables;

namespace DVLA.Data
{
    public static class Utility
    {
        private static readonly IConfigurationRoot _configuration;
        static Utility()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set path to the appsettings.json file
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static PassResult? GetPassResult(string passType)
        {
            return passType == "UNLIMITED" ? PassResult.Unlimited :
                passType == "LIMITED FOR 3 MONTHS" ? PassResult.ThreeMonths : PassResult.SixMonths;
        }


        public static DateTime? GetExpiryDate(PassResult? passResult)
        {
            var now = DateTime.UtcNow;
            DateTime? expiryDate = null;

            switch (passResult)
            {
                case PassResult.ThreeMonths:
                    expiryDate = now.AddMonths(3);
                    break;
                case PassResult.SixMonths:
                    expiryDate = now.AddMonths(6);
                    break;
                case PassResult.Unlimited:
                    expiryDate = now.AddYears(2);
                    break;
                default: break;
            }

            return expiryDate;
        }

        public static byte[] ExportToExcel<T>(List<T> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                var dataTable = ConvertToDataTable(data);
                worksheet.Cell(1, 1).InsertTable(dataTable);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private static DataTable ConvertToDataTable<T>(List<T> data)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }


        public static bool ValidatePassport(IFormFile passportData)
        {
            int size = Convert.ToInt32(_configuration["AppConstants:PassportMaxSize"]);
            return passportData.Length < 1024 * size;
        }


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

        public static DateTime StartOfDay(DateTime startDate)
        {
            DateTime date = startDate.Date;
            return date;
        }

        public static DateTime EndOfDay(DateTime endDate)
        {
            DateTime date = endDate.Date;
            date = date.AddHours(23).AddMinutes(59).AddSeconds(59);
            return date;
        }

    }
}
