//using AlphaChiTech.Virtualization;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Robin.Classes
//{
//    public class TesterSource : IPagedSourceProvider<SimpleDataItem>
//    {
//        public PagedSourceItemsPacket<SimpleDataItem> GetItemsAt(int pageoffset, int count, bool usePlaceholder)
//        {
//            return new PagedSourceItemsPacket<SimpleDataItem>()
//            {
//                LoadedAt = DateTime.Now,
//                Items = (from items in SimpleDataSource.Instance.Items select items).Skip(pageoffset).Take(count)
//            };
//        }

//        public int Count
//        {
//            get { return SimpleDataSource.Instance.Items.Count; }
//        }

//        public int IndexOf(SimpleDataItem item)
//        {
//            return SimpleDataSource.Instance.Items.IndexOf(item);
//        }

//        public void OnReset(int count)
//        {
//        }
//    }

//    public class SimpleViewModel
//    {
//        private ObservableCollection<SimpleDataItem> _MyData = null;
//        public ObservableCollection<SimpleDataItem> MyData
//        {
//            get
//            {
//                if (_MyData == null)
//                {
//                    _MyData = new ObservableCollection<SimpleDataItem>();
//                    foreach (var o in SimpleDataSource.Instance.Items)
//                    {
//                        _MyData.Add(o);
//                    }
//                }
//                return _MyData;
//            }
//        }
//    }
//}
