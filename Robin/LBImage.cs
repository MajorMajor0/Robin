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
    
    public partial class LBImage : INotifyPropertyChanged
    {
        private long _iD;
    	public long ID 
    	{ 
    		get { return _iD; } 
    		set { _iD = value; OnPropertyChanged("ID"); } 
    	}
    
        private string _type;
    	public string Type 
    	{ 
    		get { return _type; } 
    		set { _type = value; OnPropertyChanged("Type"); } 
    	}
    
        private string _fileName;
    	public string FileName 
    	{ 
    		get { return _fileName; } 
    		set 
    		{
    			_fileName = value;
    			OnPropertyChanged("FileName");
    			OnPropertyChanged("FilePath");
    		} 
    	}
    
        private string _lBRegion;
    	public string LBRegion 
    	{ 
    		get { return _lBRegion; } 
    		set { _lBRegion = value; OnPropertyChanged("LBRegion"); } 
    	}
    
        private long _region_ID;
    	public long Region_ID 
    	{ 
    		get { return _region_ID; } 
    		set { _region_ID = value; OnPropertyChanged("Region_ID"); } 
    	}
    
        private Nullable<long> _lBRelease_ID;
    	public Nullable<long> LBRelease_ID 
    	{ 
    		get { return _lBRelease_ID; } 
    		set { _lBRelease_ID = value; OnPropertyChanged("LBRelease_ID"); } 
    	}
    
    
        public virtual LBRelease LBRelease { get; set; }
     
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
