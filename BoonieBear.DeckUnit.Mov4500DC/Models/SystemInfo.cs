using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BoonieBear.DeckUnit.Mov4500UI.Models
{
    
    public class CollectionInfo:INotifyPropertyChanged
    {
        
        private string _category;

        private float _number = 0;
        public float Number
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
    public class AdcpInfo : INotifyPropertyChanged
    {

        public AdcpInfo(int cat, sbyte value)
        {
            _category = cat;
            _number = value;
        }
        private int _category;

        private sbyte _number = 0;
        public sbyte Number
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

        public int Category
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