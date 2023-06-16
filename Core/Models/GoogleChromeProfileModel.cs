using Core.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace Core.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class GoogleChromeProfileModel
    {

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Vorname { get; set; } = string.Empty;
        public string DisplayText { get; set; } = "invalid";
        public string Profilename { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = "https://lh3.googleusercontent.com/a/AAcHTtd0OXo6ShHitlOriLtLxXmJWPi9zpjvsWi1xNkmtw=s96-c";

        public string ProfileFilePath { get; set; } = string.Empty;

        public bool IsValid { get; set; } = false;



        [Obsolete("This property is obsolete. Use oogleChromeProfileModel(string path) instead.", false)]
        /// <summary>
        /// Default constructor
        /// </summary>
        public GoogleChromeProfileModel() { }

        /// <summary>
        /// bekommt einen kompletten Pfadmanem auf die preferencess datei und erstett darau sein komplettes profiledata objekt für uns 
        /// </summary>
        /// <param name="fullyQualifiedFilename"></param>
        //public GoogleChromeProfileModel(string fullyQualifiedFilename)
        //{
        //    ProfileFilePath = fullyQualifiedFilename;

        //    string fileContents = File.ReadAllText(fullyQualifiedFilename);

        //    JObject jsonObject = JObject.Parse(fileContents);
        //    JToken accountInfo = jsonObject["account_info"];
        //    JToken profileInfo = jsonObject["profile"];

        //    var result = JsonConvert.DeserializeObject<Profil>(fileContents);
        //    Console.WriteLine(result.Profile.Name);


        //    if (accountInfo != null && accountInfo.HasValues)
        //    {

        //        foreach (JToken token in accountInfo)
        //        {
        //            if (token["email"] == null)
        //            {
        //                IsValid = false;
        //                break;
        //            }

        //            Email = token["email"].Value<string>();

        //            // Use Value<string?>() to handle nullable values
        //            Name = token["full_name"]?.Value<string?>();
        //            Vorname = token["given_name"]?.Value<string?>();

        //            foreach (JToken subProfileToken in profileInfo)
        //            {
        //                //string? _profilname = subProfileToken["name"]?.Value<string?>();
        //                //if (_profilname != null)
        //                //{
        //                // Profilename = _profilname;

        //                string displayInfo = $"{Vorname} ({Name})";
        //                DisplayText = displayInfo; // DisplayText initialization
        //                IsValid = true;
        //                //}
        //            }
        //        }


        //    }

        //}


        public GoogleChromeProfileModel(string fullyQualifiedFilename)
        {
            ProfileFilePath = fullyQualifiedFilename;
            string fileContents = File.ReadAllText(fullyQualifiedFilename);

            var result = JsonConvert.DeserializeObject<Profil>(fileContents);

            if (result != null && result.AccountInfo != null && result.AccountInfo.Length > 0)
            {
                var accountInfo = result.AccountInfo[0];

                Email = accountInfo.Email;
                Name = accountInfo.FullName;
                Vorname = accountInfo.GivenName;
                PictureUrl= accountInfo.PictureUrl;

                Profilename = result.Profile.Name;
                    string displayInfo = $"{Vorname} ({Profilename})";
                    DisplayText = displayInfo;
                    IsValid = true;
               
            }
        }

    }
}