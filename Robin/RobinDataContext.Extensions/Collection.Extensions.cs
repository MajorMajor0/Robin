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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Robin
{
	public partial class Collection
	{
		[NotMapped]
		public IEnumerable<IDbObject> FilteredCollection
		{
			get { return (Games as IEnumerable<IDbObject>).Union(Releases as IEnumerable<IDbObject>); }
		}

		public Collection(IEnumerable<IDbObject> objects) : this()
		{
			foreach (IDbObject idbo in objects)
			{
				if (idbo is Game)
				{
					Games.Add(idbo as Game);
				}

				else if (idbo is Release)
				{
					Releases.Add(idbo as Release);
				}
			}

		}

		public void Add(IDbObject idbObject)
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

		public void Remove(IDbObject idbObject)
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
