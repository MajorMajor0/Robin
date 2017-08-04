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

namespace Robin
{
    // Base class for view model
    class AutoFilterCollection<T> : INotifyPropertyChanged where T : IDBobject
    {
        public Type Type
        {
            get { return typeof(T); }
        }
        const string ALL = "*All";
#if DEBUG
        Stopwatch[] Watch = new Stopwatch[10];
#endif


        public List<StringFilter> StringFilters { get; set; }


        public List<BoolFilter> BoolFilters { get; set; }

        private string textFilter;
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

        public AutoFilterCollection(List<T> sourceCollection)
        {
#if DEBUG
            for (int i = 0; i < Watch.Count(); i++)
            {
                Watch[i] = new Stopwatch();
                Watch[i].Start();
            }
#endif

            BoolFilters = new List<BoolFilter>();
            StringFilters = new List<StringFilter>();
            FilteredCollection = new List<T>();
            SourceCollection = sourceCollection;
            Update();
            Debug.WriteLine("New AutoFilterCollection");
        }

        // The data to be put into the viewmodel--releases, games, platforms
        List<T> sourceCollection;
        public List<T> SourceCollection
        {
            get { return sourceCollection; }
            set
            {
                Debug.WriteLine("Set SourceCollection");
                if (sourceCollection != value)
                {
                    sourceCollection = value;
                    CalculateStringFilters();
                    CalculateBoolFilters();
                }
            }
        }

        public List<T> FilteredCollection { get; set; }

        public int HasArtCount
        {
            get { return FilteredCollection.Where(x => x.HasArt).Count(); }
        }

        // Determine list of string filter columns--one time only
        void CalculateStringFilters()
        {
#if DEBUG
            Watch[0].Restart();
#endif
            switch (Type.Name)
            {
                case "Game":
                    StringFilters.Add(new StringFilter("Publisher", () => Update()));
                    StringFilters.Add(new StringFilter("Genres", () => Update()));
                    StringFilters.Add(new StringFilter("Players", () => Update()));
                    StringFilters.Add(new StringFilter("Platform", () => Update()));
                    StringFilters.Add(new StringFilter("Regions", () => Update()));
                    break;

                case "Release":
                    StringFilters.Add(new StringFilter("PlatformTitle", () => Update(), "Platform"));
                    break;

                case "Platform":
                    StringFilters.Add(new StringFilter("Type", () => Update()));
                    StringFilters.Add(new StringFilter("Generation", () => Update()));
                    StringFilters.Add(new StringFilter("Developer", () => Update()));
                    StringFilters.Add(new StringFilter("Manufacturer", () => Update()));
                    StringFilters.Add(new StringFilter("Media", () => Update()));
                    break;
                default:
                    break;
            }
#if DEBUG
            Debug.WriteLine("CalculateStringFilters (1x): " + Watch[0].ElapsedMilliseconds);
#endif
        }

        // Determine list of bool filters--one time only
        void CalculateBoolFilters()
        {
#if DEBUG
            Watch[1].Restart();
#endif
            switch (Type.Name)
            {
                case "Game":
                    BoolFilters.Add(new BoolFilter("IsCrap", () => Update(), "Crap"));
                    BoolFilters.Add(new BoolFilter("Included", () => Update(), "Playable"));
                    BoolFilters.Add(new BoolFilter("Unlicensed", () => Update()));
                    break;

                case "Release":
                    BoolFilters.Add(new BoolFilter("IsCrap", () => Update(), "Crap"));
                    BoolFilters.Add(new BoolFilter("Included", () => Update(), "Playable"));
                    BoolFilters.Add(new BoolFilter("Unlicensed", () => Update()));
                    BoolFilters.Add(new BoolFilter("HasArt", () => Update()));
                    break;

                case "Platform":
                    BoolFilters.Add(new BoolFilter("Included", () => Update(), "Playable"));
                    BoolFilters.Add(new BoolFilter("Preferred", () => Update()));
                    break;
                default:
                    break;
            }
#if DEBUG
            Debug.WriteLine("CalculateBoolFilters: " + Watch[1].ElapsedMilliseconds);
#endif
        }

        // Populate stringfilters
        public void Update(bool freeze = false)
        {
#if DEBUG
            Watch[3].Restart();
            Watch[1].Restart();
#endif
            CalculateFilteredCollection();
#if DEBUG
            Debug.WriteLine("CalculateFilteredCollection: " + Watch[1].ElapsedMilliseconds); Watch[1].Restart();
#endif

            foreach (var stringFilter in StringFilters)
            {
#if DEBUG
                Watch[0].Restart();
#endif
                GetDistinctValuesForFilter(stringFilter);
#if DEBUG
                Debug.WriteLine("Distinct values for " + stringFilter.Property + ": " + Watch[0].ElapsedMilliseconds); Watch[0].Restart();
#endif
            }

            OnPropertyChanged("FilteredCollection");
#if DEBUG
            Debug.WriteLine("Update " + typeof(T).Name + ": " + Watch[3].ElapsedMilliseconds + "\n\n");
#endif
        }

        // Figure out distinct values for an AutoFilteredColumn--many times
        public void GetDistinctValuesForFilter(StringFilter stringFilter)
        {
#if DEBUG
            Watch[2].Restart();
#endif
            switch (stringFilter.Property)
            {
                case "Regions":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Game>).SelectMany(x => x.RegionsList).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Genres":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Game>).SelectMany(x => x.GenreList).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Publisher":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Game>).Select(x => x.Publisher).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Players":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Game>).Select(x => x.Players).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Platform":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Game>).Select(x => x.Platform).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;

