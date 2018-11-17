using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Robin.Core;
using Settings = Robin.WPF.Properties.Settings;

namespace Robin.WPF
{
	/// <summary>
	/// Interaction logic for ViewSelectWindow.xaml
	/// </summary>
	public partial class OptionsWindow : Window
	{
		Settings Settings => Settings.Default;

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

		public bool DisplayMess
		{
			get
			{
				return Settings.DisplayMess;
			}
			set
			{
				Settings.DisplayMess = value;
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


		public bool ScrapePlatformBoxFront
		{
			get
			{
				return Settings.ScrapePlatformBoxFront;
			}
			set
			{
				Settings.ScrapePlatformBoxFront = value;
				HasChanges = true;
			}
		}

		public bool ScrapePlatformBoxBack
		{
			get
			{
				return Settings.ScrapePlatformBoxBack;
			}
			set
			{
				Settings.ScrapePlatformBoxBack = value;
				HasChanges = true;
			}
		}

		public bool ScrapePlatformBanner
		{
			get
			{
				return Settings.ScrapePlatformBanner;
			}
			set
			{
				Settings.ScrapePlatformBanner = value;
				HasChanges = true;
			}
		}

		public bool ScrapePlatformConsole
		{
			get
			{
				return Settings.ScrapePlatformConsole;
			}
			set
			{
				Settings.ScrapePlatformConsole = value;
				HasChanges = true;
			}
		}

		public bool ScrapePlatformController
		{
			get
			{
				return Settings.ScrapePlatformController;
			}
			set
			{
				Settings.ScrapePlatformController = value;
				HasChanges = true;
			}
		}


		public bool ScrapeReleaseBoxFront
		{
			get
			{
				return Settings.ScrapeReleaseBoxFront;
			}
			set
			{
				Settings.ScrapeReleaseBoxFront = value;
				HasChanges = true;
			}
		}

		public bool ScrapeReleaseBoxBack
		{
			get
			{
				return Settings.ScrapeReleaseBoxBack;
			}
			set
			{
				Settings.ScrapeReleaseBoxBack = value;
				HasChanges = true;
			}
		}

		public bool ScrapeReleaseScreen
		{
			get
			{
				return Settings.ScrapeReleaseScreen;
			}
			set
			{
				Settings.ScrapeReleaseScreen = value;
				HasChanges = true;
			}
		}

		public bool ScrapeReleaseBanner
		{
			get
			{
				return Settings.ScrapeReleaseBanner;
			}
			set
			{
				Settings.ScrapeReleaseBanner = value;
				HasChanges = true;
			}
		}

		public bool ScrapeReleaseLogo
		{
			get
			{
				return Settings.ScrapeReleaseLogo;
			}
			set
			{
				Settings.ScrapeReleaseLogo = value;
				HasChanges = true;
			}
		}


		public AppSettings.DisplayOption DisplayChoice
		{
			get
			{
				return AppSettings.DisplayChoice;
			}
			set
			{
				AppSettings.DisplayChoice = value;
				HasChanges = true;
			}
		}

		public IEnumerable<AppSettings.DisplayOption> DisplayOptions
		{
			get
			{
				return Enum.GetValues(typeof(AppSettings.DisplayOption)).Cast<AppSettings.DisplayOption>();
			}
		}


		bool displayCrapStorage;
		bool displayAdultStorage;
		bool displayNonGamesStorage;
		bool displayNotIncludedStorage;
		bool displayMessStorage;
		bool sortGamesRandomStorage;

		bool scrapePlatformBoxFrontStorage;
		bool scrapePlatformBoxBackStorage;
		bool scrapePlatformBannerStorage;
		bool scrapePlatformConsoleStorage;
		bool scrapePlatformControllerStorage;

		bool scrapeReleaseBoxFrontStorage;
		bool scrapeReleaseBoxBackStorage;
		bool scrapeReleaseBannerStorage;
		bool scrapeReleaseScreenStorage;
		bool scrapeReleaseLogoStorage;

		AppSettings.DisplayOption displayChoiceStorage;

		bool HasChanges;

		public OptionsWindow()
		{
			StoreSettings();
			InitializeComponent();
			DataContext = this;

			OKCommand = new Command(OK, OKCanExecute, "OK", "Accept changes.");
			CancelCommand = new Command(Cancel, "Cancel", "Discard changes");

			try
			{
				Show();
				Activate();
			}
			catch { }

			
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
			displayCrapStorage = DisplayCrap;
			displayAdultStorage = DisplayAdult;
			displayNonGamesStorage = DisplayNonGames;
			displayNotIncludedStorage = DisplayNotIncluded;
			displayMessStorage = DisplayMess;

			sortGamesRandomStorage = SortGamesRandom;

			displayChoiceStorage = DisplayChoice;

			scrapePlatformBoxFrontStorage = ScrapePlatformBoxFront;
			scrapePlatformBoxBackStorage = ScrapePlatformBoxBack;
			scrapePlatformBannerStorage = ScrapePlatformBanner;
			scrapePlatformConsoleStorage = ScrapePlatformConsole;
			scrapePlatformControllerStorage = ScrapePlatformController;

			scrapeReleaseBoxFrontStorage = ScrapeReleaseBoxFront;
			scrapeReleaseBoxBackStorage = ScrapeReleaseBoxBack;
			scrapeReleaseScreenStorage = ScrapeReleaseScreen;
			scrapeReleaseBannerStorage = ScrapeReleaseBanner;
			scrapeReleaseLogoStorage = ScrapeReleaseLogo;

		}

		void RestoreSettings()
		{
			DisplayCrap = displayCrapStorage;
			DisplayAdult = displayAdultStorage;
			DisplayNonGames = displayNonGamesStorage;
			DisplayNotIncluded = displayNotIncludedStorage;
			DisplayMess = displayMessStorage;

			SortGamesRandom = sortGamesRandomStorage;

			DisplayChoice = displayChoiceStorage;

			ScrapePlatformBoxFront = scrapePlatformBoxFrontStorage;
			ScrapePlatformBoxBack = scrapePlatformBoxBackStorage;
			ScrapePlatformBanner = scrapePlatformBannerStorage;
			ScrapePlatformConsole = scrapePlatformConsoleStorage;
			ScrapePlatformController = scrapePlatformControllerStorage;

			ScrapeReleaseBoxFront = scrapeReleaseBoxFrontStorage;
			ScrapeReleaseBoxBack = scrapeReleaseBoxBackStorage;
			ScrapeReleaseScreen = scrapeReleaseScreenStorage;
			ScrapeReleaseBanner = scrapeReleaseBannerStorage;
			ScrapeReleaseLogo = scrapeReleaseLogoStorage;

		}

	}
}
