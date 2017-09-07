/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General internal License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General internal License for more details. 
 * 
 * You should have received a copy of the GNU General internal License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;

namespace Robin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow
    {
        MainWindowViewModel MWVM = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MWVM;
            ListViewScale.ScaleX = .8;
            ListViewScale.ScaleY = .8;
            Activate();
#if DEBUG
            Button BonusButton = new Button();
            BonusButton.Content = "BONUS!";
            BonusButton.Width = 50;
            BonusButton.Height = 30;
            BonusButton.Click += BonusButton_Click;
            DockPanel.SetDock(BonusButton, Dock.Right);
            SearchBox_DockPanel.Children.Add(BonusButton);
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
#endif
        }
#if DEBUG
        private async void BonusButton_Click(object sender, RoutedEventArgs e)
        {
            Reporter.Report("BONUS!");

            await Task.Run(() =>
            {
            });
        }
#endif
        private void MainList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                ListViewScale.CenterX = Mouse.GetPosition(MainList).X;
                ListViewScale.CenterY = Mouse.GetPosition(MainList).Y;

                if (ListViewScale.ScaleX + .1 * Math.Sign(e.Delta) >= 0.4 &&
                    ListViewScale.ScaleX + .1 * Math.Sign(e.Delta) < +3)
                {
                    ListViewScale.ScaleX += .1 * Math.Sign(e.Delta);
                    ListViewScale.ScaleY += .1 * Math.Sign(e.Delta);

                }

                else
                {
                    ListViewScale.ScaleX += 0;
                    ListViewScale.ScaleY += 0;
                }
            }
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {

            var DatabaseWindow = new DatabaseWindow();
            DatabaseWindow.Show();
            DatabaseWindow.Activate();
        }

        private void MainListPlatform_Drop(object sender, DragEventArgs e)
        {
            Platform platform = (sender as StackPanel).DataContext as Platform;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                platform.GetReleaseDirectoryAsync((string[])e.Data.GetData(DataFormats.FileDrop, false));
            }
        }

        private void EmulatorStackPanel_Drop(object sender, DragEventArgs e)
        {
            Emulator emulator = (sender as StackPanel).DataContext as Emulator;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePath = e.Data.GetData(DataFormats.FileDrop, false) as string[];
                if (filePath != null)
                {
                    emulator.Add(filePath[0]);
                }
                else
                {
                    Reporter.Warn("Something is wrong with the file path you dropped.");
                }


            }
        }

        private void SaveDatabase_Click(object sender, RoutedEventArgs e)
        {
            MWVM.Save();
        }

        private void MainList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainList.UnselectAll();
            if (e.ClickCount > 1)
            {

            }
        }

        private void ReleasesButton_Click(object sender, RoutedEventArgs e)
        {
            GamesButton.IsChecked = false;
            PlatformsButton.IsChecked = false;
            EmulatorsButton.IsChecked = false;

            MainList.DataContext = MWVM.ReleaseCollection;
        }

        private void GamesButton_Click(object sender, RoutedEventArgs e)
        {
            ReleasesButton.IsChecked = false;
            PlatformsButton.IsChecked = false;
            EmulatorsButton.IsChecked = false;

            MainList.DataContext = MWVM.GameCollection;
        }

        private void PlatformsButton_Click(object sender, RoutedEventArgs e)
        {
            ReleasesButton.IsChecked = false;
            GamesButton.IsChecked = false;
            EmulatorsButton.IsChecked = false;

            MainList.DataContext = MWVM.PlatformCollection;
        }

        private void EmulatorsButton_Click(object sender, RoutedEventArgs e)
        {
            ReleasesButton.IsChecked = false;
            GamesButton.IsChecked = false;
            PlatformsButton.IsChecked = false;

            MainList.DataContext = MWVM.EmulatorCollection;
        }

        private void CreateThumbNail_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Stopwatch Watch = new Stopwatch();
            Watch.Start();
            int i = 0;
