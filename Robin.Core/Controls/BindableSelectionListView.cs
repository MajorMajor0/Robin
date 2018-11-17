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

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Robin.Core
{
    public class BindableSelectionListView : ListView
    {
        public BindableSelectionListView()
        {
            SelectionChanged += CustomListView_SelectionChanged;
        }

        void CustomListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoundSelectedItems = SelectedItems;
        }

        public IList BoundSelectedItems
        {
            get
            {
                return SelectedItems;
            }

            set { SetValue(BoundSelectedItemsProperty, value); }
        }

		public static readonly DependencyProperty BoundSelectedItemsProperty =
       DependencyProperty.Register("BoundSelectedItems", typeof(IList), typeof(BindableSelectionListView), new PropertyMetadata(null));

      //  public static readonly DependencyProperty BoundSelectedItemProperty =
      //DependencyProperty.Register("BoundSelectedItem", typeof(object), typeof(BindableSelectionListBox), new PropertyMetadata(null));

    }
}
