using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.unity.units.parts.weapons.guns.common;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.extensions.attributes;
using rvinowise.contracts;
using rvinowise.unity.extensions.pooling;
using rvinowise.unity.units;
using rvinowise.unity.units.control;

namespace rvinowise.unity.infrastructure {
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

    void Start()
    {
        
    }

    void Update()
    {
        if (Time.time - last_producing_time >= producing_time) {
            produce_unit();
            last_producing_time = Time.time;
        }
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
            intelligence.team = team;
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