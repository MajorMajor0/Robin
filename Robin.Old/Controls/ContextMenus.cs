using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Robin
{
    
    public partial class MainWindow : Window
    {
        
        //public class ContextMenus
        //{
        //    public class Mainlist
        //    {

        //        public static ContextMenu Roms {get; set;}
        //        public static ContextMenu Platforms { get; set; }
        //        private static MenuItem Roms_MenuItem1 { get; set; }
        //        private static MenuItem Platforms_MenuItem1 { get; set; }
        //        private static MenuItem MenuItem3 { get; set; }

        //        static Mainlist()
        //        {
        //            Roms = new ContextMenu();
        //            Roms_MenuItem1 = new MenuItem();
        //            Roms_MenuItem1.Header = "Compare to GDB";
        //            Roms_MenuItem1.Click += new System.Windows.RoutedEventHandler(Item1_Click);
        //            Roms.Items.Add(Roms_MenuItem1);

        //            Platforms = new ContextMenu();
        //            Platforms_MenuItem1 = new MenuItem();
        //            Platforms_MenuItem1.Header = "Get games";
        //            Platforms_MenuItem1.Click += new System.Windows.RoutedEventHandler(MainList_Menu_GetGames);
        //            Platforms.Items.Add(Platforms_MenuItem1);
        //        }

        //        public static void Item1_Click(Object sender, RoutedEventArgs e)
        //        {
        //            MessageBox.Show("Hello!");
                    
        //        }

        //        private void MainList_Menu_GetGames(object sender, RoutedEventArgs e)
        //        {
        //            ((Platform)Robin.MainList.SelectedItem).GetGames();
        //        }
        //    }       
        //}
    }
}
