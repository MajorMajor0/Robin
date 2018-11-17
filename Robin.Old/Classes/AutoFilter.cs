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
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using Robin.Core.Properties;
using System.Collections.ObjectModel;

namespace Robin.Core
{
	// Base class for view model
	public class AutoFilterCollection : INotifyPropertyChanged
	{
		bool freezeUpdate = false;

		protected string title;
		public string Title => title;

		protected const string ALL = "*All";

		public List<StringFilter> StringFilters { get; }

		public List<BoolFilter> BoolFilters { get; }

		protected string textFilter;
		public string TextFilter
		{
			get { return textFilter; }
			set
			{
				if (value != textFilter)
				{
					textFilter = value;
					if (textFilter.Length != 1)
					{
						Update();
						OnPropertyChanged("TextFilter");
					}
				}
			}
		}

		public virtual IList FilteredCollection { get; }

		public virtual int SourceCount { get; }

		internal Settings Settings => Properties.Settings.Default;

		public AutoFilterCollection()
		{
			BoolFilters = new List<BoolFilter>();
			StringFilters = new List<StringFilter>();
			ClearTextFilterCommand = new Command(ClearTextFilter, ClearTextFilterCanExecute, "X", "Clear filter text.");
		}


		public void Update()
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			if (!freezeUpdate)
			{
				CalculateFilteredCollection();
				GetDistinctValuesForFilter();
			}
#if DEBUG
			Debug.WriteLine("Update: " + Watch.ElapsedMilliseconds);
#endif
		}

		public void ClearFilters()
		{
			freezeUpdate = true;

			TextFilter = "";

			foreach (StringFilter stringFilter in StringFilters)
			{
				stringFilter.Value = null;
			}

			foreach (BoolFilter boolFilter in BoolFilters)
			{
				boolFilter.Value = null;
			}

			freezeUpdate = false;
			Update();
		}

		public virtual void SaveSettings()
		{

		}

		protected virtual void GetDistinctValuesForFilter()
		{
		}

		protected virtual void CalculateFilteredCollection()
		{
		}


		public Command ClearTextFilterCommand { get; set; }

		private void ClearTextFilter()
		{
			TextFilter = "";
		}

		private bool ClearTextFilterCanExecute()
		{
			return !string.IsNullOrEmpty(TextFilter);
		}


		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string prop)
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
			Debug.WriteLine("OnPropertyChanged " + prop + ": " + Watch.ElapsedMilliseconds + "\n");
