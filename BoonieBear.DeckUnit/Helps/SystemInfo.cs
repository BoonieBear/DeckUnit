using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BoonieBear.DeckUnit.Helps
{
    public class SeriesData
    {
        private string _seriesDisplayName;

        public string SeriesDisplayName
        {
            get { return _seriesDisplayName; }
            set
            {
                _seriesDisplayName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SeriesDisplayName"));
                }
            }
            
        }

        public string SeriesDescription { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<SystemInfo> Items { get; set; }
    }
    public class SystemInfo:INotifyPropertyChanged
    {
        private string _category;

        private double _number = 0;
        public double Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Number"));
                }
            }

        }

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Category"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}