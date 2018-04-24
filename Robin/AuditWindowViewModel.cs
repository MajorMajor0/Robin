using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public class AuditWindowViewModel : INotifyPropertyChanged
	{
		public List<Platform> PlatformList => R.Data.Platforms.Local.OrderBy(x => x.Title).ToList();
		public Platform SelectedPlatform { get; set; }

		TitledCollection<Audit.Result> selectedAuditResults;
		public TitledCollection<Audit.Result> SelectedAuditResults
		{
			get
			{
				return selectedAuditResults;
			}

			set
			{
				selectedAuditResults = value;
				OnPropertyChanged("AuditResults");
				OnPropertyChanged("GoodResults");
				OnPropertyChanged("MissingResults");
				OnPropertyChanged("BadResults");
				OnPropertyChanged("BestResults");
				OnPropertyChanged("GoodCount");
				OnPropertyChanged("MissingCount");
				OnPropertyChanged("BadCount");
				OnPropertyChanged("BestCount");
			}
		}

		public ObservableCollection<TitledCollection<Audit.Result>> AuditResultsCollection { get; set; } = new ObservableCollection<TitledCollection<Audit.Result>>();

		public List<Audit.Result> GoodResults => SelectedAuditResults?.Where(x => x.Status == Status.Good).ToList();

		public List<Audit.Result> BadResults => SelectedAuditResults?.Where(x => x.Status == Status.Bad).ToList();

		public List<Audit.Result> MissingResults => SelectedAuditResults?.Where(x => x.Status == Status.Missing).ToList();

		public List<Audit.Result> BestResults => SelectedAuditResults?.Where(x => x.Status == Status.Best).ToList();

		public int GoodCount => SelectedAuditResults?.Count(x => x.Status == Status.Good) ?? 0;

		public int MissingCount => SelectedAuditResults?.Count(x => x.Status == Status.Missing) ?? 0;

		public int BadCount => SelectedAuditResults?.Count(x => x.Status == Status.Bad) ?? 0;

		public int BestCount => SelectedAuditResults?.Count(x => x.Status == Status.Best) ?? 0;

		public AuditWindowViewModel()
		{
			AuditCommand = new Command(Audit, AuditCanExecute, "Audit", "Audit selected platform.");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		public Command AuditCommand { get; set; }

		async void Audit()
		{
			Reporter.Report("Auditing");
			TitledCollection<Audit.Result> resultCollection = null;

			await Task.Run(() =>
			{
				resultCollection = Robin.Audit.AuditRoms(SelectedPlatform);
			});

			AuditResultsCollection.Add(resultCollection);
		}

		bool AuditCanExecute()
		{
			return SelectedPlatform != null;
		}
	}
}
