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

    public IDestructible destructible;
    public IBleeding_body bleeding_body;
    
    public TMP_Text text_label;

    private Intelligence intelligence;
    public delegate void Evend_handler(Damage_receiver damagable);
    public event Evend_handler on_destroyed;
    public delegate void On_damaged_handler(float damage_change);
    
    public event On_damaged_handler on_damage_changed;
    
    void Awake() {
        intelligence = GetComponent<Intelligence>();

        destructible = GetComponent<IDestructible>();
        
    }

    private void start_dying() {
        if (destructible != null) {
            destructible.on_start_dying();
        }
        Debug.Log($"({name})Damage_receiver.start_dying, received_damage={received_damage}");
        intelligence.notify_about_destruction();
        Destroy(intelligence);
        on_destroyed?.Invoke(this);
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
                    return; 
                }
                //damaging_projectile.stop_at_position(collision.GetContact(0).point);
            }
            Debug.Log($"AIMING: ({name})Damage_receiver.OnCollisionEnter2D(damage_dealer:{damage_dealer.name})");
            receive_damage(damage_dealer.effect_amount);
            
        }
    }

    public void receive_damage(float in_damage) {
        received_damage += in_damage;
        if (text_label != null) {
            text_label.text = received_damage.ToString(CultureInfo.InvariantCulture);
        }
        on_damage_changed?.Invoke(in_damage);
        if (received_damage >= max_damage) {
            start_dying();
        }
    }

#if UNITY_EDITOR

#endif
}
}