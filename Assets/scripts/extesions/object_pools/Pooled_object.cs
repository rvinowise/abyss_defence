using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.debug;
using rvinowise.rvi.contracts;


namespace extensions.pooling {

public class Pooled_object: MonoBehaviour {

    public Object_pool _pool;// { get; set; }

    private static int count = 0;

    public Object_pool pool {
        get { return _pool; }
        set {
            Contract.Requires(
                _pool == null,
                "pool of a pooled object can only be set once"
            );
            _pool = value;
        }
    }

    public GameObject instantiate() {
        if (pool == null) {
            count++;
            pool = new Object_pool(this.gameObject);
            Debug.Log(string.Format("amount of prefabs with pooled Component '{0}' = {1}", this, count));
        }
        return pool.get();
    }
    public TComponent instantiate<TComponent>() where TComponent: MonoBehaviour {
        GameObject game_object = get_from_pool();
        TComponent component = game_object.GetComponent<TComponent>();
        return component;
    }
    
    public TComponent instantiate<TComponent>(
        Vector2 in_position,
        Quaternion in_rotation
    ) 
        where TComponent: MonoBehaviour 
    {
        GameObject game_object = get_from_pool();
        game_object.transform.position = in_position;
        game_object.transform.rotation = in_rotation;
        TComponent component = game_object.GetComponent<TComponent>();
        return component;
    }

    private GameObject get_from_pool() {
        if (pool == null) {
            count++;
            pool = new Object_pool(this.gameObject);
            Debug.Log(string.Format("amount of prefabs with pooled Component '{0}' = {1}", this, count));
        }
        GameObject game_object = pool.get();
        return game_object;
    }

    public void destroy() {
        if (pool != null) {
            gameObject.SetActive(false);
            pool.return_to_pool(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    
}


}