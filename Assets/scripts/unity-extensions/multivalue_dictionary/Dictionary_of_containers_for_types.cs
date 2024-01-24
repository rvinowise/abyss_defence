using System;
using System.Collections.Generic;


namespace rvinowise.unity.extensions {

public class Dictionary_of_containers_for_types<TContainer, TValue> 
    where TValue: class {

    public Dictionary<Type, Queue<TValue>> dictionary = new Dictionary<Type, Queue<TValue>>();

    public void insert(System.Type key_type, TValue obj) {
        Queue<TValue> bases = get_or_create_place_for_type(key_type);
        bases.Enqueue(obj);
    }
    
    public TValue get(
        System.Type type
    ) {
        Queue<TValue> bases = get_or_create_place_for_type(type);

        if (bases.Count == 0) {
            return null;
        }
        TValue retrieved_value = bases.Dequeue();
        return retrieved_value;
    }
    
    private Queue<TValue> get_or_create_place_for_type(Type type) {
        Queue<TValue> container;
        dictionary.TryGetValue(type, out container);
        if (container == null) {
            container = new Queue<TValue>();
            dictionary.Add(type, container);
        }
        return container;
    }
}
}