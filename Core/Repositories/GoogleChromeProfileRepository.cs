using Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.Linq;
using System.Threading;
using Core.Repositories;
using System.Linq.Expressions;

namespace Core.Repositories
{
    public class GoogleChromeProfileRepository
    {
        private static string WindowsUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string localGoogleChromeUserProfileDirectory = Path.Combine(WindowsUserProfile, @"AppData\Local\Google\Chrome\User Data\");
        private static string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string chromeUserDataPath = Path.Combine(localApplicationData, "Google", "Chrome", "User Data");
        public static string chromeExePath = CreateGooglChromeExecutablePathFromRegistriy();


    
        private static string[] GetProfilePathOfCurrentUserProfile(string path)
        {
            string searchPattern = "Preferences";
            
            DirectoryInfo object2 = new DirectoryInfo(path);
            string[] subFolders = object2.EnumerateDirectories().OrderBy(d => (d.CreationTime)).ThenBy(d => (d.Name)).Select(d => (d.FullName)).ToArray();

            
            string[] subFolders2 = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            List<string> relevantSubfolders = new List<string>();

            foreach (string subFolder in subFolders)
            {
                string preferencesFile = Path.Combine(subFolder,searchPattern);

                if (File.Exists(preferencesFile))
                {
                    relevantSubfolders.Add(preferencesFile);
                }
            }

            return relevantSubfolders.ToArray();
        }

        public static List<GoogleChromeProfileModel> ReadAllProfilesFrom(string path)
        {
            List<GoogleChromeProfileModel> result = new List<GoogleChromeProfileModel>();

            try
            { 
            string[] relevantSubfolders = GetProfilePathOfCurrentUserProfile(path);

            foreach (string currentFullyQualifiedPathToPreferencesFile in relevantSubfolders)
            {
               
                    GoogleChromeProfileModel currentItem = new GoogleChromeProfileModel(currentFullyQualifiedPathToPreferencesFile);

                    if (currentItem.IsValid)
                    {
                        result.Add(currentItem);
                    }
              
            }
            }
            catch 
            {
                //Fehlerbehebund wenn MyChromeProfiles nicht existiert
            }
            return result;
        }











        public static string CreateGooglChromeExecutablePathFromRegistriy()
        {
            const string chromeRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(chromeRegKey))
            {
                if (key != null)
                {
                    object value = key.GetValue(null);
                    if (value != null && value is string chromeExePath)
                    {
                        return chromeExePath;
                    }
                }
            }
            throw new Exception("Chrome-Anwendungspfad konnte nicht gefunden werden.");
        }
        
        #region Alle Profile löschen Funktion
        public static void DeleteAllProfiles()
        {

            try
            {
                // Überprüfe, ob das User Data-Verzeichnis existiert
                if (Directory.Exists(chromeUserDataPath))
                {
                    // Durchsuche alle Unterverzeichnisse im User Data-Verzeichnis
                    foreach (string profileDir in Directory.GetDirectories(chromeUserDataPath))
                    {
                        // Lösche das Profilverzeichnis
                        Directory.Delete(profileDir, true);
                    }

                    //MessageBox.Show("Alle Chrome-Profile wurden erfolgreich gelöscht.");
                }
                else
                {
                    MessageBox.Show("Das Chrome User Data-Verzeichnis wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Löschen der Chrome-Profile: {ex.Message}");
            }

            try
            {
                // Überprüfen, ob das Chrome-Benutzerdatenverzeichnis vorhanden ist
                if (Directory.Exists(chromeUserDataPath))
                {
                    // Beenden von Chrome, falls es läuft
                    KillChromeProcesses();

                    // Umbenennen des Chrome-Benutzerdatenverzeichnisses
                    string renamedPath = chromeUserDataPath + "_old";
                    Directory.Move(chromeUserDataPath, renamedPath);

                    // Starten von Chrome, um ein neues Benutzerdatenverzeichnis zu erstellen
                    Process.Start(chromeExePath);

                    // Warten auf das Starten von Chrome und Beenden der neuen Chrome-Prozesse
                    WaitForChromeStartup();
                    KillChromeProcesses();

                    // Löschen des umbenannten Benutzerdatenverzeichnisses
                    Directory.Delete(renamedPath, true);

                    MessageBox.Show("Chrome-Benutzerdaten wurden erfolgreich gelöscht.");
                }
                else
                {
                    MessageBox.Show("Das Chrome-Benutzerdatenverzeichnis wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim löschen der Chrome-Benutzerdaten: {ex.Message}");
            }

        }
        static void KillChromeProcesses()
        {
            foreach (Process process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
                process.WaitForExit();
            }
        }
        static void WaitForChromeStartup()
        {
            while (Process.GetProcessesByName("chrome").Length == 0)
            {
                // Warten auf das Starten von Chrome
                System.Threading.Thread.Sleep(100);
            }
        }
        #endregion

        #region Kopieren Funktion
        public static string CopyProfiles(List<GoogleChromeProfileModel> profiles, string copyToThisPath)
        {
            string? copiedFilePath = null;

            foreach (GoogleChromeProfileModel profile in profiles)
            {
                string destinationFilePath = CopyProfile(profile, copyToThisPath);
                copiedFilePath = destinationFilePath;
            }

            return copiedFilePath;

          
        }

        /// <summary>
        /// erstellt ein neues Verzeichis "profile-email" und kopiert dort die prefrences datei rein
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="downloadedStoreFolderPath"></param>
        /// <returns></returns>

        private static string CopyProfile(GoogleChromeProfileModel profile, string copyToThisPath)
        {
            string sourceProfileFolderName = Path.Combine(copyToThisPath, profile.Email);
            
            //erstellt eine Ordner und benennt wie Email
            Directory.CreateDirectory(sourceProfileFolderName);

            // Ziel-Pfad für die heruntergeladene Datei erstellen
            string fileName = Path.GetFileName(profile.ProfileFilePath);
            string destinationFilePath = Path.Combine(sourceProfileFolderName, fileName);

            // Datei kopieren
            File.Copy(profile.ProfileFilePath, destinationFilePath, true);

            return destinationFilePath;
        }
        #endregion

        public static void KillAllChromeProcess()
        {
            foreach (Process process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }
        }


        #region Profile erstellen Funktion
        public static void CreateCleanChromoProfileinInCurrentUserprofile(string folderName)
        {
            Process.Start(chromeExePath, $"--profile-directory=\"{folderName}\"");
        }

        public static void ConstructNewGoogleProfileInCurrentUserprofile(List<GoogleChromeProfileModel> profilesToBeCreated)
        {
            foreach (GoogleChromeProfileModel profile in profilesToBeCreated)
            {
                string subFoldername = profile.Email;

                CreateCleanChromoProfileinInCurrentUserprofile(subFoldername);
                CopyProfile(profile, chromeUserDataPath);
            }
        }
        #endregion
    }
}