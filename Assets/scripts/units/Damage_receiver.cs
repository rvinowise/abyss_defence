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
    
    public TMP_Text text_label;

    private Intelligence intelligence;
    
    
    void Awake() {
        intelligence = GetComponent<Intelligence>();

        destructible = GetComponent<IDestructible>();
        
    }

    private void start_dying() {
        if (destructible != null) {
            Debug.Log($"({name})Damage_receiver.start_dying, received_damage={received_damage}");
            destructible.on_start_dying();
            Destroy(intelligence);
        }
    }


    void OnCollisionEnter2D(Collision2D collision) {
        
        if (collision.get_damaging_projectile() is { } damaging_projectile ) {
            if (!damaging_projectile.GetComponent<Collider2D>().isActiveAndEnabled) {
                //at high speeds, projectile mistakenly bounses off the target, even though it should stop at the target.
                //switching its collider off at the first collision signifies that it shouldn't bounce and collide anymore
                return; 
            }
            Debug.Log($"AIMING: ({name})Damage_receiver.OnCollisionEnter2D(projectile:{damaging_projectile.name})");
            damaging_projectile.stop_at_position(collision.GetContact(0).point);
            receive_damage(1f);
        }
        else if (collision.gameObject.GetComponent<Damage_dealer>() is {} damage_dealer){
            //receive_damage(1f);
        }
    }

    public void receive_damage(float in_damage) {
        received_damage += in_damage;
        if (text_label != null) {
            text_label.text = received_damage.ToString(CultureInfo.InvariantCulture);
        }
        if (received_damage >= max_damage) {
            start_dying();

        }
    }

#if UNITY_EDITOR

#endif
}
}