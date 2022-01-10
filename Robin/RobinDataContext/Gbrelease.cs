using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robin
{
	public partial class Gbrelease : INotifyPropertyChanged
	{
		public Gbrelease()
		{
			Releases = new HashSet<Release>();
		}

		public long Id { get; set; }
		public string Title { get; set; }
		public long? RegionId { get; set; }
		public string Overview { get; set; }

		[Column("GBGame_ID")]
		public long? GbgameId { get; set; }
		public string Players { get; set; }
		public long GbplatformId { get; set; }
		public string BoxUrl { get; set; }
		public string ScreenUrl { get; set; }
		public DateTime? Date { get; set; }

		public virtual Gbgame Gbgame { get; set; }
		public virtual Gbplatform Gbplatform { get; set; }
		public virtual Region Region { get; set; }
		public virtual ICollection<Release> Releases { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
