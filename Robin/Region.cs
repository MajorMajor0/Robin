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
    
    public partial class Region : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Region()
        {
    		GBReleases = new List<GBRelease>();
    		Matches = new List<Match>();
    		OVGReleases = new List<OVGRelease>();
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
    
        private string _datomatic;
    	public string Datomatic 
    	{ 
    		get { return _datomatic; } 
    		set { _datomatic = value; OnPropertyChanged("Datomatic"); } 
    	}
    
        private Nullable<long> _iD_GB;
    	public Nullable<long> ID_GB 
    	{ 
    		get { return _iD_GB; } 
    		set { _iD_GB = value; OnPropertyChanged("ID_GB"); } 
    	}
    
        private string _title_GB;
    	public string Title_GB 
    	{ 
    		get { return _title_GB; } 
    		set { _title_GB = value; OnPropertyChanged("Title_GB"); } 
    	}
    
        private string _uNCode;
    	public string UNCode 
    	{ 
    		get { return _uNCode; } 
    		set { _uNCode = value; OnPropertyChanged("UNCode"); } 
    	}
    
        private Nullable<long> _priority;
    	public Nullable<long> Priority 
    	{ 
    		get { return _priority; } 
    		set { _priority = value; OnPropertyChanged("Priority"); } 
    	}
    
        private string _mame;
    	public string Mame 
    	{ 
    		get { return _mame; } 
    		set { _mame = value; OnPropertyChanged("Mame"); } 
    	}
    
        private string _launchbox;
    	public string Launchbox 
    	{ 
    		get { return _launchbox; } 
    		set { _launchbox = value; OnPropertyChanged("Launchbox"); } 
    	}
    
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<GBRelease> GBReleases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Match> Matches { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<OVGRelease> OVGReleases { get; set; }
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
