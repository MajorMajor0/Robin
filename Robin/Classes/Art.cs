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
	public abstract class Art : INotifyPropertyChanged
	{
		public virtual string URL { get; set; }

		public virtual string Title { get; }

        public virtual string Abbreviation { get; }

		public bool Scrape(string Path)
		{
			using (WebClient webclient = new WebClient())
			{
				if (!File.Exists(Path))
				{
					if (URL != null)
					{
						Reporter.Report("Attempting scrape from " + URL);

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

        public abstract string Path(string type, long id);


		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}

	public partial class Banner : Art
	{
		public override string Title => "Banner";

		public Banner(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-BNR");
        }

    }

	public partial class Box3D : Art
	{
		public override string Title => "Box 3D";

		public Box3D(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-B3D");
        }
    }

	public partial class BoxBack : Art
	{
		public override string Title => "Box Back";

		public BoxBack(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-BXB");
        }
    }

	public partial class BoxFront : Art
	{
		public override string Title => "Box Front";

		public BoxFront(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-BXF");
        }
    }

	public partial class Cart3D : Art
	{
		public override string Title => "Cart 3D";

		public Cart3D(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-C3D");
        }
    }

	public partial class CartBack : Art
	{
		public override string Title => "Cart Back";

		public CartBack(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-CB");
        }
    }

	public partial class CartFront : Art
	{
		public override string Title => "Cart Front";

		public CartFront(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-CF");
        }
    }

	public partial class ControlPanel : Art
	{
		public override string Title => "Control Panel";

		public ControlPanel(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-CP");
        }
    }

	public partial class ControlInformation : Art
	{
		public override string Title => "Control Information";

		public ControlInformation(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-CI");
        }
    }

    public partial class Logo : Art
    {
        public override string Title => "Logo";

        public Logo(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-LGO");
        }
    }

    public partial class Marquee : Art
    {
        public override string Title => "Marquee";

        public Marquee(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-MAR");
        }
    }

    public partial class Screen : Art
    {
        public override string Title => "Screen";

        public Screen(string url)
        {
            URL = url;
        }

        public override string Path(string type, long id)
        {
            return string.Concat(type, "-", id, "-SCR");
        }
    }
}
