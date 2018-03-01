using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
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
