using System.ComponentModel;

namespace BoonieBear.DeckUnit.Helps
{
    public class SystemInfo:INotifyPropertyChanged
    {
        public string Category { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}