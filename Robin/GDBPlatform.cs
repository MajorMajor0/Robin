//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Robin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    
    public partial class GDBPlatform : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GDBPlatform()
        {
    		GDBReleases = new List<GDBRelease>();
        }
    
        private long _iD;
    	public long ID 
    	{ 
    		get { return _iD; } 
    		set { _iD = value; OnPropertyChanged("ID"); } 
    	}
    
        private string _title;
    	public string Title 
    	{ 
    		get { return _title; } 
    		set { _title = value; OnPropertyChanged("Title"); } 
    	}
    
        private Nullable<System.DateTime> _date;
    	public Nullable<System.DateTime> Date 
    	{ 
    		get { return _date; } 
    		set { _date = value; OnPropertyChanged("Date"); } 
    	}
    
        private string _developer;
    	public string Developer 
    	{ 
    		get { return _developer; } 
    		set { _developer = value; OnPropertyChanged("Developer"); } 
    	}
    
        private string _manufacturer;
    	public string Manufacturer 
    	{ 
    		get { return _manufacturer; } 
    		set { _manufacturer = value; OnPropertyChanged("Manufacturer"); } 
    	}
    
        private string _cpu;
    	public string Cpu 
    	{ 
    		get { return _cpu; } 
    		set { _cpu = value; OnPropertyChanged("Cpu"); } 
    	}
    
        private string _sound;
    	public string Sound 
    	{ 
    		get { return _sound; } 
    		set { _sound = value; OnPropertyChanged("Sound"); } 
    	}
    
        private string _display;
    	public string Display 
    	{ 
    		get { return _display; } 
    		set { _display = value; OnPropertyChanged("Display"); } 
    	}
    
        private string _media;
    	public string Media 
    	{ 
    		get { return _media; } 
    		set { _media = value; OnPropertyChanged("Media"); } 
    	}
    
        private string _controllers;
    	public string Controllers 
    	{ 
    		get { return _controllers; } 
    		set { _controllers = value; OnPropertyChanged("Controllers"); } 
    	}
    
        private Nullable<decimal> _rating;
    	public Nullable<decimal> Rating 
    	{ 
    		get { return _rating; } 
    		set { _rating = value; OnPropertyChanged("Rating"); } 
    	}
    
        private string _overview;
    	public string Overview 
    	{ 
    		get { return _overview; } 
    		set { _overview = value; OnPropertyChanged("Overview"); } 
    	}
    
        private string _boxFrontURL;
    	public string BoxFrontURL 
    	{ 
    		get { return _boxFrontURL; } 
    		set { _boxFrontURL = value; OnPropertyChanged("BoxFrontURL"); } 
    	}
    
        private string _boxBackURL;
    	public string BoxBackURL 
    	{ 
    		get { return _boxBackURL; } 
    		set { _boxBackURL = value; OnPropertyChanged("BoxBackURL"); } 
    	}
    
        private string _bannerURL;
    	public string BannerURL 
    	{ 
    		get { return _bannerURL; } 
    		set { _bannerURL = value; OnPropertyChanged("BannerURL"); } 
    	}
    
        private string _consoleURL;
    	public string ConsoleURL 
    	{ 
    		get { return _consoleURL; } 
    		set { _consoleURL = value; OnPropertyChanged("ConsoleURL"); } 
    	}
    
        private string _controllerURL;
    	public string ControllerURL 
    	{ 
    		get { return _controllerURL; } 
    		set { _controllerURL = value; OnPropertyChanged("ControllerURL"); } 
    	}
    
        private System.DateTime _cacheDate;
    	public System.DateTime CacheDate 
    	{ 
    		get { return _cacheDate; } 
    		set { _cacheDate = value; OnPropertyChanged("CacheDate"); } 
    	}
    
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<GDBRelease> GDBReleases { get; set; }
     
        public event PropertyChangedEventHandler PropertyChanged;
    
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    	
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                WhenPropertyChanged(e);
                PropertyChanged(this, e);
            }
        }
    
        partial void WhenPropertyChanged(PropertyChangedEventArgs e);
        
    }
}
