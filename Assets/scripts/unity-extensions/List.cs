using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;

namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{
    

    public static T get_random_item<T>(this List<T> list) {
        float random = Random.Range(0, list.Count);
        int index = Mathf.RoundToInt(random);
        Contract.Assert(
            index>=0 && index < list.Count, 
            string.Format("random={0}, index={1}, list.Count={2}", random, index, list.Count)
        );
        return list[index];
    }
    
    
    
}

}