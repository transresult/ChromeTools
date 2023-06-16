using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Profil
    {
        [JsonProperty("account_info")]
        public AccountInfo[] AccountInfo { get; set; }
        [JsonProperty("profile")]
        public ProfileInfo Profile { get; set; }
    }

    public class AccountInfo {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        
    }

    public class ProfileInfo
    {
        [JsonProperty("name")]
        public string   Name { get; set; }


    }
}
