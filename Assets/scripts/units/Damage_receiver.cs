using System;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace rvinowise.unity {

public class Damage_receiver: MonoBehaviour {


    public float max_damage = 2f;
    public float received_damage;

    public ILeaving_persistent_residue leaving_residue;
    public Disappearing_body disappearing_body;

    public TMP_Text text_label;
    
    private Divisible_body divisible_body;

    private Intelligence intelligence;
    
    private bool needs_to_die;
    private float dying_moment = float.MaxValue;
    private float dying_time = 1f;
    void Awake() {
        divisible_body = GetComponent<Divisible_body>();
        leaving_residue = GetComponent<ILeaving_persistent_residue>();
        disappearing_body = GetComponent<Disappearing_body>();
        intelligence = GetComponent<Intelligence>();

        if (divisible_body != null) {
            divisible_body.on_polygon_changed+= prepare_to_death;
        }
    }

    public void prepare_to_death() {
        needs_to_die = true;
    }
    private void start_dying() {
        if (leaving_residue != null) {
            dying_moment = Time.time + dying_time;

        } else if (disappearing_body != null) {
            Debug.Log($"({name})Damage_receiver.start_dying, received_damage={received_damage}");
            disappearing_body.settle_when_stops();
            Destroy(intelligence);
        }
    }

    private void die() {
        if (leaving_residue != null) {
            leaving_residue?.leave_persistent_residue();
            Destroy(gameObject);
        }
    }
  

    void FixedUpdate() {
        if (needs_to_die) {
            die();
        }
    }

    void Update() {
        if (Time.time >= dying_moment) {
            die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        
        if (collision.get_damaging_projectile() is { } damaging_projectile ) {
            Debug.Log($"AIMING: ({name})Damage_receiver.OnCollisionEnter2D(projectile:{damaging_projectile.name})");
            receive_damage(1f);
        }
    }

    public void receive_damage(float in_damage) {
        received_damage += in_damage;
        if (text_label != null) {
            text_label.text = received_damage.ToString(CultureInfo.InvariantCulture);
        }
        if (received_damage > max_damage) {
            start_dying();
            //intelligence.start_dying(damaging_projectile);

        }
    }

#if UNITY_EDITOR

#endif
}
}