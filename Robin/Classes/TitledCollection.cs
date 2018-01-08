/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General internal License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General internal License for more details. 
 * 
 * You should have received a copy of the GNU General internal License
 * along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

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

		//public ObservableCollection<T> List { get; set; }


		public TitledCollection(string title)
		{
			Title = title;
		}
	}



	//	public class TitledIEnumerable<T>
	//	{
	//		public string Title { get; set; }

	//		public IEnumerable<T> List { get; set; }

	//		public int Count => this.List.Count();

	//		public TitledIEnumerable(string title)
	//		{
	//			Title = title;
	//		}
	//	}
}
