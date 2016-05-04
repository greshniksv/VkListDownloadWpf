using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VkListDownloader2.Getters;

namespace VkListDownloader2
{
    public class MetadataGetter : IGetter
    {
        private string metadataFile;

        public MetadataGetter(string file)
        {
            metadataFile = file;
        }

        public Dictionary<string, Uri> GetAudios()
        {
            var result = new Dictionary<string, Uri>();
            string fileData;
            using (TextReader reader = new StreamReader(metadataFile, Encoding.UTF8))
            {
                fileData = reader.ReadToEnd();
            }

            int position = 0;
            while (position < fileData.Length)
            {
                var artist = Tools.FixFileName(GetBlock(fileData, ref position));
                var title = Tools.FixFileName(GetBlock(fileData, ref position));
                var url = GetBlock(fileData, ref position);
                var key = string.Concat(artist, " - ", title);
                if (!result.ContainsKey(key))
                {
                    result.Add(string.Concat(artist, " - ", title), new Uri(url.Replace("h=", "http://")));
                }
            }
            return result;
        }

        private string GetBlock(string data, ref int position)
        {
            string charCount = string.Empty;
            for (int i = position; i <= data.Length; i++)
            {
                if (data[i] != '|')
                {
                    charCount += data[i];
                }
                else
                {
                    position = i + 1;
                    break;
                }
            }
            var charCountInt = Int32.Parse(charCount);
            string result = data.Substring(position, charCountInt);
            position += charCountInt;
            return result;
        }
    }
}
