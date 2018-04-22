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
		public ObservableCollection<Platform> PlatformList => R.Data.Platforms.Local;
		public Platform SelectedPlatform { get; set; }

		List<Audit.Result> auditResults;
		public List<Audit.Result> AuditResults
		{
			get
			{
				return auditResults;
			}

			set
			{
				auditResults = value;
				OnPropertyChanged("AuditResults");
				OnPropertyChanged("GoodResults");
				OnPropertyChanged("BadResults");
				OnPropertyChanged("BestResults");
				OnPropertyChanged("GoodCount");
				OnPropertyChanged("BadCount");
				OnPropertyChanged("BestCount");
			}
		}

		public List<Audit.Result> GoodResults => AuditResults?.Where(x => x.Status == Status.Good).ToList();

		public List<Audit.Result> BadResults => AuditResults?.Where(x => x.Status == Status.Bad).ToList();

		public List<Audit.Result> BestResults => AuditResults?.Where(x => x.Status == Status.Best).ToList();

		public int GoodCount => AuditResults?.Count(x => x.Status == Status.Good) ?? 0;

		public int BadCount => AuditResults?.Count(x => x.Status == Status.Bad) ?? 0;

		public int BestCount => AuditResults?.Count(x => x.Status == Status.Best) ?? 0;

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
			await Task.Run(() =>
			{
				AuditResults = SelectedPlatform.AuditRoms();
			});
		}

		bool AuditCanExecute()
		{
			return SelectedPlatform != null;
		}
	}
}
