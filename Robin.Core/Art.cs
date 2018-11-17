using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin.Core
{
	public class Art
	{
		public string URL { get; }
		public string Title { get; }
		public string Path { get; }

		public Art(string title, string url, string path)
		{
			URL = url;
			Title = title;
			Path = path;
		}

	}
}
