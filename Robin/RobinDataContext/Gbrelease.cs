using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robin
{
	public partial class GBRelease : INotifyPropertyChanged
	{
		public GBRelease()
		{
			Releases = new HashSet<Release>();
		}

		public long ID { get; set; }
		public string Title { get; set; }
		public long? RegionId { get; set; }
		public string Overview { get; set; }

		[Column("GBGame_ID")]
		public long? GBGameId { get; set; }
		public string Players { get; set; }
		public long GBPlatformId { get; set; }
		public string BoxUrl { get; set; }
		public string ScreenUrl { get; set; }
		public DateTime? Date { get; set; }

		public virtual GBGame GBGame { get; set; }
		public virtual GBPlatform GBPlatform { get; set; }
		public virtual Region Region { get; set; }
		public virtual ICollection<Release> Releases { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
