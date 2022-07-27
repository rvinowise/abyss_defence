using System.Collections.Generic;


namespace rvinowise.unity.extensions {

public class Dictionary_of_queues<
    TKey,
    TValue
>:
    Dictionary_of_queues_abstract<
        TKey,
        TValue,
        Dictionary<TKey,Queue<TValue>>
    >
where TValue: class
{

}

public class Dictionary_of_queues_abstract<
    TKey, 
    TValue,
    TDictionary
> 
    where TValue: class
    where TDictionary: IDictionary<TKey, Queue<TValue>>, new()
{

    public TDictionary dictionary = 
        new TDictionary();

    public void add(TKey key, TValue obj) {
        Queue<TValue> bases = get_or_create_place_for_key(key);
        bases.Enqueue(obj);
    }
    
    public Queue<TValue> get_values(
        TKey key
    ) {
        return get_container_for_key(key);
    }

    public TValue retrieve_one(TKey key) {

        Queue<TValue> container = get_container_for_key(key);
        if (container == null) {
            return null;
        }
        
        return retrieve_one_from_container(container);
    }
    

    public Queue<TValue> this[TKey key] {
        get => get_values(key);
    }
    private TValue retrieve_one_from_container(Queue<TValue> queue) {
        return queue.Dequeue();
    }
    
    private Queue<TValue> get_or_create_place_for_key(TKey key) {
        Queue<TValue> container = null;
        if (!has_container_for_key(key)) {
            container = new Queue<TValue>();
            dictionary.Add(key, container);
        }
        return container;
    }
    private bool has_container_for_key(TKey key) {
        Queue<TValue> container;
        dictionary.TryGetValue(key, out container);
        if (container == null) {
            return false;
        }
        return true;
    }

    private Queue<TValue> get_container_for_key(TKey key) {
        Queue<TValue> container;
        dictionary.TryGetValue(key, out container);
        
        return container;
    }

}

}