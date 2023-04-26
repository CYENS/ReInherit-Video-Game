namespace Cyens.ReInherit.Serialization
{
    public interface IJsonDataObject<T>
    {
        public T WriteJsonData();

        public void LoadJsonData(T data);
    }
}