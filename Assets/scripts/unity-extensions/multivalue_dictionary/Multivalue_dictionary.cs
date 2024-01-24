
using System.Collections.Generic;
using System.Linq;

namespace rvinowise.unity.extensions {


public class Multivalue_dictionary<
    TKey, 
    TValue,
    TDictionary, 
    TContainer
> 
    where TValue: class
    where TDictionary: IDictionary<TKey, TContainer>, new()
    where TContainer: class, ICollection<TValue>, new()
{

    public TDictionary dictionary = 
        new TDictionary();

    public void add(TKey key, TValue obj) {
        TContainer bases = get_or_create_place_for_key(key);
        bases.Add(obj);
    }
    
    public TContainer get_values(
        TKey key
    ) {
        return get_container_for_key(key);
    }

    public TValue retrieve_one(TKey key) {

        TContainer container = get_container_for_key(key);
        if (container == null) {
            return null;
        }
        
        return retrieve_one_from_container(container);
    }

    public TValue retrieve_one_from_container(TContainer container) {
        throw new System.NotImplementedException();
    }

    public TValue retrieve_one_from_container(Queue<TValue> queue) {
        return queue.Dequeue();
    }
    public TValue retrieve_one_from_container(List<TValue> list) {
        TValue value =  list.Last();
        list.Remove(list.LastOrDefault());
        return value;
    }

    public TContainer this[TKey key] {
        get => get_values(key);
    }
    
    private TContainer get_or_create_place_for_key(TKey key) {
        TContainer container;
        dictionary.TryGetValue(key, out container);
        if (container == null) {
            container = new TContainer();
            dictionary.Add(key, container);
        }
        return container;
    }
    

    private TContainer get_container_for_key(TKey key) {
        TContainer container;
        dictionary.TryGetValue(key, out container);
        
        return container;
    }
}
}