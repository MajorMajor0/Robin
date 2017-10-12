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
using System.Windows.Controls;
using System.Windows.Input;

namespace Robin
{
	/// <summary>
	/// Interaction logic for MatchWindow.xaml
	/// </summary>
	public partial class MatchWindow : Window
	{
		MatchWindowViewModel MWVM;

		public MatchWindow(Release release)
		{
			MWVM = new MatchWindowViewModel(release);
			InitializeComponent();
			DataContext = MWVM;
            Show();
			Activate();
		}

		private void Match_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Match_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MWVM.Match((e.OriginalSource as Control).DataContext as IDBRelease);
		}

		private void ShowBox_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ShowBox_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MWVM.ShowBox((e.OriginalSource as Control).DataContext as IDBRelease);
		}
	}

	public static class MWCommands
	{
		public static RoutedUICommand Match = new RoutedUICommand("Match", "Match", typeof(MWCommands));
		public static RoutedUICommand ShowBox = new RoutedUICommand("Show box art", "ShowBox", typeof(MWCommands));

	}
}


