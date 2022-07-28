namespace Cyens.ReInherit.Pooling
{
    public interface IGatherableCallback
    {
        public void OnGather();
    }

    public interface ISpawnableCallback
    {
        public void OnSpawn();
    }

    public interface IPoolable : IGatherableCallback, ISpawnableCallback { }
}