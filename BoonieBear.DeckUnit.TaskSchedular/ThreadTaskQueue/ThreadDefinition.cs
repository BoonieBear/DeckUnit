namespace BoonieBear.DeckUnit.TaskSchedular.ThreadTaskQueue
{
    public enum THREADED_ENQUEUE_TYPE
    {
        NORMAL = 0,
        CLEAR_AND_ADD,
    }

    public enum COMMAND_PRIORITY
    {
        PRIORITY_NORMAL = 0,
        PRIORITY_LOWEST,
        PRIORITY_HIGH
    }

    public enum COMMAND_STATE
    {
        STATE_NONE = 0,
        STATE_WAIT,
        STATE_RUN,
        STATE_COMPLETE,
        STATE_CANCEL
    }
}
