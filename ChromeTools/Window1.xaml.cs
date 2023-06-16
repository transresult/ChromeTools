using ChromeTool;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChromeTools
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private string _downloadedStoreFolder= CreateInitialStoreFolderForDownloadedProfiles();

        public string DownloadedStoreFolder
        {
            get => _downloadedStoreFolder;
            set
            {
                if (value != string.Empty)
                {
                    
                    _downloadedStoreFolder = value;
                }
            }
        }

        private string _chromeStoreFolder = GoogleChromeProfileRepository.localGoogleChromeUserProfileDirectory;

        public string ChromeStoreFolder
        {
            get => _chromeStoreFolder;
            set
            {
                if (value != string.Empty)
                {
                    _chromeStoreFolder = value;
                }
            }
        }

        public Window1()
        {
            InitializeComponent();
            
            DownloadedStoreFolderPathTextBox.Text = DownloadedStoreFolder;
            
        }

        private static string CreateInitialStoreFolderForDownloadedProfiles()
        {
            string homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

            if (!string.IsNullOrEmpty(homeDrive))
            {
                return System.IO.Path.Combine(homeDrive, "MyChromeProfiles");
            }
            else
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyChromeProfiles");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GoogleUserProfilePathTextBox.Text = GoogleChromeProfileRepository.localGoogleChromeUserProfileDirectory;
            WindowsChromeExePath.Text = GoogleChromeProfileRepository.chromeExePath;

        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadedStoreFolder = DownloadedStoreFolderPathTextBox.Text;
            ListBox downloadedProfileListBox = MainWindow.mainWindow.DownloadedProfileListBox;
            TextBlock statusText = MainWindow.mainWindow.statusText;

            MainWindow.RefreshSavedProfileListbox(downloadedProfileListBox, statusText, MainWindow.OptionsWindow);
            
            this.Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
