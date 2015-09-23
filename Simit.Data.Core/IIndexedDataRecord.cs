namespace Simit.Data.Core
{
    public interface IIndexedDataRecord
    {
        object this[string name] { get; }
    }
}