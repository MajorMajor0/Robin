/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Robin
{
	public partial class DatabaseWindow : Window
	{
		DatabaseWindowViewModel DBWVM;

		public DatabaseWindow()
		{
			DBWVM = new DatabaseWindowViewModel();
			InitializeComponent();
			DataContext = DBWVM;
			Activate();
		}

		private void BONUS_button_Click(object sender, RoutedEventArgs e)
		{
			Reporter.Report("BONUS!");
		}

		private void Compare_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CompareToDBAsync();
		}

		private void AcceptClick(object sender, RoutedEventArgs e)
		{
			//if (DatabaseGrid.Content != null &&
			//	DatabaseGrid.Content.GetType() == typeof(Compares))
			//{
			//	DBWVM.Accept();
			//}
		}

		public void GetDirectory_Click(object sender, RoutedEventArgs e)
		{
			//if (PlatformList.SelectedItem != null)
			//{
			//	(PlatformList.SelectedItem as Platform).GetReleaseDirectoryAsync(DBWVM.WindowReporter);
			//}
		}

		private void WriteDB_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.SaveChanges();
		}

		// Add Database file from datomatic
		private async void GetDatomatic_Click(object sender, RoutedEventArgs e)
		{
			await Task.Run(() =>
			{
			});
			//if (PlatformList.SelectedItems.Count == 1)
			//{
			//	Platform platform = PlatformList.SelectedItem as Platform;
			//	await Task.Run(() =>
			//	{
			//		//DBWVM.R.Data.Configuration.AutoDetectChangesEnabled = false;
			//		Datomatic datomatic = new Datomatic();
			//		datomatic.CacheFromXML(platform);
			//		//DBWVM.R.Data.ChangeTracker.DetectChanges();
			//		//DBWVM.SaveChanges();
			//		//DBWVM.R.Data.Configuration.AutoDetectChangesEnabled = false;
			//	});
			//}
		}

		private void PlatformList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (PlatformList.SelectedItems.Count > 0)
			{
				DatabaseGrid.DataContext = DBWVM.SelectedPlatform;
				CountBlock.DataContext = PlatformList.SelectedItem;
			}
		}

		private void ComparisonResultsList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			DatabaseGrid.DataContext = ComparisonResultsList.SelectedItem;
			if (ComparisonResultsList.SelectedItem != null)
			{
				CountBlock.DataContext = ((Compares)ComparisonResultsList.SelectedItem).List;
			}
		}

		private void DataGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			//if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			//{
			//	DataGridScale.ScaleX += (e.Delta > 0) ? .1 : -.1;
			//	DataGridScale.ScaleY += (e.Delta > 0) ? .1 : -.1;
			//}
		}

		private void Cache_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CachePlatform();
		}

		private void ArtWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ArtWindow_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Release release = (e.OriginalSource as Control).DataContext as Release;
			ArtWindow artWindow = new ArtWindow(release);
			artWindow.Show();
			artWindow.Activate();
		}

		private void MatchWindow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void MatchWindow_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Release release = (e.OriginalSource as Control).DataContext as Release;
			MatchWindow matchWindow = new MatchWindow(release);
			matchWindow.Show();
			matchWindow.Activate();
		}
	}
	public static class DBCommands
	{
		public static RoutedUICommand ArtWindow = new RoutedUICommand("Open Art Window", "ArtWindow", typeof(CustomCommands));
		public static RoutedUICommand MatchWindow = new RoutedUICommand("Open Match Window", "ArtWindow", typeof(CustomCommands));

	}
}
