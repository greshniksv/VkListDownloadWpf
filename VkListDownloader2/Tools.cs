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
            string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            name = name.Length < 50 ? name : name.Substring(0, 50);
            return invalid.Aggregate(name, (current, c) => current.Replace(c.ToString(), ""));
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