#endif
		}
	}

	public class AutoFilterReleases : AutoFilterCollection
	{
		List<Release> sourceCollection;

		List<Release> filteredCollection;

		public override int SourceCount => sourceCollection.Count;

		public override IList FilteredCollection => filteredCollection;

		public StringFilter PublisherFilter { get; }
		public StringFilter GenreFilter { get; }
		public StringFilter PlayersFilter { get; }
		public StringFilter PlatformFilter { get; }
		public StringFilter RegionsFilter { get; }
		public StringFilter YearFilter { get; }

		public BoolFilter UnlicensedFilter { get; }
		public BoolFilter BeatenFilter { get; }
		public BoolFilter CrapFilter { get; }
		public BoolFilter IncludedFilter { get; }
		public BoolFilter IsGameFilter { get; }
		public BoolFilter AdultFilter { get; }

		public AutoFilterReleases(List<Release> _sourceCollection, string _title)
		{
			title = _title;
			sourceCollection = _sourceCollection.OrderBy(x => x.Platform.ID).ThenBy(x => x.Title).ToList(); ;

			StringFilters.Add(PublisherFilter = new StringFilter("Publisher", () => Update(), Settings.ReleaseFilterPublisher));
			StringFilters.Add(GenreFilter = new StringFilter("Genre", () => Update(), Settings.ReleaseFilterGenre));
			StringFilters.Add(PlayersFilter = new StringFilter("Players", () => Update(), Settings.ReleaseFilterPlayers));
			StringFilters.Add(PlatformFilter = new StringFilter("Platform", () => Update(), Settings.ReleaseFilterPlatform));
			StringFilters.Add(RegionsFilter = new StringFilter("Regions", () => Update(), Settings.ReleaseFilterRegions));
			StringFilters.Add(YearFilter = new StringFilter("Year", () => Update(), Settings.ReleaseFilterYear));

			BoolFilters.Add(UnlicensedFilter = new BoolFilter("Unlicensed", () => Update(), Settings.ReleaseFilterUnlicensed));
			BoolFilters.Add(BeatenFilter = new BoolFilter("Beaten", () => Update(), Settings.ReleaseFilterIsBeaten));

			if (Settings.DisplayCrap)
			{
				BoolFilters.Add(CrapFilter = new BoolFilter("Crap", () => Update(), Settings.ReleaseFilterIsCrap));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => !x.IsCrap).ToList();
			}

			if (Settings.DisplayNotIncluded)
			{
				BoolFilters.Add(IncludedFilter = new BoolFilter("Playable", () => Update(), Settings.ReleaseFilterIncluded));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => x.Included).ToList();
			}

			if (Settings.DisplayNonGames)
			{
				BoolFilters.Add(IsGameFilter = new BoolFilter("Is Game", () => Update(), Settings.ReleaseFilterIsGame));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => x.IsGame).ToList();
			}

			if (Settings.DisplayAdult)
			{
				BoolFilters.Add(AdultFilter = new BoolFilter("Adult", () => Update(), Settings.ReleaseFilterAdult));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => !x.IsAdult).ToList();
			}

			Update();
		}

		protected override void CalculateFilteredCollection()
		{
			IEnumerable<Release> _filteredCollection = sourceCollection;
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			//// Filter items based on text
			if (!string.IsNullOrEmpty(TextFilter))
			{
				_filteredCollection = _filteredCollection.Where(x => Regex.IsMatch(x.Title, TextFilter.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}

			if (PublisherFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Publisher == PublisherFilter.Value);
			}

			if (GenreFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Genre != null && x.Genre.Contains(GenreFilter.Value));
			}

			if (PlayersFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Players == PlayersFilter.Value);
			}

			if (PlatformFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.PlatformTitle == PlatformFilter.Value);
			}

			if (RegionsFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Region.Title.Contains(RegionsFilter.Value));
			}

			if (YearFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Year == YearFilter.Value);
			}

			if (UnlicensedFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Unlicensed == UnlicensedFilter.Value);
			}

			if (BeatenFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsBeaten == BeatenFilter.Value);
			}

			if (Settings.DisplayCrap && CrapFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsCrap == CrapFilter.Value);
			}

			if (Settings.DisplayNotIncluded && IncludedFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Included == IncludedFilter.Value);
			}

			if (Settings.DisplayNonGames && IsGameFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsGame == IsGameFilter.Value);
			}

			if (Settings.DisplayAdult && AdultFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsAdult == AdultFilter.Value);
			}

			filteredCollection = _filteredCollection.ToList();

#if DEBUG
			Debug.WriteLine("Calculate filtered collection: " + Watch.ElapsedMilliseconds);
#endif
			OnPropertyChanged("FilteredCollection");
		}

		protected override void GetDistinctValuesForFilter()
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			PublisherFilter.DistinctValues = filteredCollection.Select(x => x.Publisher);
			GenreFilter.DistinctValues = filteredCollection.SelectMany(x => x.GenreList);
			PlayersFilter.DistinctValues = filteredCollection.Select(x => x.Players);
			PlatformFilter.DistinctValues = filteredCollection.Select(x => x.PlatformTitle);
			RegionsFilter.DistinctValues = filteredCollection.Select(x => x.RegionTitle);
			YearFilter.DistinctValues = filteredCollection.Select(x => x.Year).Distinct();
#if DEBUG
			Debug.WriteLine("Get distinct values: " + Watch.ElapsedMilliseconds);
