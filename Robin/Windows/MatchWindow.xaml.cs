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
		readonly MatchWindowViewModel MWVM;

		public MatchWindow(Release release)
		{
			MWVM = new MatchWindowViewModel(release);
			DataContext = MWVM;
			InitializeComponent();
			Show();
			Activate();
		}
	}
}


