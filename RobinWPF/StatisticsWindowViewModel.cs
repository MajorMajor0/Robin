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

using System.Collections.ObjectModel;
using System.Linq;
using Robin.Core;

namespace Robin.WPF
{
	class StatisticsWindowViewModel
	{
		public ObservableCollection<Platform> Platforms => new ObservableCollection<Platform>(R.Data.Platforms.Local.Where(x => x.Preferred));

		public Platform SelectedPlatform { get; set; }

		public StatisticsWindowViewModel()
		{
			MatchWindowCommand = new Command(MatchWindow, MatchWindowCanExecute, "Match Window", "Open a window to match releases to databases");
		}


		public Command MatchWindowCommand { get; set; }

		void MatchWindow()
		{
			Release release = SelectedPlatform.Releases.FirstOrDefault(x => !x.HasArt) ?? SelectedPlatform.Releases.FirstOrDefault();

			MatchWindow matchWindow = new MatchWindow(release);
		}

		bool MatchWindowCanExecute()
		{
			return SelectedPlatform != null;
		}

	}
}
