using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class Game : IDbObject
	{
		public Game()
		{
			Releases = new List<Release>();
			Collections = new HashSet<Collection>();
		}

		public long ID { get; set; }

		private bool isGame;
		public bool IsGame
		{
			get => isGame;
			set { isGame = value; OnPropertyChanged(nameof(IsGame)); }
		}

		private bool isMess;
		public bool IsMess
		{
			get => isMess;
			set { isMess = value; OnPropertyChanged(nameof(IsMess)); }
		}

		private bool isCrap;
		public bool IsCrap
		{
			get => isCrap;
			set { isCrap = value; OnPropertyChanged(nameof(IsCrap)); }
		}

		private bool isAdult;
		public bool IsAdult
		{
			get => isAdult;
			set { isAdult = value; OnPropertyChanged(nameof(IsAdult)); }
		}

		private bool isBeaten;
		public bool IsBeaten
		{
			get => isBeaten;
			set { isBeaten = value; OnPropertyChanged(nameof(IsBeaten)); }
		}
		public string Overview { get; set; }

		public string Developer { get; set; }

		public string Publisher { get; set; }

		public string Genre { get; set; }

		public bool Unlicensed { get; set; }

		public string VideoFormat { get; set; }

		public string Players { get; set; }

		private double? rating;
		public double? Rating
		{
			get => rating;
			set { rating = value; OnPropertyChanged(nameof(Rating)); }
		}


		public virtual IList<Release> Releases { get; set; }

		public virtual ICollection<Collection> Collections { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
