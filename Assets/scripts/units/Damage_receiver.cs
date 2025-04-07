using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace rvinowise.unity {

public class Damage_receiver: MonoBehaviour {


    public float max_damage = 2f;
    public float received_damage;

    public List<IDestructible> destructibles = new List<IDestructible>();
    public IBleeding_body bleeding_body;
    
    public TMP_Text text_label;

    public Intelligence intelligence;
    public delegate void Event_handler(Damage_receiver damagable);
    public event Event_handler on_destroyed;
    public delegate void On_damaged_handler(float damage_change);
    
    public event On_damaged_handler on_damage_changed;
    
    void Awake() {
        intelligence = GetComponentInParent<Intelligence>();

        if (!destructibles.Any()) {
            destructibles = GetComponents<IDestructible>().ToList();
        }
        
    }

    public void start_dying() {
        if (destructibles.Any()) {
            destructibles.ForEach(destructible => destructible.die());
            if (intelligence) {
                intelligence.notify_about_destruction();
                Destroy(intelligence);
            }
            on_destroyed?.Invoke(this);
        }
        Debug.Log($"({name})Damage_receiver.start_dying, received_damage={received_damage}");
    }


    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Damage_dealer>() is {} damage_dealer){
            if (damage_dealer.is_ignoring_damage_receiver(this)) {
                return;
            }
            if (collision.get_damaging_projectile() is { } damaging_projectile ) {
                if (!damaging_projectile.GetComponent<Collider2D>().isActiveAndEnabled) {
                    //at high speeds, projectile mistakenly bounses off the target, even though it should stop at the target.
                    //switching its collider off at the first collision signifies that it shouldn't bounce and collide anymore
                    //Debug.Break();
                    return; 
                }
                //damaging_projectile.stop_at_position(collision.GetContact(0).point);
            }
            Debug.Log($"AIMING: ({name})Damage_receiver.OnCollisionEnter2D(damage_dealer:{damage_dealer.name})");
            receive_damage(damage_dealer.effect_amount);
            
        }
    }

    public void receive_damage(float in_damage) {
        on_damage_changed?.Invoke(in_damage);
        
        if (
            (received_damage < max_damage)&& //so that it won't start dying many times after exceeding the maximum damage
            (received_damage + in_damage >= max_damage) 
        ){
            start_dying();
        }
        
        received_damage += in_damage;
        if (text_label != null) {
            text_label.text = received_damage.ToString(CultureInfo.InvariantCulture);
        }
    }

#if UNITY_EDITOR

#endif
}
}