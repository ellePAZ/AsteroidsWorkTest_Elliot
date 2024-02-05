public interface IDeathObservable
{
    protected System.Action OnKilled
    {
        get;
        set;
    }

    public void Subscribe(System.Action callback);
}