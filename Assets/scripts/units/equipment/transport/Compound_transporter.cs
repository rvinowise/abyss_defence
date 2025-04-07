using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity
{
public class Compound_transporter: 
    ITransporter
{

    public List<GameObject> weapon_objects; //only for initialisation in inspector
    public readonly List<ITransporter> child_transporters;


    // internal override void Awake() {
    //     base.Awake();
    //     foreach (var weapon_object in weapon_objects) {
    //         weapons.Add(weapon_object.GetComponent<Attacker_child_of_group>());
    //     }
    // }

    public Compound_transporter(IEnumerable<ITransporter> transporters) {
        child_transporters = transporters.ToList();
    }
    
    
    #region ITransporter

    public float get_possible_rotation() {
        throw new System.NotImplementedException();
    }

    public float get_possible_impulse() {
        throw new System.NotImplementedException();
    }

    public void set_moved_body(Turning_element in_body) {
        throw new System.NotImplementedException();
    }
    public Turning_element get_moved_body() {
        throw new System.NotImplementedException();
    }

    public void move_towards_destination(Vector2 destination) {
        throw new System.NotImplementedException();
    }

    public void face_rotation(Quaternion rotation) {
        throw new System.NotImplementedException();
    }


    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }

    #endregion
}

}

