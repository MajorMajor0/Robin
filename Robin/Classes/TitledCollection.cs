using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public class TitledCollection<T> : ObservableCollection<T>
	{
		public string Title { get; set; }

		public ObservableCollection<T> List { get; set; }


		public TitledCollection(string title)
		{
			Title = title;
		}
	}



	//public class TitledIEnumerable<T>
	//{
	//	public string Title { get; set; }

	//	public IEnumerable<T> List { get; set; }

	//	public int Count => this.List.Count();

	//	public TitledIEnumerable(string title)
	//	{
	//		Title = title;
	//	}
	//}
}
