using System;

namespace Core.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class GoogleChromeProfileModel
    {

        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string ProfileFilePath { get; private set; } = String.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GoogleChromeProfileModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        public GoogleChromeProfileModel(string path)
        {
            ProfileFilePath = path;
        }
    }

}