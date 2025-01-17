﻿using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Deprecated due to GC issues
/// </summary>
/// <typeparam name="P" ="P"></typeparam>
/// <typeparam name="V"></typeparam>
class PriorityQueue<P, V>
{
    private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();

    public int Count
    {
        get { return list.Count; }
    }


    public void Enqueue(P priority, V value)
    {
        Queue<V> q;
        if (!list.TryGetValue(priority, out q))
        {
            q = new Queue<V>();
            list.Add(priority, q);
        }
        q.Enqueue(value);
    }
    public V Dequeue()
    {
        // will throw if there isn’t any first element!
        var pair = list.First();
        var v = pair.Value.Dequeue();
        if (pair.Value.Count == 0) // nothing left of the top priority.
            list.Remove(pair.Key);
        return v;
    }
    public bool IsEmpty
    {
        get { return !list.Any(); }
    }

    public bool Contains(V item)
    {
        var e = list.GetEnumerator();
        
        foreach (var x in list.Where(i => Equals(e.Current.Key, item)))
        {
            return true;
        }
        return false;
    }
}
