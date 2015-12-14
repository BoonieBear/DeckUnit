using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BoonieBear.DeckUnit.Mov4500UI.Models
{
    
    public class PieInfo:INotifyPropertyChanged
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
}