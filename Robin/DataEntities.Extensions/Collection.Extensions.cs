/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System.Collections.Generic;
using System.Linq;

namespace Robin
{
	public partial class Collection
	{
		public IEnumerable<IDBobject> FilteredCollection
		{
			get { return (Games as IEnumerable<IDBobject>).Union(Releases as IEnumerable<IDBobject>); }
		}

		public void Add(IDBobject idbObject)
		{
			if (idbObject is Game)
			{
				Games.Add(idbObject as Game);
				OnPropertyChanged("FilteredCollection");
			}
			if (idbObject is Release)
			{
				Releases.Add(idbObject as Release);
				OnPropertyChanged("FilteredCollection");
			}
		}

		public void Remove(IDBobject idbObject)
		{
			if (idbObject is Game)
			{
				Games.Remove(idbObject as Game);
				OnPropertyChanged("FilteredCollection");
			}
			if (idbObject is Release)
			{
				Releases.Remove(idbObject as Release);
				OnPropertyChanged("FilteredCollection");
			}
		}
	}
}
