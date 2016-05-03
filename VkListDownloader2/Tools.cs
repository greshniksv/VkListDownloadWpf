using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VkListDownloader2
{
    public static class Tools
    {
        public static string GetMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string FixFileName(string name)
        {
            const string correctChars =
                "123456789qwertyuiopasdfghjklzxcvbnm,.'!~@#$%&*()-_=+`йцукенгшщзхъфывапролджэячсмитьбюё ";
            var result = string.Empty;
            for (int i = 0; i < name.Length; i++)
            {
                if (correctChars.Any(x => x == name.ToLower()[i]))
                {
                    result += name[i];
                }
                else
                {
                    //result += ".";
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "Unknown";
            }
            return result.Length < 40 ? result : result.Substring(0, 40);
        }

        public static bool TestDir(string dir)
        {
            string testFile = Path.Combine(dir, Guid.NewGuid().ToString());
            try
            {
                File.CreateText(testFile).Close();
                File.Delete(testFile);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }

}
