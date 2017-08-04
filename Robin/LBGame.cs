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
    
    public partial class LBGame : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LBGame()
        {
    		LBImages = new List<LBImage>();
    		LBReleases = new List<LBRelease>();
    		Releases = new List<Release>();
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
    
        private string _overview;
    	public string Overview 
    	{ 
    		get { return _overview; } 
    		set { _overview = value; OnPropertyChanged("Overview"); } 
    	}
    
        private long _lBPlatform_ID;
    	public long LBPlatform_ID 
    	{ 
    		get { return _lBPlatform_ID; } 
    		set { _lBPlatform_ID = value; OnPropertyChanged("LBPlatform_ID"); } 
    	}
    
        private string _genres;
    	public string Genres 
    	{ 
    		get { return _genres; } 
    		set { _genres = value; OnPropertyChanged("Genres"); } 
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
    
        private string _videoURL;
    	public string VideoURL 
    	{ 
    		get { return _videoURL; } 
    		set { _videoURL = value; OnPropertyChanged("VideoURL"); } 
    	}
    
        private string _wikiURL;
    	public string WikiURL 
    	{ 
    		get { return _wikiURL; } 
    		set { _wikiURL = value; OnPropertyChanged("WikiURL"); } 
    	}
    
        private string _players;
    	public string Players 
    	{ 
    		get { return _players; } 
    		set { _players = value; OnPropertyChanged("Players"); } 
    	}
    
    
        public virtual LBPlatform LBPlatform { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<LBImage> LBImages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<LBRelease> LBReleases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Release> Releases { get; set; }
     
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
