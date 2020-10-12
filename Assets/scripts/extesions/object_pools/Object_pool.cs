using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extensions.pooling {

using TObject = GameObject;
public class Object_pool {
    [SerializeField] private GameObject prefab;

    private Queue<TObject> objects = new Queue<TObject>();

    public Object_pool(GameObject in_prefab) {
        prefab = in_prefab;
    }


    public GameObject get() {
        if (objects.Count == 0) {
            return add_object(1);
        }
        GameObject retrieved_object = objects.Dequeue();
        retrieved_object.SetActive(true);
        return retrieved_object;
    }

    public void return_to_pool(TObject in_object) {
        
        objects.Enqueue(in_object);
    }

    private TObject add_object(int count) {
        var new_object = GameObject.Instantiate(prefab);
        new_object.GetComponent<Pooled_object>().pool = this;
        
        return new_object;
    }
}

}