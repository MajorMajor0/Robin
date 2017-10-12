using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robin
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MergeWindow : Window, INotifyPropertyChanged
	{
		public IEnumerable<LBPlatform> LBPlatformList
		{
			get
			{
				if (searchTerm != null)
				{
					return R.Data.LBPlatforms.Local.Where(x => x != null && Regex.IsMatch(x.Title, SearchTerm, RegexOptions.IgnoreCase));
				}
				return R.Data.LBPlatforms.Local;
			}
		}

		public LBPlatform SelectedLBPlatform { get; set; }

		string searchTerm;

		public string SearchTerm
		{
			get
			{
				return searchTerm;
			}

			set
			{
				if (searchTerm != value)
				{
					searchTerm = value;
					OnPropertyChanged("LBPlatformList");
				}
			}
		}
		
		public string NewTitle { get; }

		public MergeWindow(string newTitle)
		{
			InitializeComponent();
			
			NewTitle = newTitle;
			AddCommand = new Command(Add, "Add", "Add the new platform to local Launchbox cache.");
			MergeCommand = new Command(Merge, "Merge", "Merge the new platform with the selected existing Launchbox platform and update.");
			DataContext = this;
		}


		public Command AddCommand { get; set; }

		public void Add()
		{
			DialogResult = false;
		}


		public Command MergeCommand { get; set; }

		public void Merge()
		{
			DialogResult = true;
			Close();
		}

		private bool MergeCanExecute()
		{
			return SelectedLBPlatform != null;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}


	}
}
