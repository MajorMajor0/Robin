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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Robin;

partial class DatabaseWindowViewModel : INotifyPropertyChanged
{
	public int Threshold { get; set; }

	public ObservableCollection<IDB> IDBs { get; set; }

	IDB selectedIDB;

	public IDB SelectedIDB
	{
		get => selectedIDB;
		set
		{
			if (value != selectedIDB)
			{
				selectedIDB = value;
				OnPropertyChanged("SelectedIDB");
			}
		}
	}

	public IEnumerable PlatformList => SelectedIDB?.Platforms;

	public IList SelectedPlatforms { get; set; }

	public IDbPlatform SelectedPlatform { get; set; }
	public IDbRelease SelectedRelease { get; set; }


	public ObservableCollection<Compares> ComparisonResults { get; set; }

	public Compares SelectedComparisonResult { get; set; }


	public DatabaseWindowViewModel()
	{
		Threshold = 0;

		IDBs = new ObservableCollection<IDB>();

		InitializeDBs();
		IntializeCommands();

		ComparisonResults = new TitledCollection<Compares>("Comparison Results");
	}


	async void InitializeDBs()
	{
		if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
		{
			//TODO: put some dummy components here for design time
		}

		// Trying to load this stuff at design time will throw an exception and crash the designer
		else
		{
			Datomatic datomatic = null;
			await Task.Run(() => { datomatic = new(); }).ConfigureAwait(true);
			IDBs.Add(datomatic);

			GamesDB gamesDB = null;
			await Task.Run(() => { gamesDB = new(); }).ConfigureAwait(true);
			IDBs.Add(gamesDB);

			GiantBomb giantBomb = null;
			await Task.Run(() => { giantBomb = new(); }).ConfigureAwait(true);
			IDBs.Add(giantBomb);

			OpenVGDB openVGDB = null;
			await Task.Run(() => { openVGDB = new(); }).ConfigureAwait(true);
			IDBs.Add(openVGDB);

			Launchbox launchBox = null;
			await Task.Run(() => { launchBox = new(); }).ConfigureAwait(true);
			IDBs.Add(launchBox);
		}
	}

	public void CopyData()
	{
		// TODO: Does this function even work? Is it even hooked-up to a button?
		Reporter.Report("Getting data...");
		foreach (Release release in R.Data.Releases)
		{
			release.CopyData();
		}
		Reporter.ReportInline("finished.");
	}


	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string name)
	{
		Debug.WriteLine(name);
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
