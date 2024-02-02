using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.sensors;
using rvinowise.unity.units.parts.transport;
using Baggage = rvinowise.unity.units.parts.Baggage;
using rvinowise.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.weapons.guns.common;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.control {

/* asks tool controllers what they can do,
 orders them some actions based on this information,
 they do these actions later in the same step */
public abstract class Intelligence :
    MonoBehaviour
{

    public Baggage baggage;
    
    public ISensory_organ sensory_organ;
    public GameObject sensory_organ_object;
    
    public ITransporter transporter;
    public GameObject transporter_object;
    
    public IWeaponry weaponry;
    public GameObject weaponry_object;
    
    public float last_rotation;

    public Team team;

    public Action_runner action_runner { get; } = new Action_runner();


    protected virtual void Awake() {
        sensory_organ = sensory_organ_object.GetComponent<ISensory_organ>();
        transporter = transporter_object.GetComponent<ITransporter>();
        weaponry = weaponry_object.GetComponent<IWeaponry>();
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );
        baggage = GetComponentInChildren<Baggage>();
        
    }

    protected virtual void Start() {
        if (team != null) {
            notify_other_units();
        }
        add_actors_to_action_runner();
        action_runner.start_fallback_actions();
    }

    private void add_actors_to_action_runner() {
        var actors = GetComponentsInChildren<IActor>();
        foreach (IActor actor in actors) {
            actor.init_for_runner(action_runner);
            action_runner.add_actor(actor);
        }
    }

    private void notify_other_units() {
        team.units.Add(this);

        foreach (Team enemy_team in team.enemies) {
            foreach (var enemy_unit in enemy_team.units) {
                enemy_unit.consider_enemy(this);
            }
        }
    }



    public virtual void consider_enemy(Intelligence in_enemy) { }

    protected virtual void Update() {
        read_input();
        action_runner.update();
    }
    

    protected virtual void read_input() { }

    protected void save_last_rotation(Quaternion needed_direction) {
        float angle_difference = transform.rotation.degrees_to(needed_direction).degrees;
        last_rotation = angle_difference;
    }

    public virtual void start_dying(Projectile damaging_projectile) {
        
    }
    


    public void add_action(Action action) {
        action_runner.add_action(action);
    }
}
}