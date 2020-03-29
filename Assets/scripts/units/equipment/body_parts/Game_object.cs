using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units;


namespace rvinowise {

public class Game_object:Child {

    public readonly GameObject game_object; 
    
    
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
    
    public Transform parent {
        get {
            return transform.parent;
        }
        set { transform.SetParent(value, false); }
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
    
    public Vector2 local_position { //same as local_position but with Unity's parenting
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }
    public SpriteRenderer spriteRenderer {
        get {
            return game_object.GetComponent<SpriteRenderer>();
        }
    }
    public Animator animator  {
        get {
            if (_animator == null) {
                _animator = game_object.GetComponent<Animator>();
            }
            return _animator;
        }
        set { _animator = value; }
    }
    private Animator _animator;
    

    
    
    public Game_object() {
        game_object = new GameObject();
        game_object.AddComponent<SpriteRenderer>();
    }
    public Game_object(string name) {
        game_object = new GameObject(name);
        game_object.AddComponent<SpriteRenderer>();
    }
    public Game_object(string name, GameObject prefab) {
        game_object = GameObject.Instantiate(prefab, Vector2.zero, Quaternion.identity);
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


    
    public string animation {
        set {
            if (value != null) {
                if (animator == null) {
                    animator = game_object.AddComponent<Animator>();
                }
                RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(value);
                animator.runtimeAnimatorController = controller;
            }
            else {
                if (animator != null) {
                    Component.Destroy(animator);
                }
            }
        }
    }


    
}
}