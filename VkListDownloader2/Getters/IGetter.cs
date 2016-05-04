using System;
using System.Collections.Generic;

namespace VkListDownloader2.Getters
{
    interface IGetter
    {
        Dictionary<string, Uri> GetAudios();
    }
}
