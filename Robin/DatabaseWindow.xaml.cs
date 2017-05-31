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

namespace Robin
{
	public partial class DatabaseWindow : Window
	{
		DatabaseWindowViewModel DBWVM;
		public DatabaseWindow(RobinDataEntities rdata)
		{
			DBWVM = new DatabaseWindowViewModel(rdata);
			InitializeComponent();
			DataContext = DBWVM;
		}

		private void BONUS_button_Click(object sender, RoutedEventArgs e)
		{
			Reporter.Report("BONUS!");
			DBWVM.CopyData();
		}

		private void CompareToGamesdb_click(object sender, RoutedEventArgs e)
		{
			DBWVM.CompareToDBAsync(LocalDB.GamesDB);
		}

		private void Compare_to_GB_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CompareToDBAsync(LocalDB.GiantBomb);
		}
		private void CompareToOVG_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CompareToDBAsync(LocalDB.OpenVGDB);
		}

		private void CompareToLB_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CompareToDBAsync(LocalDB.LaunchBox);
		}

		private void AcceptClick(object sender, RoutedEventArgs e)
		{
			if (DatabaseGrid.Content != null &&
				DatabaseGrid.Content.GetType() == typeof(Compares))
			{
				DBWVM.Accept();
			}
		}

		private void DatabaseGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			////string type = PlatformList.SelectedItem.GetType().ToString();
			//string type = DatabaseGrid.Content.GetType().ToString();
			//type = type.Split(new char[] { '.', '_' })[4];

			//switch (type)
			//{
			//    case "Compares":
			//        //        if (e.Column.Header.ToString() == "ToolTip") e.Cancel = true;
			//        //        if (e.Column.Header.ToString() == "DBIndex") e.Cancel = true;
			//        //        if (e.Column.Header.ToString() == "RIndex") e.Cancel = true;
			//        //        if (e.Column.Header.ToString() == "Title") e.Cancel = true;
			//        //        if (e.Column.Header.ToString() == "DBID") e.Cancel = true;
			//        //        if (e.Column.Header.ToString() == "RID") e.Cancel = true;
			//        //        if (e.Column.Header.ToString() == "AcceptMatch") e.Column.DisplayIndex = 1;
			//        //        if (e.Column.Header.ToString() == "DBTitle") e.Column.DisplayIndex = 2;
			//        break;

			//    case "Release":
			//        //    if (e.PropertyType == typeof(Robin.Game))
			//        //    {
			//        //        var col_gam = new DataGridTextColumn();
			//        //        col_gam.Header = "Game";
			//        //        col_gam.Binding = new Binding("Game.Name");
			//        //        e.Column = col_gam;
			//        //    }

			//        //    if (e.PropertyType == typeof(Robin.id))
			//        //    {
			//        //        var col_id = new DataGridTextColumn();
			//        //        col_id.Header = "ID";
			//        //        col_id.Binding = new Binding("Id.GamesDB");
			//        //        e.Column = col_id;
			//        //    }

			//        //    if (e.PropertyType == typeof(Robin.crc))
			//        //    {
			//        //        var col_crc = new DataGridTextColumn();
			//        //        col_crc.Header = "SHA1";
			//        //        col_crc.Binding = new Binding("Crc.SHA1");
			//        //        e.Column = col_crc;
			//        //    }
			//        //    if (e.Column.Header.ToString() == "Name") e.Column.DisplayIndex = 1;
			//        //    if (e.Column.Header.ToString() == "HasChanged") e.Cancel = true;
			//        //    if (e.Column.Header.ToString() == "IsExpanded") e.Cancel = true;
			//        //    if (e.Column.Header.ToString() == "ToolTip") e.Cancel = true;
			//        break;

			//    case "Platform":
			//        if (e.Column.Header.ToString() == "Overview") e.Cancel = true;
			//        if (e.Column.Header.ToString() == "Platform") e.Cancel = true;
			//        if (e.Column.Header.ToString() == "Region") e.Cancel = true;
			//        if (e.Column.Header.ToString() == "CloneOf_ID") e.Cancel = true;
			//        if (e.Column.Header.ToString() == "games") e.Cancel = true;
			//        break;
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
			if (PlatformList.SelectedItems.Count == 1)
			{
				Platform platform = PlatformList.SelectedItem as Platform;
				await Task.Run(() =>
				{
					//DBWVM.Rdata.Configuration.AutoDetectChangesEnabled = false;
					Datomatic datomatic = new Datomatic();
					datomatic.CacheFromXML(platform);
					//DBWVM.Rdata.ChangeTracker.DetectChanges();
					//DBWVM.SaveChanges();
					//DBWVM.Rdata.Configuration.AutoDetectChangesEnabled = false;
				});
			}
		}

		private void PlatformList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (PlatformList.SelectedItems.Count > 0)
			{
				DatabaseGrid.Content = PlatformList.SelectedItem;
				CountBlock.DataContext = ((Platform)PlatformList.SelectedItem).Releases;
			}
		}

		private void ComparisonResultsList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			DatabaseGrid.Content = ComparisonResultsList.SelectedItem;
			if (ComparisonResultsList.SelectedItem != null)
			{
				CountBlock.DataContext = ((Compares)ComparisonResultsList.SelectedItem).List;
			}
		}

		private void DataGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				DataGridScale.ScaleX += (e.Delta > 0) ? .1 : -.1;
				DataGridScale.ScaleY += (e.Delta > 0) ? .1 : -.1;
			}
		}

		private void CacheGB_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CachePlatform(LocalDB.GiantBomb);
		}

		private void CacheGDB_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CachePlatform(LocalDB.GamesDB);
		}

		private void CacheOV_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CachePlatform(LocalDB.OpenVGDB);
		}

		private void CacheLB_button_Click(object sender, RoutedEventArgs e)
		{
			DBWVM.CachePlatform(LocalDB.LaunchBox);
		}
	}
}
