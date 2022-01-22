using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin
{
	public partial class Gdbrelease : IDbRelease
	{
		public Gdbrelease()
		{
			Releases = new HashSet<Release>();
		}

		public long Id { get; set; }
		public string Title { get; set; }
		public long GdbplatformId { get; set; }
		public string Developer { get; set; }
		public string Publisher { get; set; }
		public string Players { get; set; }
		public string Overview { get; set; }
		public double? Rating { get; set; }
		public string Genre { get; set; }
		public DateTime? Date { get; set; }

		public string BoxFrontUrl { get; set; }
		public string BoxBackUrl { get; set; }
		public string BannerUrl { get; set; }
		public string ScreenUrl { get; set; }
		public string LogoUrl { get; set; }
		public bool? Coop { get; set; }

		public virtual Gdbplatform Gdbplatform { get; set; }
		public virtual ICollection<Release> Releases { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
