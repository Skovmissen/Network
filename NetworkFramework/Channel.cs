namespace NetworkFramework
{
    public interface Channel<T>
    {
        void Send(T msg);

        T Receive();

        void SetVerbose(bool val);
    }
}