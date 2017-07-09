using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robin
{
	/// <summary>
	/// Interaction logic for ArtWindow.xaml
	/// </summary>
	public partial class ArtWindow : Window
	{
		public ArtWindow(Release release)
		{
			InitializeComponent();
			DataContext = release;
		}
	}
}