#endif
		}

		public override void SaveSettings()
		{
			Settings.ReleaseFilterPublisher = PublisherFilter.Value;
			Settings.ReleaseFilterGenre = GenreFilter.Value;
			Settings.ReleaseFilterPlayers = PlayersFilter.Value;
			Settings.ReleaseFilterPlatform = PlatformFilter.Value;
			Settings.ReleaseFilterRegions = RegionsFilter.Value;
			Settings.ReleaseFilterYear = YearFilter.Value;

			Settings.ReleaseFilterIncluded = IncludedFilter?.Value;
			Settings.ReleaseFilterIsCrap = CrapFilter?.Value;
			Settings.ReleaseFilterUnlicensed = UnlicensedFilter.Value;
			Settings.ReleaseFilterIsBeaten = BeatenFilter.Value;
			Settings.ReleaseFilterAdult = AdultFilter?.Value;
			Settings.ReleaseFilterIsGame = IsGameFilter?.Value;
		}
	}

	public class AutoFilterGames : AutoFilterCollection
	{
		List<Game> sourceCollection;

		List<Game> filteredCollection;

		public override int SourceCount => sourceCollection.Count;

		public override IList FilteredCollection => filteredCollection;

		public StringFilter PublisherFilter { get; }
		public StringFilter GenreFilter { get; }
		public StringFilter PlayersFilter { get; }
		public StringFilter PlatformFilter { get; }
		public StringFilter RegionsFilter { get; }
		public StringFilter YearFilter { get; }

		public BoolFilter UnlicensedFilter { get; }
		public BoolFilter BeatenFilter { get; }
		public BoolFilter CrapFilter { get; }
		public BoolFilter IncludedFilter { get; }
		public BoolFilter IsGameFilter { get; }
		public BoolFilter AdultFilter { get; }
		public BoolFilter MessFilter { get; }

		public AutoFilterGames(List<Game> _sourceCollection, string _title)
		{
			title = _title;

			if (Settings.SortGamesRandom)
			{
				sourceCollection = _sourceCollection;
				sourceCollection.Shuffle();
			}

			else
			{
				sourceCollection = _sourceCollection.OrderBy(x=>x.Platform_ID).ThenBy(x=>x.Title).ToList();
			}

			StringFilters.Add(PublisherFilter = new StringFilter("Publisher", () => Update(), Settings.GameFilterPublisher));
			StringFilters.Add(GenreFilter = new StringFilter("Genre", () => Update(), Settings.GameFilterGenre));
			StringFilters.Add(PlayersFilter = new StringFilter("Players", () => Update(), Settings.GameFilterPlayers));
			StringFilters.Add(PlatformFilter = new StringFilter("Platform", () => Update(), Settings.GameFilterPlatform));
			StringFilters.Add(RegionsFilter = new StringFilter("Regions", () => Update(), Settings.GameFilterRegions));
			StringFilters.Add(YearFilter = new StringFilter("Year", () => Update(), Settings.GameFilterYear));

			BoolFilters.Add(UnlicensedFilter = new BoolFilter("Unlicensed", () => Update(), Settings.ReleaseFilterUnlicensed));
			BoolFilters.Add(BeatenFilter = new BoolFilter("Beaten", () => Update(), Settings.GameFilterIsBeaten));

			if (Settings.DisplayCrap)
			{
				BoolFilters.Add(CrapFilter = new BoolFilter("Crap", () => Update(), Settings.GameFilterIsCrap));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => !x.IsCrap).ToList();
			}

			if (Settings.DisplayNotIncluded)
			{
				BoolFilters.Add(IncludedFilter = new BoolFilter("Playable", () => Update(), Settings.GameFilterIncluded));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => x.Included).ToList();
			}

			if (Settings.DisplayNonGames)
			{
				BoolFilters.Add(IsGameFilter = new BoolFilter("Is Game", () => Update(), Settings.GameFilterIsGame));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => x.IsGame).ToList();
			}

			if (Settings.DisplayAdult)
			{
				BoolFilters.Add(AdultFilter = new BoolFilter("Adult", () => Update(), Settings.GameFilterAdult));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => !x.IsAdult).ToList();
			}

			if (Settings.DisplayMess)
			{
				BoolFilters.Add(MessFilter = new BoolFilter("Mess", () => Update(), Settings.GameFilterIsMess));
			}
			else
			{
				sourceCollection = sourceCollection.Where(x => !x.IsAdult).ToList();
			}
			Update();
		}

		protected override void CalculateFilteredCollection()
		{
			IEnumerable<Game> _filteredCollection = sourceCollection;

#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif

			// String filters
			if (!string.IsNullOrEmpty(TextFilter))
			{
				_filteredCollection = _filteredCollection.Where(x => Regex.IsMatch(x.Title, TextFilter.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}

			if (PublisherFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Publisher == PublisherFilter.Value);
			}

			if (GenreFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Genre != null && x.Genre.Contains(GenreFilter.Value));
			}

			if (PlayersFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Players == PlayersFilter.Value);
			}

			if (PlatformFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.PlatformTitle == PlatformFilter.Value);
			}

			if (RegionsFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.RegionsList.Contains(RegionsFilter.Value));
			}

			if (YearFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Year == YearFilter.Value);
			}

			// Bool filters
			if (UnlicensedFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Unlicensed == UnlicensedFilter.Value);
			}

			if (BeatenFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsBeaten == BeatenFilter.Value);
			}

			if (Settings.DisplayCrap && CrapFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsCrap == CrapFilter.Value);
			}

			if (Settings.DisplayNotIncluded && IncludedFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.Included == IncludedFilter.Value);
			}

			if (Settings.DisplayNonGames && IsGameFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsGame == IsGameFilter.Value);
			}

			if (Settings.DisplayAdult && AdultFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsAdult == AdultFilter.Value);
			}

			if (Settings.DisplayMess && MessFilter.IsSet)
			{
				_filteredCollection = _filteredCollection.Where(x => x.IsMess == MessFilter.Value);
			}

			filteredCollection = _filteredCollection.ToList();

