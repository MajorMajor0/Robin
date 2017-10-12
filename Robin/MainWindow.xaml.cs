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
using GalaSoft.MvvmLight.Command;
using System.Windows.Media;

namespace Robin
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 

	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			QuitFocusCommand = new RelayCommand(QuitFocus);
			ListViewScale.ScaleX = .8;
			ListViewScale.ScaleY = .8;
			Activate();
#if DEBUG
			DebugStuff();
#endif
		}

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

		private void MainList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			MainList.UnselectAll();
		}

		private void MainListWrapPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (sender is VirtualizingWrapPanel panel && MainList.SelectedItems != null && e.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(panel, MainList.SelectedItems, DragDropEffects.Copy);
			}
		}

		private void TreeTextBlock_Drop(object sender, DragEventArgs e)
		{
			if ((e.OriginalSource as TextBlock).DataContext is Collection collection)
			{
				for (int i = 0; i < MainList.SelectedItems.Count; i++)
				{
					IDBobject idbObject = MainList.SelectedItems[i] as IDBobject;
					collection.Add(idbObject);
				}

			}
		}

		private void Main_Window_Closing(object sender, CancelEventArgs e)
		{
			R.Data.Save();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


		public RelayCommand QuitFocusCommand { get; set; }

		public void QuitFocus()
		{
			FocusManager.SetFocusedElement(this, null);
		}

#if DEBUG
		private void DebugStuff()
		{
			Button bonusButton = new Button();
			bonusButton.Content = "BONUS!";
			bonusButton.Width = 50;
			bonusButton.Height = 30;
			bonusButton.Click += BonusButton_Click;
			bonusButton.Foreground = new SolidColorBrush(Colors.Blue);
			DockPanel.SetDock(bonusButton, Dock.Right);
			SearchBox_DockPanel.Children.Add(bonusButton);
			PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;

			CreateThumbnailsCommand = new RelayCommand(CreateThumbnails);
			MenuItem createThumbs = new MenuItem();
			createThumbs.Header = "Create thumbnails";
			createThumbs.ToolTip = "Create a thumbnail for every release that doesn't have one.";
			createThumbs.Command = CreateThumbnailsCommand;
			createThumbs.Foreground = new SolidColorBrush(Colors.Blue);

			ArtMenuItem.Items.Add(createThumbs);

		}

		private async void BonusButton_Click(object sender, RoutedEventArgs e)
		{
			Reporter.Report("BONUS!");

			await Task.Run(() =>
			{
				
			});
		}

		public RelayCommand CreateThumbnailsCommand { get; set; }

		private void CreateThumbnails()
		{
			Stopwatch Watch = new Stopwatch();
			Watch.Start();
			int i = 0;

			foreach (Release release in R.Data.Releases)
			{
				Watch.Restart();
				Debug.WriteLine(i++ + " : " + Watch.ElapsedMilliseconds);

				release.CreateThumbnail();
			}
		}
#endif
	}
}
