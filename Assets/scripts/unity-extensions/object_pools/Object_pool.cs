using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace rvinowise.unity.extensions.pooling {

using TObject = GameObject;
public class Object_pool {
    public GameObject prefab;

    private Queue<TObject> objects = new Queue<TObject>();

    public Object_pool(GameObject in_prefab) {
        prefab = in_prefab;
    }


    public GameObject get() {
        if (objects.Count == 0) {
            return add_object();
        }
        GameObject retrieved_object = objects.Dequeue();
        copy_parameters_of_prefab_into_object(prefab, retrieved_object);
        return retrieved_object;
    }

    private void copy_parameters_of_prefab_into_object(
        GameObject src_prefab,
        GameObject dst_object
    ) {
        var pooled_prefab = src_prefab.GetComponent<Pooled_object>();

        pooled_prefab.copy_enabledness_of_all_behaviours(dst_object);
        pooled_prefab.init_reset_components(dst_object);
        dst_object.layer = pooled_prefab.gameObject.layer;
    }
    

    public void return_to_pool(TObject in_object) {
        
        objects.Enqueue(in_object);
    }


    public void prefill(int qty) {
        foreach(int i in Enumerable.Range(0,qty)) {
            TObject new_object = add_object();
            new_object.SetActive(false);
            return_to_pool(new_object);
        }
    }

    private TObject add_object() {
        var new_object = GameObject.Instantiate(prefab);
        new_object.GetComponent<Pooled_object>().pool = this;
        
        return new_object;
    }
}

}