#endif

            foreach (Release release in R.Data.Releases)
            {
#if DEBUG
                Watch.Restart();
                Debug.WriteLine(i++ + " : " + Watch.ElapsedMilliseconds);
#endif
                release.CreateThumbnail();
            }
        }

        private void ConvertLynx_Click(object sender, RoutedEventArgs e)
        {
            //Handy.ConvertDirectory();
        }


        private void MainListWrapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is VirtualizingWrapPanel panel && MWVM.SelectedDBs != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(panel, MWVM.SelectedDBs, DragDropEffects.Copy);
            }
        }

        private void CollectionStackPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainList.DataContext = MWVM.SelectedCollection;
        }

        private void CollectionStackPanel_Drop(object sender, DragEventArgs e)
        {
            if ((e.OriginalSource as Border).DataContext is Collection collection)
            {
                for (int i = 0; i < MWVM.SelectedDBs.Count; i++)
                {
                    IDBobject idbObject = MWVM.SelectedDBs[i] as IDBobject;
                    collection.Add(idbObject);
                }

            }
        }

        private void Main_Window_Closing(object sender, CancelEventArgs e)
        {
            MWVM.Save();
        }

        private void About_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
        }


        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = MainList.SelectedItem != null && (MainList.SelectedItem as Game).Included;
            e.CanExecute = MWVM.SelectedDB != null && MWVM.SelectedDB.Included;
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainList.SelectedItem != null)
            {
                (MainList.SelectedItem as Game).Play(null);
            }
        }

        private void GetAllArt_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList.DataContext != null && (MainList.DataContext == MWVM.ReleaseCollection || MainList.DataContext == MWVM.GameCollection || MainList.DataContext == MWVM.PlatformCollection);
        }

        private void GetAllArt_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainList.DataContext == MWVM.ReleaseCollection || MainList.DataContext == MWVM.GameCollection)
            {
                MWVM.GetAllReleaseArt();
            }

            if (MainList.DataContext == MWVM.PlatformCollection)
            {
                MWVM.GetAllPlatformArt();
            }
        }

        private void GetAllData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList.DataContext != null && (MainList.DataContext == MWVM.ReleaseCollection || MainList.DataContext == MWVM.GameCollection);
        }

        private void GetAllData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.GetAllReleaseData();
        }


        private void GetSelectedArt_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MWVM.SelectedDB != null;
        }

        private void GetSelectedArt_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MWVM.GetSelectedArt();
        }

        private void MarkAsCrap_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = MainList.SelectedItem != null && !(MainList.SelectedItem as IDBobject).IsCrap;
        }

        private void MarkAsCrap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.MarkCrap(true);
        }

        private void MarkNotCrap_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList.SelectedItem != null && (MainList.SelectedItem as IDBobject).IsCrap;
        }

        private void MarkNotCrap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.MarkCrap(false);
        }


        private void MarkAsPreferred_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = e.OriginalSource as Control;
            e.CanExecute = MainList.SelectedItem != null && (control.DataContext as Release) != null && !(control.DataContext as Release).Preferred;
        }

        private void MarkAsPreferred_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var control = e.OriginalSource as Control;
            Release release = control.DataContext as Release;
            Game game = release.Game;

            foreach (Release _release in game.Releases)
            {
                _release.Preferred = false;
            }
            release.Preferred = true;
        }

        private void MarkNotPreferred_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = e.OriginalSource as Control;
            e.CanExecute = MainList.SelectedItem != null && (control.DataContext as Release) != null && (control.DataContext as Release).Preferred;
        }

        private void MarkNotPreferred_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var control = e.OriginalSource as Control;
            Release release = control.DataContext as Release;
            Game game = release.Game;

            foreach (Release _release in game.Releases)
            {
                _release.Preferred = true;
            }
            release.Preferred = false;
        }


        private void MarkEmulatorPreferred_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = e.OriginalSource as Control;
            e.CanExecute = false;
            if (MainList.SelectedItem as Emulator != null)
            {
                e.CanExecute = control.DataContext as Platform != null;
            }

            if (MainList.SelectedItem as Platform != null)
            {
                e.CanExecute = control.DataContext as Emulator != null;
            }

        }

        private void MarkEmulatorPreferred_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var control = e.OriginalSource as Control;
            Emulator emulator = control.DataContext as Emulator ?? MainList.SelectedItem as Emulator;
            Platform platform = MainList.SelectedItem as Platform ?? control.DataContext as Platform;

            emulator.MarkPreferred(platform);
        }


        private void MarkAsGame_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                MainList.SelectedItem != null &&
                (
                ((MainList.SelectedItem as Game != null) && !(MainList.SelectedItem as Game).IsGame)
                ||
                ((MainList.SelectedItem as Release != null) && !(MainList.SelectedItem as Release).IsGame)
                );
        }

        private void MarkAsGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.MarkGame(true);
        }

        private void MarkNotGame_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                MainList.SelectedItem != null &&
                (
                ((MainList.SelectedItem as Game != null) && (MainList.SelectedItem as Game).IsGame)
                ||
                ((MainList.SelectedItem as Release != null) && (MainList.SelectedItem as Release).IsGame)
                );
        }

        private void MarkNotGame_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.MarkGame(false);
        }


        private void MarkAsBeaten_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList.SelectedItem != null && (((MainList.SelectedItem as Game) != null && !(MainList.SelectedItem as Game).IsBeaten) || (MainList.SelectedItem as Release != null) && !(MainList.SelectedItem as Release).IsBeaten);
        }

        private void MarkAsBeaten_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.MarkBeaten(true);
        }

        private void MarkNotBeaten_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList.SelectedItem != null && (((MainList.SelectedItem as Game) != null && (MainList.SelectedItem as Game).IsBeaten) || (MainList.SelectedItem as Release != null) && (MainList.SelectedItem as Release).IsBeaten);
        }

        private void MarkNotBeaten_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.MarkBeaten(false);
        }


        private void AddCollection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddCollection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.AddCollection();
        }

        private void RemoveCollection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RemoveCollection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Collection collection = (e.OriginalSource as Control).DataContext as Collection;
            MWVM.CollectionList.Remove(collection);
        }


        private void RemoveFromCollection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList.DataContext == MWVM.SelectedCollection;
        }

        private void RemoveFromCollection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int N = MWVM.SelectedDBs.Count;
            for (int i = N - 1; i >= 0; i--)
            {
                IDBobject idbObject = MWVM.SelectedDBs[i] as IDBobject;
                MWVM.SelectedCollection.Remove(idbObject);
            }
        }


        private void ClearFilters_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainList != null && MainList.ItemsSource != null;
        }

        private void ClearFilters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MWVM.ClearFilters(MainList.DataContext);
        }

        private void ReporterWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ReporterWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReporterWindow reporterWindow = new ReporterWindow();
            reporterWindow.Show();
            reporterWindow.Activate();
        }

        private void QuitFocus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void QuitFocus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, null);
        }
    }
}
