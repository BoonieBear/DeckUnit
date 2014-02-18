namespace BoonieBear.DockUnit.NavigationEvents
{
    class GoHistoryDataPageNavigationRequest
    {
        private int _dataid;
        public GoHistoryDataPageNavigationRequest(int logid)
        {
            _dataid = logid;
        }

        public string Data
        {
            get { return _dataid.ToString(); }
        }
    }
}
