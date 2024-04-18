using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity
{
public class Compound_transporter: 
    IActor_transporter
{

    public List<GameObject> weapon_objects; //only for initialisation in inspector
    public readonly List<IActor_transporter> child_transporters;


    // internal override void Awake() {
    //     base.Awake();
    //     foreach (var weapon_object in weapon_objects) {
    //         weapons.Add(weapon_object.GetComponent<Attacker_child_of_group>());
    //     }
    // }

    public Compound_transporter(IEnumerable<IActor_transporter> transporters) {
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

    private Action_runner action_runner;
    public void init_for_runner(Action_runner in_action_runner) {
        this.action_runner = in_action_runner;
    }

    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    #endregion
}

}

