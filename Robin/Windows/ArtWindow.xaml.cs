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
 
using System.Windows;

namespace Robin
{
	/// <summary>
	/// Interaction logic for ArtWindow.xaml
	/// </summary>
	public partial class ArtWindow : Window
	{
		ArtWindowViewModel AWVM;

		public ArtWindow(Release release)
		{
			AWVM = new ArtWindowViewModel(release);
			InitializeComponent();
			DataContext = AWVM;
			Show();
			Activate();
		}
	}
}
