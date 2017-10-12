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
		public DatabaseWindow()
		{
			InitializeComponent();
			Show();
			Activate();
#if DEBUG
			DebugStuff();
#endif
		}

		private void AcceptClick(object sender, RoutedEventArgs e)
		{
			if (DatabaseGrid.Content != null &&
				DatabaseGrid.Content.GetType() == typeof(Compares))
			{
				(DataContext as DatabaseWindowViewModel).Accept();
			}
		}

		private void PlatformList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (PlatformList.SelectedItems.Count > 0)
			{
				DatabaseGrid.DataContext = (PlatformList.SelectedItem as IDBPlatform);
				CountBlock.DataContext = (PlatformList.SelectedItem as IDBPlatform).Releases;
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
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				DataGridScale.ScaleX += (e.Delta > 0) ? .1 : -.1;
				DataGridScale.ScaleY += (e.Delta > 0) ? .1 : -.1;
			}
		}

#if DEBUG
		private void DebugStuff()
		{
			Button BonusButton = new Button();
			BonusButton.Content = "BONUS!";
			BonusButton.Style = FindResource("DatabaseWindowButtonStyle1") as Style;
			BonusButton.Click += BonusButton_Click;
			DockPanel.SetDock(BonusButton, Dock.Right);
			Buttons_StackPanel.Children.Add(BonusButton);
		}

		private async void BonusButton_Click(object sender, RoutedEventArgs e)
		{
			Reporter.Report("BONUS!");

			await Task.Run(() =>
			{
			});
		}
#endif
	}
}
