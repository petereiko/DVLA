using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared
{
    public static class Utility
    {
        //public static string Encrypt(string plainText, string password="Securityr&d1", string salt="HEFRA")
        //{
        //    byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        //    using (Aes aes = Aes.Create())
        //    {
        //        var key = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
        //        aes.Key = key.GetBytes(aes.KeySize / 8);
        //        aes.IV = key.GetBytes(aes.BlockSize / 8);

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter writer = new StreamWriter(cs))
        //                {
        //                    writer.Write(plainText);
        //                }
        //            }
        //            return Convert.ToBase64String(ms.ToArray());
        //        }
        //    }
        //}

        //public static string Decrypt(string encryptedText, string password = "Securityr&d1", string salt = "HEFRA")
        //{
        //    byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        //    byte[] cipherBytes = Convert.FromBase64String(encryptedText);
        //    using (Aes aes = Aes.Create())
        //    {
        //        var key = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
        //        aes.Key = key.GetBytes(aes.KeySize / 8);
        //        aes.IV = key.GetBytes(aes.BlockSize / 8);

        //        using (MemoryStream ms = new MemoryStream(cipherBytes))
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
        //            {
        //                using (StreamReader reader = new StreamReader(cs))
        //                {
        //                    return reader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //}

        public static async Task<string> ConvertImageUrlToBase64(string imageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(imageUrl);
                    response.EnsureSuccessStatusCode();

                    var contentType = response.Content.Headers.ContentType.MediaType;
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    string base64String = Convert.ToBase64String(imageBytes);

                    return $"data:{contentType};base64,{base64String}";
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private const int urlIdEcodeSalt = 13411;
        public static string EncryptUrlID(int id, int salt = 0)
        {
            salt = salt == 0 ? urlIdEcodeSalt : salt;
            return (id * salt).ToString();
        }

        public static int DecryptUrlID(string encryptedID, int salt = 0)
        {
            salt = salt == 0 ? urlIdEcodeSalt : salt;
            return (Convert.ToInt32(encryptedID) / salt);
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


        public static string GetSafeString(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public static int? GetSafeInt32(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }
        public static byte? GetSafeByte(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (byte?)null : reader.GetByte(ordinal);
        }

        public static long? GetSafeInt64(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (long?)null : reader.GetInt64(ordinal);
        }
        public static bool GetSafeBoolean(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? false : reader.GetBoolean(ordinal);
        }

        public static DateTime? GetSafeDateTime(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        public static decimal? GetSafeDecimal(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }
        public static double? GetSafeDouble(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (double?)null : reader.GetDouble(ordinal);
        }

        public static Guid? GetSafeGuid(this DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (Guid?)null : reader.GetGuid(ordinal);
        }

    }
}
