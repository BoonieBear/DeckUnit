using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BoonieBear.DeckUnit.Models
{
    
    public class SystemInfo:INotifyPropertyChanged
    {
        private string _name;

        private double _size = 0;
        public double Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                }
            }

        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}