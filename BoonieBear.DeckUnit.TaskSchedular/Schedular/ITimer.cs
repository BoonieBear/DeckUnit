using System;

namespace BoonieBear.DeckUnit.Utilities.Schedular
{
    public interface ITimer
    {
        TimeSpan Interval { get; set; }
        void Start();
        void Stop();
        void Register(TimerEventHandler Tick);
        //void Register(ElapsedEventHandler Tick);
        //void Register(EventHandler Tick);
    }
}
