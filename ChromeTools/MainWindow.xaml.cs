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
using System.Windows.Navigation;
using System.Xml.Linq;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using ChromeTools;
using Core.Repositories;
using Core.Models;
using Microsoft.VisualBasic;

namespace ChromeTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Window1 OptionsWindow = new Window1();
        public static MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        List<GoogleChromeProfileModel> SelectedProfiles = new List<GoogleChromeProfileModel>();


        public MainWindow()
        {
            InitializeComponent();
           
        }


        #region hinzufügen oder entfernen Items Buttons in Ausgewähltes Profile ListBox
        private void AddProfileToSelectedListbox_Click(object sender, RoutedEventArgs e)
        {
            //GoogleChromeProfileModel selectedProfile = DownloadedProfileListBox.SelectedItem as GoogleChromeProfileModel;

            //if (selectedProfile != null)
            //{
            //    SelectedProfileListBox.Items.Add(selectedProfile);
            //}
            GoogleChromeProfileModel selectedProfile = DownloadedProfileListBox.SelectedItem as GoogleChromeProfileModel;
           

            if (selectedProfile != null)
            {
                SelectedProfileListBox.ItemsSource = SelectedProfiles;
                SelectedProfiles.Add(selectedProfile);
                SelectedProfileListBox.Items.Refresh();
            }

        }

        private void RemoveProfileFromSelectedListbox_Click(object sender, RoutedEventArgs e)
        {
            //GoogleChromeProfileModel selectedProfile = SelectedProfileListBox.SelectedItem as GoogleChromeProfileModel;

            //if (selectedProfile != null)
            //{
            //    SelectedProfileListBox.Items.Remove(selectedProfile);
            //}
            GoogleChromeProfileModel selectedProfile = SelectedProfileListBox.SelectedItem as GoogleChromeProfileModel;

            if (selectedProfile != null)
            {
                SelectedProfileListBox.ItemsSource = SelectedProfiles;
                SelectedProfiles.Remove(selectedProfile);
                SelectedProfileListBox.Items.Refresh();
            }
        }

        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshActiveProfileListbox();
            RefreshSavedProfileListbox(DownloadedProfileListBox, statusText, OptionsWindow);
        }


        #region ToolBar Buttons
        private void CreateandSetProfil_Click(object sender, RoutedEventArgs e)
        {
            //List<GoogleChromeProfileModel> selectedProfiles = new List<GoogleChromeProfileModel>();

            //GoogleChromeProfileRepository.KillAllChromeProcess();
            //GetSelectedProfile();

            // TODO 
            // selectedProfiles = SelectedProfileListBox.ItemsSource;
            // GoogleChromeProfileRepository.ConstructNewGoogleProfileInCurrentUserprofile(selectedProfiles);
            // 
            GoogleChromeProfileRepository.KillAllChromeProcess();

            List<GoogleChromeProfileModel> selectedProfiles = SelectedProfileListBox.ItemsSource as List<GoogleChromeProfileModel>;

            if (selectedProfiles != null && selectedProfiles.Count > 0)
            {
                GoogleChromeProfileRepository.ConstructNewGoogleProfileInCurrentUserprofile(selectedProfiles);

                //GetSelectedProfile();
            }
            else
            {
                // Keine Profile ausgewählt
                statusText.Text = "Es wurden keine Profile ausgewählt.";
            }
            RefreshActiveProfileListbox();
        }
        private void DeleteAllProfiles_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Möchten Sie wirklich alle Chrome Profile löschen?", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                GoogleChromeProfileRepository.DeleteAllProfiles();
            }
        }

        #endregion



        #region SortUp und SortDown Buttons
        private void SortUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedProfileListBox.SelectedItems.Cast<GoogleChromeProfileModel>().ToList();

            if (selectedItems.Any())
            {
                var selectedIndex = SelectedProfileListBox.SelectedIndex;

                if (selectedIndex > 0)
                {
                    foreach (var selectedItem in selectedItems)
                    {
                        SelectedProfiles.Remove(selectedItem);
                    }

                    var insertIndex = selectedIndex - 1;
                    foreach (var selectedItem in selectedItems)
                    {
                        SelectedProfiles.Insert(insertIndex, selectedItem);
                        insertIndex++;
                    }

                    SelectedProfileListBox.ItemsSource = null;
                    SelectedProfileListBox.ItemsSource = SelectedProfiles;
                    SelectedProfileListBox.SelectedIndex = selectedIndex - 1;
                }
            }
        }
        private void SortDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedProfileListBox.SelectedItems.Cast<GoogleChromeProfileModel>().ToList();

            if (selectedItems.Any())
            {
                var selectedIndex = SelectedProfileListBox.SelectedIndex;
                var selectedProfilesCount = SelectedProfiles.Count;

                if (selectedIndex < selectedProfilesCount - selectedItems.Count)
                {
                    foreach (var selectedItem in selectedItems)
                    {
                        SelectedProfiles.Remove(selectedItem);
                    }

                    var insertIndex = selectedIndex + selectedItems.Count;
                    foreach (var selectedItem in selectedItems)
                    {
                        SelectedProfiles.Insert(insertIndex, selectedItem);
                        insertIndex++;
                    }

                    SelectedProfileListBox.ItemsSource = null;
                    SelectedProfileListBox.ItemsSource = SelectedProfiles;
                    SelectedProfileListBox.SelectedIndex = selectedIndex + selectedItems.Count;
                }
            }
        }
        #endregion

        #region hier sind alle Downloads Buttons
        private void DownloadAllProfiles_Click(object sender, RoutedEventArgs e)
        {
            string ChromeProfilePath = OptionsWindow.ChromeStoreFolder;
            List<GoogleChromeProfileModel> profiles = GoogleChromeProfileRepository.ReadAllProfilesFrom(ChromeProfilePath);
            string downloadedStoreFolderPath = OptionsWindow.DownloadedStoreFolder;

            foreach (GoogleChromeProfileModel profile in profiles)
            {
                string destinationPath = GoogleChromeProfileRepository.CopyProfiles(new List<GoogleChromeProfileModel> { profile }, downloadedStoreFolderPath);

                if (!string.IsNullOrEmpty(destinationPath))
                {
                    statusText.Text += destinationPath + "\n";
                }
                else
                {
                    statusText.Text = "Fehler beim Herunterladen der Profile.";
                }
            }
            RefreshSavedProfileListbox(DownloadedProfileListBox, statusText, OptionsWindow);
        }

        private void DownloadSelectedProfile_Click(object sender, RoutedEventArgs e)
        {
            string ChromeProfilePath = OptionsWindow.ChromeStoreFolder;
            GoogleChromeProfileModel selectedProfile = ActiveProfileListBox.SelectedItem as GoogleChromeProfileModel ;
            string downloadedStoreFolder = OptionsWindow.DownloadedStoreFolder;

            // Das ausgewählte Profil finden
            List<GoogleChromeProfileModel> profiles = GoogleChromeProfileRepository.ReadAllProfilesFrom(ChromeProfilePath);

            GoogleChromeProfileModel selectedProfileModel = profiles.FirstOrDefault(p => p.DisplayText.Equals(selectedProfile.DisplayText, StringComparison.OrdinalIgnoreCase));

            if (selectedProfileModel != null)
            {

                string copiedFilePath = GoogleChromeProfileRepository.CopyProfiles(new List<GoogleChromeProfileModel> { selectedProfileModel }, downloadedStoreFolder);

                if (!string.IsNullOrEmpty(copiedFilePath))
                {
                    statusText.Text = "Das Profil unter " + copiedFilePath + " gespeichert.";
                }
                else
                {
                    statusText.Text = "Fehler beim Herunterladen des Profils.";
                }
            }
            RefreshSavedProfileListbox(DownloadedProfileListBox, statusText, OptionsWindow);
        }
        #endregion


        #region Hier sind alle Buttons und Funktionen zum Aktualisieren aller Listboxen
        private void SavedProfileRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshSavedProfileListbox(DownloadedProfileListBox, statusText, OptionsWindow);
        }
        private void ActiveProfileRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshActiveProfileListbox();
        }
        private void SelectedProfileRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // SelectedProfileListBox.Items.Clear();
            SelectedProfiles.Clear();
            SelectedProfileListBox.Items.Refresh();
        }
      
        public static void RefreshSavedProfileListbox(ListBox downloadedProfileListBox, TextBlock statusText, Window1 OptionsWindow)
        {
            string downloadedStoreFolder = OptionsWindow.DownloadedStoreFolder;
            List<GoogleChromeProfileModel> savedProfilesInStore = GoogleChromeProfileRepository.ReadAllProfilesFrom(downloadedStoreFolder);
            downloadedProfileListBox.ItemsSource = savedProfilesInStore;
        }
        private void RefreshActiveProfileListbox()
        {
            string localGoogleChromeUserProfileDirectory = OptionsWindow.ChromeStoreFolder;
            ActiveProfileListBox.Items.Clear();
            List<GoogleChromeProfileModel> profiles = GoogleChromeProfileRepository.ReadAllProfilesFrom(localGoogleChromeUserProfileDirectory);

            foreach (GoogleChromeProfileModel profile in profiles)
            {

                ActiveProfileListBox.Items.Add(profile);
            }

        }

        #endregion


        #region hier sind alle Mainmenu Events enthalten

        private void MainMenu_Properties_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow.Show();
            RefreshSavedProfileListbox(DownloadedProfileListBox, statusText, OptionsWindow); // müssen wir machen, weil sich vielleiht das verzeichnis gheädnert hat
        }


    private void Darkmode_Click(object sender, RoutedEventArgs e)
    {
        ActiveProfileListBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3E3B3B"));
        ActiveProfileListBox.Foreground = Brushes.White;
        SelectedProfileListBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3E3B3B"));
        SelectedProfileListBox.Foreground = Brushes.White;
        DownloadedProfileListBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3E3B3B"));
        DownloadedProfileListBox.Foreground = Brushes.White;
    }
    private void LightMode_Click(object sender, RoutedEventArgs e)
    {
        ActiveProfileListBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
        ActiveProfileListBox.Foreground = Brushes.Black;
        SelectedProfileListBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
        SelectedProfileListBox.Foreground = Brushes.Black;
        DownloadedProfileListBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
        DownloadedProfileListBox.Foreground = Brushes.Black;

    }
    private void ExitButton(object sender, RoutedEventArgs e)
    {
       this.Close();
    }
        #endregion

    }

}

