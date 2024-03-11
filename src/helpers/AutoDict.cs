
using System.Collections;

// a more useful version of Dictionary that automatically generates keys
public class AutoDict<K, V> : Dictionary<K, V>
{
    private Func<V, K> _keyselector;

    public AutoDict(Func<V, K> keyselector)
    {
        _keyselector = keyselector;
    }

    public AutoDict(Func<V, K> keyselector, ICollection<V> ls)
    {
        _keyselector = keyselector;
        AddAll(ls);
    }

    public void Add(V val)
    {
        base.Add(_keyselector(val), val);
    }

    public void Remove(V val) {
        base.Remove(_keyselector(val));
    }

    public void AddAll(ICollection<V> collection) {
        HashSet<K> keys = collection.Select(val => _keyselector(val)).ToHashSet();
        foreach (var (First, Second) in keys.Zip(collection)) {
            this[First] = Second;
        }
    }
}