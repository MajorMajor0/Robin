using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class Collection : INotifyPropertyChanged
	{
		public Collection()
		{
			Games = new HashSet<Game>();
			Releases = new List<Release>();
		}

		public long Id { get; set; }

		private string title;
		public string Title
		{
			get => title;
			set { title = value; OnPropertyChanged(nameof(Title)); }
		}

		private string type;
		public string Type
		{
			get => type;
			set { type = value; OnPropertyChanged(nameof(Type)); }
		}


		public virtual ICollection<Game> Games { get; set; }
		public virtual IList<Release> Releases { get; set; }


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
