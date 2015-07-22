namespace BoonieBear.DeckUnit.Events
{
    class GoSimplePage
    {
        private string _titile;
        public GoSimplePage(string titile)
		{
		    _titile = titile;
		}

        public string Titile
        {
            get { return _titile; }
        }
    }
}
