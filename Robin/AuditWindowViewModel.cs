using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
	public class AuditWindowViewModel : INotifyPropertyChanged
	{
		List<Mame.MAME.AuditResult> auditResults;
		public List<Mame.MAME.AuditResult> AuditResults
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

		public List<Mame.MAME.AuditResult> GoodResults => AuditResults?.Where(x => x.Result.ToLower() == "good").ToList();

		public List<Mame.MAME.AuditResult> BadResults => AuditResults?.Where(x => x.Result.ToLower() == "bad").ToList();

		public List<Mame.MAME.AuditResult> BestResults => AuditResults?.Where(x => x.Result.ToLower() == "best").ToList();

		public int GoodCount => AuditResults?.Count(x => x.Result.ToLower() == "good") ?? 0;

		public int BadCount => AuditResults?.Count(x => x.Result.ToLower() == "bad") ?? 0;

		public int BestCount => AuditResults?.Count(x => x.Result.ToLower() == "best") ?? 0;

		public AuditWindowViewModel()
		{
			GetAuditResults();
		}

		async void GetAuditResults()
		{
			await Task.Run(() => { AuditResults = Mame.MAME.AuditRoms(); });
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
