using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {

public class Pump_shotgun: Gun {
    
    [SerializeField]
    public Transform shell_ejector;
    public Slot reloading_slot { get; private set; }

    public int max_rounds = 12;
    
    /* current values */
    public int rounds_n;
    
    protected override void init_holding_places() {
        main_holding = Holding_place.main(this);
        second_holding = Holding_place.create(transform);
        second_holding.place_on_tool = new Vector2(0.54f, 0f);
        second_holding.grip_gesture = Hand_gesture.Support_of_horizontal;
        second_holding.grip_direction = new Degree(-70f);
    }
    
    protected override void init_components() {
        base.init_components();
        reloading_slot = GetComponentInChildren<Slot>();
    }

    public override void insert_ammunition(Ammunition in_ammunition) {
        if (rounds_n < max_rounds) {
            rounds_n++;
        }
    }

    public bool can_apply_ammunition(Ammunition in_ammunition) {
        return rounds_n < max_rounds;
    }

    protected override void fire() {

    }
}
}