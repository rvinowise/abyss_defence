using System;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;


namespace rvinowise.unity.extensions.pooling {

public class Pooled_object: MonoBehaviour {

    public Object_pool _pool;// { get; set; }

    private static int count = 0;

    public Object_pool pool {
        get { 
            return _pool;
        }
        set {
            Contract.Requires(
                _pool == null,
                "pool of a pooled object can only be set once"
            );
            _pool = value;
        }
    }

    public GameObject get_prefab() => pool.prefab;
    public List<Component> reset_components = new List<Component>();
    public UnityEngine.Events.UnityEvent on_restored_from_pool;


    private void ensure_pool_created() {
        if (pool == null) {
            count++;
            //this.gameObject.SetActive(false); // so that OnEnable is called as the instance constructor
            pool = new Object_pool(this.gameObject);
            Debug.Log(String.Format("pooled_prefab: {0}, total: {1}", gameObject.name, count));
        }
    }
    
    [called_by_prefab]
    public TComponent get_from_pool<TComponent>(
        Vector2 in_position,
        Quaternion in_rotation
    ) 
        where TComponent: Component 
    {
        GameObject retrieved_object = get_from_pool();
        retrieved_object.transform.position = in_position;
        retrieved_object.transform.rotation = in_rotation;
        retrieved_object.SetActive(true);
        
        retrieved_object.GetComponent<Pooled_object>().on_restored_from_pool?.Invoke();

        TComponent component = retrieved_object.GetComponent<TComponent>();
        return component;
    }

    [called_by_prefab]
    public TComponent get_from_pool<TComponent>() 
    where TComponent: Component
    {
        TComponent component = get_from_pool().GetComponent<TComponent>();
        component.gameObject.SetActive(true);
        return component;
    }

    [called_by_prefab]
    public GameObject get_from_pool() {
        Contract.Requires(
            gameObject.is_prefab(),
            "it should be called from a prefab"
        );
        ensure_pool_created();
        GameObject game_object = pool.get();
        return game_object;
    }


    [called_by_prefab]
    public void init_reset_components(GameObject restored_object) {
        foreach(Component src_component in reset_components) {
            Component dst_component = restored_object.GetComponent(src_component.GetType());
            dst_component.copy_fields_from(src_component);
        } 
    }

    public void copy_enabledness_of_all_behaviours(GameObject restored_object) {
        foreach (var src_behaviour in GetComponents<Behaviour>()) {
            Component dst_component = restored_object.GetComponent(src_behaviour.GetType());
            dst_component.copy_enabledness(src_behaviour);
        }
    }

    [called_by_prefab]
    public void prefill_pool(int qty) {
        ensure_pool_created();
        pool.prefill(qty);
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