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

namespace rvinowise.unity.infrastructure {
public class Producing_units : MonoBehaviour
{

    public GameObject created_unit_prefab;
    
    public float producing_time;
    public string emitting_animation;
    public Transform spawn;

    private float last_producing_time = 0;
    private Animator animator;
    private GameObject unit_being_created;

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

        /* unit_being_created = GameObject.Instantiate(
            created_unit_prefab, 
            transform.position + spawn_position,
            transform.rotation * Directions.degrees_to_quaternion(90f)
        ); */

        unit_being_created = created_unit_prefab.GetComponent<Pooled_object>().get_from_pool<Transform>(
        ).gameObject;
        unit_being_created.transform.position = spawn.position;
        unit_being_created.transform.rotation = spawn.rotation;

        unit_being_created.SetActive(true);
    }

   [called_in_animation]
   void make_unit_ready(AnimationEvent in_event) {
       Contract.Requires(
            unit_being_created != null, 
            "another function must create the unit before this frame changes it"
        );

        unit_being_created.transform.set_z(created_unit_prefab.transform.position.z);
   }
}

}