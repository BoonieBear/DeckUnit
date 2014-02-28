namespace BoonieBear.DeckUnit.MessageEvents
{
    class GoHistoryDataPageBaseNavigationRequest
    {
        private string _titile;
		public GoHistoryDataPageBaseNavigationRequest(string titile)
		{
		    _titile = titile;
		}

        public string Titile
        {
            get { return _titile; }
        }
    }
}
