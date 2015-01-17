namespace BoonieBear.DeckUnit.Events
{
    public class GoWaterTelPageBaseNavigationRequest 
    {
        private string _titile;
        public GoWaterTelPageBaseNavigationRequest(string titile)
		{
		    _titile = titile;
		}

        public string Titile
        {
            get { return _titile; }
        }
    }
}