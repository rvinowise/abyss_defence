using System.Collections.Generic;


namespace rvinowise.unity.extensions {

public class Dictionary_of_lists<TKey, TValue> :
    Multivalue_dictionary<
        TKey,
        TValue,
        Dictionary<TKey, List<TValue>>,
        List<TValue>
    >
    where TValue: class
{
    
}
}