using Core.Models;
using System.Collections.Generic;

namespace Core.Repositories
{
    public class GoogleChromeProfileRepository
    {
        public IEnumerable<GoogleChromeProfileModel> ReadAllProfilesFrom(string path)
        {
            return new List<GoogleChromeProfileModel>();
        }
        public void SaveProfileTo(GoogleChromeProfileModel model, string path) {
        }
        public void DeleteProfileFrom(GoogleChromeProfileModel model, string path) { }



    }
}