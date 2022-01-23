using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Robin;

public partial class GDBPlatform : INotifyPropertyChanged
{
	public GDBPlatform()
	{
		GDBReleases = new HashSet<GDBRelease>();
	}

	public long ID { get; set; }
	public string Title { get; set; }

	public string Developer { get; set; }
	public string Manufacturer { get; set; }
	public string Cpu { get; set; }
	public string Sound { get; set; }
	public string Display { get; set; }
	public string Media { get; set; }
	public string Controllers { get; set; }
	public double? Rating { get; set; }
	public string Overview { get; set; }
	public string BoxFrontUrl { get; set; }
	public string BoxBackUrl { get; set; }
	public string BannerUrl { get; set; }
	public string ConsoleUrl { get; set; }
	public string ControllerUrl { get; set; }

	public DateTime? Date { get; set; }
	public DateTime CacheDate { get; set; }

	public virtual Platform Platform { get; set; }
	public virtual HashSet<GDBRelease> GDBReleases { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
