using Robin.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robin
{
	/// <summary>
	/// Interaction logic for ViewSelectWindow.xaml
	/// </summary>
	public partial class OptionsWindow : Window
	{
		Settings Settings => Properties.Settings.Default;

		public bool DisplayCrap
		{
			get
			{
				return Settings.DisplayCrap;
			}
			set
			{
				Settings.DisplayCrap = value;
				HasChanges = true;
			}
		}

		public bool DisplayAdult
		{
			get
			{
				return Settings.DisplayAdult;
			}
			set
			{
				Settings.DisplayAdult = value;
				HasChanges = true;
			}
		}

		public bool DisplayNonGames
		{
			get
			{
				return Settings.DisplayNonGames;
			}
			set
			{
				Settings.DisplayNonGames = value;
				HasChanges = true;
			}
		}

		public bool DisplayNotIncluded
		{
			get
			{
				return Settings.DisplayNotIncluded;
			}
			set
			{
				Settings.DisplayNotIncluded = value;
				HasChanges = true;
			}
		}

		public bool SortGamesRandom
		{
			get
			{
				return Settings.SortGamesRandom;
			}
			set
			{
				Settings.SortGamesRandom = value;
				HasChanges = true;
			}
		}

		bool displayCrap;
		bool displayAdult;
		bool displayNonGames;
		bool displayNotIncluded;

		bool HasChanges;

		public OptionsWindow()
		{
			StoreSettings();
			InitializeComponent();
			DataContext = this;

			OKCommand = new Command(OK, OKCanExecute, "OK", "Accept changes.");
			CancelCommand = new Command(Cancel, "Cancel", "Discard changes");

			Show();
			Activate();
		}

		public Command OKCommand { get; set; }

		 void OK()
		{
			Close();
		}

		 bool OKCanExecute()
		{
			return HasChanges;
		}


		public Command CancelCommand { get; set; }

		 void Cancel()
		{
			RestoreSettings();
			Close();
		}

		 void StoreSettings()
		{
			displayCrap = DisplayCrap;
			displayAdult = DisplayAdult;
			displayNonGames = DisplayNonGames;
			displayNotIncluded = DisplayNotIncluded;
		}

		 void RestoreSettings()
		{
			DisplayCrap = displayCrap;
			DisplayAdult = displayAdult;
			DisplayNonGames = displayNonGames;
			DisplayNotIncluded = displayNotIncluded;
		}

	}
}
