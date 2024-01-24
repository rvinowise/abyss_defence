using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace rvinowise.unity.extensions.pooling {

using TObject = GameObject;
public class Object_pool {
    [SerializeField] private GameObject prefab;

    private Queue<TObject> objects = new Queue<TObject>();

    public Object_pool(GameObject in_prefab) {
        prefab = in_prefab;
    }


    public GameObject get() {
        if (objects.Count == 0) {
            return add_object();
        }
        GameObject retrieved_object = objects.Dequeue();
        prefab.GetComponent<Pooled_object>().init_reset_components(retrieved_object);

        return retrieved_object;
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