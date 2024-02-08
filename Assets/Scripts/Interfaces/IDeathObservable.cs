public interface IDeathObservable
{
    System.Action<object> OnKilled
    {
        get;
        set;
    }

    public void Subscribe(System.Action<object> callback);
}