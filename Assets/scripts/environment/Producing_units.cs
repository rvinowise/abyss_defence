﻿using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;

namespace rvinowise.unity {
public class Producing_units : MonoBehaviour
{

    public Transform created_unit_prefab;
    
    public float producing_time;
    public string emitting_animation;
    public Transform spawn;
    public Team team;

    private float last_producing_time = 0;
    private Animator animator;
    private Transform unit_being_created;

    void Awake() {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (can_produce_unit()) {
            produce_unit();
            last_producing_time = Time.time;
        }
    }
    
    static bool has_notified_that_complexity_exceeded = false;

    private bool can_produce_unit() {
        if (!Map.instance.is_complexity_exceeded()) {
            if (Time.time - last_producing_time >= producing_time) {
                return true;
            }
            has_notified_that_complexity_exceeded = false;
        }
        else {
            if (!has_notified_that_complexity_exceeded) {
                Debug.Log(
                    $"LIMITING_UNITS: maximum complexity of map ({Map.max_complexity}) is exceeded, it's {Map.instance.current_complexity} now");
                has_notified_that_complexity_exceeded = true;
            }
        }
        return false;
    }

    private void produce_unit() {
        animator.SetTrigger(emitting_animation);
    }

    [called_in_animation]
    void instantiate_unit() {
        unit_being_created = created_unit_prefab.instantiate<Transform>();
        unit_being_created.position = spawn.position;
        unit_being_created.rotation = spawn.rotation;

        if (unit_being_created.GetComponent<Intelligence>() is Intelligence intelligence) {
            team.add_unit(intelligence);
        }
        unit_being_created.gameObject.SetActive(true);
    }

   [called_in_animation]
   void make_unit_ready(AnimationEvent in_event) {
        if (unit_being_created != null) {
            unit_being_created.transform.set_z(created_unit_prefab.transform.position.z);
        }
   }
}

}