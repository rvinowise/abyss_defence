using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.contracts;

namespace rvinowise.unity {
public class Tool: MonoBehaviour {

    public float weight = 5f;
    
    public Holding_place main_holding;
    public Holding_place second_holding;
    
    public Animator animator;


    public readonly Saved_physics last_physics = new Saved_physics();
    private void Awake() {
        init_components();
        init_holding_places();
    }


    private void init_components() {
        animator = gameObject.GetComponent<Animator>();
    }

    private void init_holding_places() {
        var holding_places = GetComponentsInChildren<Holding_place>();
        
        foreach(Holding_place holding_place in holding_places) {
            if (holding_place.is_main) {
                main_holding = holding_place;
            } else {
                second_holding = holding_place;
            }
        }
        Contract.Assert(main_holding != null);
    }
    
    public void hold_by(Hand in_hand) {
        transform.set_local_z(in_hand.held_object_local_z);
    }

    public void drop_from_hand() {
        transform.set_z(Map.instance.ground_z);
    }

    public void deactivate() {
        gameObject.SetActive(false);
    }
    public void activate() {
        gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        last_physics.position = transform.position;
    }

    
}




}