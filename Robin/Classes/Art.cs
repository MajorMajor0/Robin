using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public class Art : INotifyPropertyChanged
	{
		public string URL { get; }
		public virtual string Title { get; }
		public string FileName { get; protected set; }
		public string Path => string.Concat(FileLocation.Temp, FileName);

		public LocalDB LocalDB { get; }

		public Art(string url, string type, long id)
		{
			URL = url;
		}

		public bool Scrape()
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(Path))
				{
					if (URL != null)
					{
						Reporter.Report("Attempting scrape for " + FileName);

						if (webclient.DownloadFileFromDB(URL, Path))
						{
							Reporter.ReportInline("success!");
							OnPropertyChanged("Path");
						}
						else
						{
							Reporter.ReportInline("dammit!");
							return false;
						}
					}

					else
					{
						Reporter.Report("No front box art URL exists for LB release" + Title);
					}
				}

				else
				{
					Reporter.Report("File already exists.");
				}
			}
			return true;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}

	public class BannerArt : Art
	{
		public override string Title => "Banner";

		public BannerArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-BNR");
		}
	}

	public class Box3DArt : Art
	{
		public override string Title => "Box 3D";

		public Box3DArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-BX3");
		}
	}

	public class BoxBackArt : Art
	{
		public override string Title => "Box Back";

		public BoxBackArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-BXB");
		}
	}

	public class BoxFrontArt : Art
	{
		public override string Title => "Box Front";

		public BoxFrontArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-BXF");
		}
	}

	public class Cart3DArt : Art
	{
		public override string Title => "Cart 3D";

		public Cart3DArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-C3D");
		}
	}

	public class CartBackArt : Art
	{
		public override string Title => "Cart Back";

		public CartBackArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-CB");
		}
	}

	public class CartFrontArt : Art
	{
		public override string Title => "Cart Front";

		public CartFrontArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-CF");
		}
	}

	public class ControlPanelArt : Art
	{
		public override string Title => "Control Panel";

		public ControlPanelArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-CP");
		}
	}

	public class ControlInformationArt : Art
	{
		public override string Title => "Control Information";

		public ControlInformationArt(string url, string type, long id) : base(url, type, id)
		{
			FileName = string.Concat(type, "-", id, "-CI");
		}
	}


}
