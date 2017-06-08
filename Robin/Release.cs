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
    
    public partial class Release : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Release()
        {
    		Collections = new List<Collection>();
        }
    
        private long _iD;
    	public long ID 
    	{ 
    		get { return _iD; } 
    		set { _iD = value; OnPropertyChanged("ID"); } 
    	}
    
        private Nullable<long> _iD_GB;
    	public Nullable<long> ID_GB 
    	{ 
    		get { return _iD_GB; } 
    		set { _iD_GB = value; OnPropertyChanged("ID_GB"); } 
    	}
    
        private Nullable<long> _iD_GDB;
    	public Nullable<long> ID_GDB 
    	{ 
    		get { return _iD_GDB; } 
    		set { _iD_GDB = value; OnPropertyChanged("ID_GDB"); } 
    	}
    
        private Nullable<long> _iD_OVG;
    	public Nullable<long> ID_OVG 
    	{ 
    		get { return _iD_OVG; } 
    		set { _iD_OVG = value; OnPropertyChanged("ID_OVG"); } 
    	}
    
        private Nullable<long> _iD_LB;
    	public Nullable<long> ID_LB 
    	{ 
    		get { return _iD_LB; } 
    		set { _iD_LB = value; OnPropertyChanged("ID_LB"); } 
    	}
    
        private Nullable<long> _game_ID;
    	public Nullable<long> Game_ID 
    	{ 
    		get { return _game_ID; } 
    		set { _game_ID = value; OnPropertyChanged("Game_ID"); } 
    	}
    
        private long _platform_ID;
    	public long Platform_ID 
    	{ 
    		get { return _platform_ID; } 
    		set { _platform_ID = value; OnPropertyChanged("Platform_ID"); } 
    	}
    
        private Nullable<long> _region_ID;
    	public Nullable<long> Region_ID 
    	{ 
    		get { return _region_ID; } 
    		set { _region_ID = value; OnPropertyChanged("Region_ID"); } 
    	}
    
        private string _special;
    	public string Special 
    	{ 
    		get { return _special; } 
    		set { _special = value; OnPropertyChanged("Special"); } 
    	}
    
        private Nullable<long> _rom_ID;
    	public Nullable<long> Rom_ID 
    	{ 
    		get { return _rom_ID; } 
    		set { _rom_ID = value; OnPropertyChanged("Rom_ID"); } 
    	}
    
        private string _title;
    	public string Title 
    	{ 
    		get { return _title; } 
    		set { _title = value; OnPropertyChanged("Title"); } 
    	}
    
        private bool _isGame;
    	public bool IsGame 
    	{ 
    		get { return _isGame; } 
    		set { _isGame = value; OnPropertyChanged("IsGame"); } 
    	}
    
        private string _overview;
    	public string Overview 
    	{ 
    		get { return _overview; } 
    		set { _overview = value; OnPropertyChanged("Overview"); } 
    	}
    
        private string _developer;
    	public string Developer 
    	{ 
    		get { return _developer; } 
    		set { _developer = value; OnPropertyChanged("Developer"); } 
    	}
    
        private string _publisher;
    	public string Publisher 
    	{ 
    		get { return _publisher; } 
    		set { _publisher = value; OnPropertyChanged("Publisher"); } 
    	}
    
        private string _genre;
    	public string Genre 
    	{ 
    		get { return _genre; } 
    		set { _genre = value; OnPropertyChanged("Genre"); } 
    	}
    
        private Nullable<System.DateTime> _date;
    	public Nullable<System.DateTime> Date 
    	{ 
    		get { return _date; } 
    		set { _date = value; OnPropertyChanged("Date"); } 
    	}
    
        private bool _unlicensed;
    	public bool Unlicensed 
    	{ 
    		get { return _unlicensed; } 
    		set { _unlicensed = value; OnPropertyChanged("Unlicensed"); } 
    	}
    
        private string _language;
    	public string Language 
    	{ 
    		get { return _language; } 
    		set { _language = value; OnPropertyChanged("Language"); } 
    	}
    
        private string _videoFormat;
    	public string VideoFormat 
    	{ 
    		get { return _videoFormat; } 
    		set { _videoFormat = value; OnPropertyChanged("VideoFormat"); } 
    	}
    
        private string _version;
    	public string Version 
    	{ 
    		get { return _version; } 
    		set { _version = value; OnPropertyChanged("Version"); } 
    	}
    
        private string _players;
    	public string Players 
    	{ 
    		get { return _players; } 
    		set { _players = value; OnPropertyChanged("Players"); } 
    	}
    
        private Nullable<decimal> _rating;
    	public Nullable<decimal> Rating 
    	{ 
    		get { return _rating; } 
    		set { _rating = value; OnPropertyChanged("Rating"); } 
    	}
    
        private bool _isCrap;
    	public bool IsCrap 
    	{ 
    		get { return _isCrap; } 
    		set { _isCrap = value; OnPropertyChanged("IsCrap"); } 
    	}
    
        private bool _preferred;
    	public bool Preferred 
    	{ 
    		get { return _preferred; } 
    		set { _preferred = value; OnPropertyChanged("Preferred"); } 
    	}
    
        private bool _isBeaten;
    	public bool IsBeaten 
    	{ 
    		get { return _isBeaten; } 
    		set { _isBeaten = value; OnPropertyChanged("IsBeaten"); } 
    	}
    
    
        public virtual Game Game { get; set; }
        public virtual GBRelease GBRelease { get; set; }
        public virtual GDBRelease GDBRelease { get; set; }
        public virtual LBGame LBGame { get; set; }
        public virtual OVGRelease OVGRelease { get; set; }
        public virtual Platform Platform { get; set; }
        public virtual Region Region { get; set; }
        public virtual Rom Rom { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Collection> Collections { get; set; }
     
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
