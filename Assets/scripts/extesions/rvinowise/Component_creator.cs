using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units;
using UnityEngine.Assertions;


namespace rvinowise {

public class Component_creator:
    MonoBehaviour,
    ICompound_object 
{

    public GameObject game_object {
        get { return this.gameObject; }
}
    
    
    /* GameObject fields */
    public Quaternion rotation {
        get {
            return transform.rotation;
        }
        set => transform.rotation = value;
    }
    public Transform transform {
        get {
            return game_object.transform;
        }
    }
    
    /* ICompound_object interface */
    public Transform parent {
        get {
            return transform.parent;
        }
        set { transform.SetParent(value, false); }
    }

    public GameObject main_object {
        get { return gameObject; }
    }
    
    public Vector2 position {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }
    
    public Vector2 direction {
        get {
            return transform.right;
        }
        set {
            float needed_angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.forward * needed_angle;
        }
    }
    
    public Vector2 local_position {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }
    public SpriteRenderer spriteRenderer {
        get {
            return game_object.GetComponent<SpriteRenderer>();
        }
    }
    
    
    public void activate() {
        game_object.SetActive(true);
    }
    public void deactivate() {
        game_object.SetActive(false);
    }
    public bool active() {
        return game_object.activeSelf;
    }
    
    public void direct_to(Vector2 in_aim) {
        transform.direct_to(in_aim);
    }
    public void set_direction(float in_direction) {
        transform.set_direction(in_direction);
    }


    public virtual void update() {
    }
    
    public static GameObject instantiate_stashed(GameObject prefab) {
        GameObject game_object = GameObject.Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.identity);
        game_object.SetActive(false);
        return game_object;
    }
    public static GameObject instantiate_stashed(Component component) {
        return instantiate_stashed(component.gameObject);
    }
    public static GameObject instantiate_stashed(string name) {
        GameObject prefab = Resources.Load<GameObject>(name);
        
        return instantiate_stashed(prefab);
    }

    public static GameObject instantiate(GameObject prefab) {
        GameObject game_object = GameObject.Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.identity);
        return game_object;
    }
    public static Component instantiate(Component component) {
        return instantiate(component.gameObject).GetComponent<Component>();
    }
    
    public static GameObject instantiate(string name) {
        GameObject prefab = Resources.Load<GameObject>(name);
        Contract.Assert(prefab != null);
        
        return instantiate(prefab);
    }

    


    
}
}