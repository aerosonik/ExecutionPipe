namespace NSV.ExecutionPipe.Cache
{
    public interface IPipeCache
    {
        T Get<T>(object key);

        void Set<T>(object key, T value);

        void Delete(object key);

        void Clear();
    }
}