                case "PlatformTitle":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Release>).Select(x => x.PlatformTitle).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;

                case "Type":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Platform>).Select(x => x.Type).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Generation":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Platform>).Select(x => x.Generation).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Developer":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Platform>).Select(x => x.Developer).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Manufacturer":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Platform>).Select(x => x.Manufacturer).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                case "Media":
                    stringFilter.DistinctValues = (FilteredCollection as IEnumerable<Platform>).Select(x => x.Media).Distinct().Concat(new List<string>() { ALL }).OrderBy(x => x);
                    break;
                default:
                    break;
            }
#if DEBUG
            Debug.WriteLine(stringFilter.Property + "DistinctValues: " + Watch[2].ElapsedMilliseconds);
#endif
        }

        internal void ClearFilters()
        {
            foreach (StringFilter stringFilter in StringFilters)
            {
                stringFilter.Value = null;
            }

            foreach (BoolFilter boolFilter in BoolFilters)
            {
                boolFilter.Value = null;
            }

            TextFilter = "";

            FilteredCollection = sourceCollection;

            OnPropertyChanged("FilteredCollection");
        }

        public void CalculateFilteredCollection()
        {
            //// Start with all of the games
            //FilteredCollection = SourceCollection;

            IEnumerable<T> filteredCollection = SourceCollection;

            //// Filter items based on text
            if (!string.IsNullOrEmpty(TextFilter))
            {
                filteredCollection = filteredCollection.Where(x => Regex.IsMatch(x.Title, TextFilter.Replace(@"*", @".*"), RegexOptions.IgnoreCase));
                //filteredCollection = filteredCollection.Where(x => x.Title.ToLower().Contains(TextFilter.ToLower()));
            }

            // Run over string filters 
            foreach (StringFilter stringfilter in StringFilters)
            {
                if (stringfilter.Value != null && !stringfilter.Value.Equals(ALL))
                {
                    var prop = Type.GetProperty(stringfilter.Property);
                    if (stringfilter.Property == "Regions" || stringfilter.Property == "Genres")
                    {
                        filteredCollection = filteredCollection.Where(x => (prop.GetValue(x, null) != null && prop.GetValue(x, null).ToString().Contains(stringfilter.Value.ToString())));
                    }
                    else
                    {
                        filteredCollection = filteredCollection.Where(x => (prop.GetValue(x, null) != null && prop.GetValue(x, null).ToString().Equals(stringfilter.Value.ToString())));
                    }
                }
            }

            // Run over bool filters 
            foreach (var boolFilter in BoolFilters)
            {
                if (boolFilter.Value != null)
                {
                    switch (boolFilter.Property)
                    {
                        case "IsCrap":
                            filteredCollection = filteredCollection.Where(x => x.IsCrap == boolFilter.Value);
                            break;
                        case "Included":
                            filteredCollection = filteredCollection.Where(x => x.Included == boolFilter.Value);
                            break;
                        case "Unlicensed":
                            filteredCollection = filteredCollection.Where(x => x.Unlicensed == boolFilter.Value);
                            break;
                        case "HasArt":
                            filteredCollection = filteredCollection.Where(x => x.HasArt == boolFilter.Value);
                            break;
                        case "Preferred":
                            filteredCollection = filteredCollection.Where(x => x.Preferred == boolFilter.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            FilteredCollection = filteredCollection.ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop)
        {
#if DEBUG
            Watch[9].Restart();
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
            Debug.WriteLine("OnPropertyChanged " + prop + ": " + Watch[9].ElapsedMilliseconds + "\n");
#endif
        }
    }
}

// This is one filter and all of it's distinct values, e.g., Region, containing Japan, USA etc.
class StringFilter : INotifyPropertyChanged
{
    const string ALL = "*All";
#if DEBUG
    Stopwatch[] Watch = new Stopwatch[10];
#endif

    Action Callback;

    public string Name { get; set; }
    public string Property { get; set; }

    string _value = null;
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
            }
        }
    }

    private IEnumerable<string> distinctValues;
    public IEnumerable<string> DistinctValues
    {
        get { return distinctValues; }
        set
        {
            if (distinctValues != value)
            {
                distinctValues = value;
                OnPropertyChanged("DistinctValues");
            }
        }
    }

    public StringFilter(string property, Action callback, string name = null)
    {
#if DEBUG
        for (int i = 0; i < Watch.Count(); i++)
        {
            Watch[i] = new Stopwatch();
            Watch[i].Start();
        }
#endif
        DistinctValues = new List<string>();
        Property = property;
        Name = name ?? property;
        Value = null;
        Callback = callback;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop)
    {
#if DEBUG
        Watch[0].Restart();
#endif
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
        Debug.WriteLine("OnPropertyChanged " + Property + " " + prop + ": " + Watch[0].ElapsedMilliseconds + "\n");
#endif
    }
}

// This is one filter and all of it's distinct values, e.g., Region, containing Japan, USA etc.
class BoolFilter : INotifyPropertyChanged
{
    // The parent is the autofilter collection, roms, games, platforms
    public string Name { get; set; }
    public string Property { get; set; }
    Action Callback;

    bool? _value = null;
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

    public BoolFilter(string property, Action callback, string name = null)
    {
        Property = property;
        Name = name ?? property;

        Callback = callback;
        if (property == "Included")
        {
            Value = true;
        }

        if (property == "IsCrap")
        {
            Value = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
        Debug.WriteLine("OnPropertyChanged " + Property + " " + prop + "\n");
#endif
    }
}

