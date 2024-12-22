using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity
{
public class Gun_holder: MonoBehaviour, 
    IActor_attacker
{

    public IGun gun;
    public Turning_element turning_element;
    public Transform muzzle;

    private void Awake() {
        gun = GetComponentInChildren<IGun>();
    }
    
    
    #region IWeaponry
    public bool is_weapon_targeting_target(Transform target) {
        var hit = Physics2D.Raycast(muzzle.position, muzzle.rotation.to_vector(), reaching_distance);

        if (hit.transform == target) {
            return true;
        }
        return false;
    }

    public float reaching_distance;
    public float get_reaching_distance() {
        return reaching_distance;
    }

    public void attack(Transform target, System.Action on_completed) {
        gun.pull_trigger();
        gun.release_trigger();
        on_completed.Invoke();
    }
    
    #endregion

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    private Action_runner action_runner;

    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }
}

}

