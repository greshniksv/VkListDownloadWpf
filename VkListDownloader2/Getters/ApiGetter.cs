using System;
using System.Collections.Generic;
using VkListDownloader2.Getters;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkListDownloader2
{
    public class ApiGetter: IGetter
    {
        private readonly string _email;
        private readonly string _password;

        public ApiGetter(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public Dictionary<string, Uri> GetAudios()
        {
            var api = new VkApi();
            
            api.Authorize(new ApiAuthParams
            {
                ApplicationId = 5448335,
                Login = _email,
                Password = _password,
                Settings = Settings.Audio
            });

            User user;
            var records = api.Audio.Get(out user, new AudioGetParams());

            var result = new Dictionary<string, Uri>();
            foreach (var record in records)
            {
                string key = Tools.FixFileName($"{record.Artist} - {record.Title}");
                int adding = 0;
                while (result.ContainsKey(key))
                {
                    key = Tools.FixFileName($"{++adding}_{record.Artist} - {record.Title}");
                }
                result.Add(key, record.Url);
            }

            return result;
        }
    }
}