#if DEBUG
			Debug.WriteLine("Calculate filtered collection: " + Watch.ElapsedMilliseconds);
#endif
			OnPropertyChanged("FilteredCollection");
		}

		protected override void GetDistinctValuesForFilter()
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			PublisherFilter.DistinctValues = filteredCollection.Select(x => x.Publisher);
			GenreFilter.DistinctValues = filteredCollection.SelectMany(x => x.GenreList);
			PlayersFilter.DistinctValues = filteredCollection.Select(x => x.Players);
			PlatformFilter.DistinctValues = filteredCollection.Select(x => x.PlatformTitle);
			RegionsFilter.DistinctValues = filteredCollection.SelectMany(x => x.RegionsList);
			YearFilter.DistinctValues = filteredCollection.Select(x => x.Year).Distinct();
#if DEBUG
			Debug.WriteLine("Get distinct values: " + Watch.ElapsedMilliseconds);
#endif
		}

		public override void SaveSettings()
		{
			Settings.GameFilterPublisher = PublisherFilter.Value;
			Settings.GameFilterGenre = GenreFilter.Value;
			Settings.GameFilterPlayers = PlayersFilter.Value;
			Settings.GameFilterPlatform = PlatformFilter.Value;
			Settings.GameFilterRegions = RegionsFilter.Value;
			Settings.GameFilterYear = YearFilter.Value;

			Settings.GameFilterIncluded = IncludedFilter?.Value;
			Settings.GameFilterIsCrap = CrapFilter?.Value;
			Settings.GameFilterUnlicensed = UnlicensedFilter.Value;
			Settings.GameFilterIsBeaten = BeatenFilter.Value;
			Settings.GameFilterAdult = AdultFilter?.Value;
			Settings.GameFilterIsGame = IsGameFilter?.Value;
			Settings.GameFilterIsMess = MessFilter?.Value;
		}
	}

	public class AutoFilterPlatforms : AutoFilterCollection
	{
		List<Platform> sourceCollection;

		List<Platform> filteredCollection;

		public override int SourceCount => sourceCollection.Count;

		public override IList FilteredCollection => filteredCollection;

		public StringFilter TypeFilter { get; }
		public StringFilter GenerationFilter { get; }
		public StringFilter DeveloperFilter { get; }
		public StringFilter ManufacturerFilter { get; }
		public StringFilter MediaFilter { get; }

		public BoolFilter IncludedFilter { get; }
		public BoolFilter PreferreddFilter { get; }

		public AutoFilterPlatforms(List<Platform> _sourceCollection, string _title)
		{
			title = _title;
			sourceCollection = _sourceCollection;

			StringFilters.Add(TypeFilter = new StringFilter("Type", () => Update(), Settings.PlatformFilterType));
			StringFilters.Add(GenerationFilter = new StringFilter("Generation", () => Update(), Settings.PlatformFilterGeneration));
			StringFilters.Add(DeveloperFilter = new StringFilter("Developer", () => Update(), Settings.PlatformFilterDeveloper));
			StringFilters.Add(ManufacturerFilter = new StringFilter("Manufacturer", () => Update(), Settings.PlatformFilterManufacturer));
			StringFilters.Add(MediaFilter = new StringFilter("Media", () => Update(), Settings.PlatformFilterMedia));

			BoolFilters.Add(IncludedFilter = new BoolFilter("Playable", () => Update(), Settings.PlatformFilterIncluded));
			BoolFilters.Add(PreferreddFilter = new BoolFilter("Preferred", () => Update(), Settings.PlatformFilterPreferred));

			Update();
		}

		protected override void CalculateFilteredCollection()
		{
			IEnumerable<Platform> _filteredCollection = sourceCollection;

#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif

			//// Filter items based on text
			if (!string.IsNullOrEmpty(TextFilter))
			{
				_filteredCollection = _filteredCollection.Where(x => Regex.IsMatch(x.Title, TextFilter.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}

			filteredCollection = _filteredCollection
				.Where(x => !TypeFilter.IsSet || x.Type == TypeFilter.Value)
				.Where(x => !GenerationFilter.IsSet || x.Generation == GenerationFilter.Value)
				.Where(x => !DeveloperFilter.IsSet || x.Developer != null && x.Developer.Contains(DeveloperFilter.Value))
				.Where(x => !ManufacturerFilter.IsSet || x.Manufacturer != null && x.Manufacturer.Contains(ManufacturerFilter.Value))
				.Where(x => !MediaFilter.IsSet || x.Media != null && x.Media.Contains(MediaFilter.Value))

				.Where(x => !IncludedFilter.IsSet || x.Included == IncludedFilter.Value)
				.Where(x => !PreferreddFilter.IsSet || x.Unlicensed == PreferreddFilter.Value)

				.ToList();

#if DEBUG
			Debug.WriteLine("Calculate filtered collection: " + Watch.ElapsedMilliseconds);
#endif
			OnPropertyChanged("FilteredCollection");
		}

		protected override void GetDistinctValuesForFilter()
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			TypeFilter.DistinctValues = filteredCollection.Select(x => x.Type);
			GenerationFilter.DistinctValues = filteredCollection.Select(x => x.Generation);
			DeveloperFilter.DistinctValues = filteredCollection.Select(x => x.Developer);
			ManufacturerFilter.DistinctValues = filteredCollection.Select(x => x.Manufacturer);
			MediaFilter.DistinctValues = filteredCollection.Select(x => x.Media);
#if DEBUG
			Debug.WriteLine("Get distinct values: " + Watch.ElapsedMilliseconds);
#endif
		}

		public override void SaveSettings()
		{
			Settings.PlatformFilterType = TypeFilter.Value;
			Settings.PlatformFilterGeneration = GenerationFilter.Value;
			Settings.PlatformFilterDeveloper = DeveloperFilter.Value;
			Settings.PlatformFilterManufacturer = ManufacturerFilter.Value;
			Settings.PlatformFilterMedia = MediaFilter.Value;

			Settings.PlatformFilterIncluded = IncludedFilter.Value;
			Settings.PlatformFilterPreferred = PreferreddFilter.Value;
		}
	}

	public class AutoFilterEmulators : AutoFilterCollection
	{
		List<Emulator> sourceCollection;

		List<Emulator> filteredCollection;

		public override int SourceCount => sourceCollection.Count;

		public override IList FilteredCollection => filteredCollection;

		public StringFilter PlatformFilter { get; }

		public BoolFilter IncludedFilter { get; }

		public BoolFilter CrapFilter { get; }

		public AutoFilterEmulators(List<Emulator> _sourceCollection, string _title)
		{
			title = _title;
			sourceCollection = _sourceCollection;

			StringFilters.Add(PlatformFilter = new StringFilter("Platform", () => Update(), Settings.EmulatorFilterPlatform));

			BoolFilters.Add(IncludedFilter = new BoolFilter("Included", () => Update(), Settings.EmulatorFilterIncluded));
			BoolFilters.Add(CrapFilter = new BoolFilter("Crap", () => Update(), Settings.EmulatorFilterIsCrap));

			Update();
		}

		protected override void CalculateFilteredCollection()
		{
			IEnumerable<Emulator> _filteredCollection = sourceCollection;

#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif

			//// Filter items based on text
			if (!string.IsNullOrEmpty(TextFilter))
			{
				_filteredCollection = _filteredCollection.Where(x => Regex.IsMatch(x.Title, TextFilter.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
			}

			filteredCollection = _filteredCollection
				.Where(x => !PlatformFilter.IsSet || x.Platforms.Select(y => y.Title).Contains(PlatformFilter.Value))
				.Where(x => !IncludedFilter.IsSet || x.Included == IncludedFilter.Value)

				.ToList();

#if DEBUG
			Debug.WriteLine("Calculate filtered collection: " + Watch.ElapsedMilliseconds);
#endif
			OnPropertyChanged("FilteredCollection");
		}

		protected override void GetDistinctValuesForFilter()
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			PlatformFilter.DistinctValues = filteredCollection.SelectMany(x => x.Platforms).Select(x => x.Title);
#if DEBUG
			Debug.WriteLine("Get distinct values: " + Watch.ElapsedMilliseconds);
#endif
		}

		public override void SaveSettings()
		{
			Settings.EmulatorFilterPlatform = PlatformFilter.Value;
			Settings.EmulatorFilterIncluded = IncludedFilter.Value;
			Settings.EmulatorFilterIsCrap = CrapFilter.Value;
		}
	}


	// This is one filter and all of it's distinct values, e.g., Region, containing Japan, USA etc.
	public class StringFilter : INotifyPropertyChanged
	{
		Action Callback;

		public string Name { get; set; }

		string _value;
		public string Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;
					Callback();
					OnPropertyChanged("Value");
					OnPropertyChanged("IsSet");
				}
			}
		}

		public bool IsSet => !string.IsNullOrEmpty(_value);

		IEnumerable<string> distinctValues;
		public IEnumerable<string> DistinctValues
		{
			get { return distinctValues.Distinct().OrderBy(x => x); }
			set
			{
				if (distinctValues != value)
				{
					distinctValues = value;
					OnPropertyChanged("DistinctValues");
				}
			}
		}

		public StringFilter(string name, Action callback, string value = null)
		{
			DistinctValues = new List<string>();
			Callback = callback;
			Name = name;
			_value = value;

			ClearCommand = new Command(Clear, ClearCanExecute, "X", "Clear " + " filter.");
		}

		public Command ClearCommand { get; set; }

		private void Clear()
		{
			Value = null;
		}

		private bool ClearCanExecute()
		{
			return IsSet;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string prop)
		{
#if DEBUG
			Stopwatch Watch = Stopwatch.StartNew();
#endif
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
			Debug.WriteLine("OnPropertyChanged " + Name + " " + prop + ": " + Watch.ElapsedMilliseconds + "\n");
#endif
		}
	}

	// This is one filter and all of it's distinct values, e.g., Region, containing Japan, USA etc.
	public class BoolFilter : INotifyPropertyChanged
	{
		// The parent is the autofilter collection, roms, games, platforms
		public string Name { get; set; }

		Action Callback;

		public bool IsSet => _value != null;

		bool? _value;

		public bool? Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;
					Callback();
					OnPropertyChanged("Value");
				}
			}
		}

		public BoolFilter(string name, Action callback, bool? _value)
		{
			this._value = _value;
			Name = name;
			Callback = callback;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
			Debug.WriteLine("OnPropertyChanged " + Name + " " + prop + "\n");
#endif
		}
	}
}





