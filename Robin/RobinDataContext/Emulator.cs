using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class Emulator : INotifyPropertyChanged
	{
		public Emulator()
		{
			Cores = new HashSet<Core>();
			Platforms = new HashSet<Platform>();
			Platforms1 = new HashSet<Platform>();
		}

		public long ID { get; set; }

		public string Title { get; set; }


		private string fileName;
		public string FileName
		{
			get => fileName;
			set
			{
				fileName = value;
				OnPropertyChanged(nameof(FileName));
				OnPropertyChanged(nameof(FilePath));
			}
		}

		private string parameters;
		public string Parameters
		{
			get => parameters;
			set { parameters = value; OnPropertyChanged(nameof(Parameters)); }
		}


		public string Image { get; set; }

		private bool isCrap;
		public bool IsCrap
		{
			get => isCrap;
			set { isCrap = value; OnPropertyChanged(nameof(IsCrap)); }
		}

		public string Website { get; set; }

		private double? rating;
		public double? Rating
		{
			get => rating;
			set { rating = value; OnPropertyChanged(nameof(Rating)); }
		}

		public virtual ICollection<Core> Cores { get; set; }
		public virtual ICollection<Platform> Platforms { get; set; }
		public virtual ICollection<Platform> Platforms1 { get; set; }


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
