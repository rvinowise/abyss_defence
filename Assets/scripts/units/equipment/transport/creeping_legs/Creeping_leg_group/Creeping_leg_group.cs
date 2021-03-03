using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.contracts;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.transport;
using UnityEngine.Assertions;
using static rvinowise.unity.geometry2d.Directions;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {
public partial class Creeping_leg_group: 
    Children_group
    ,ITransporter
{
    
    #region Children_group
    public override IEnumerable<IChild_of_group> children  {
        get { return legs as IEnumerable<IChild_of_group>; }
    }

    protected override void Awake() {
        base.Awake();
        init_components();
    }

    protected override void Start() {
        base.Start();
        foreach(var leg in legs) {
            leg.init_folding_direction();
        }
        create_moving_strategy();

        Contract.Requires(GetComponents<Turning_element>().Length <= 1,
            "only one component with Turning_element is enough");
        turning_element = GetComponent<Turning_element>();
    }

    private void create_moving_strategy() {
        if (legs.Count == 4) {
            stable_leg_groups = new List<Stable_leg_group>() {
                new Stable_leg_group(
                    new List<ILeg>() {left_front_leg, right_hind_leg}
                ),
                new Stable_leg_group(
                    new List<ILeg>() {right_front_leg, left_hind_leg}
                )
            };
        } 
        guess_moving_strategy();
    }

    protected void init_components() {
        rigid_body = gameObject.GetComponent<Rigidbody2D>();
        if (rigid_body != null) {
            move_in_direction = move_in_direction_as_rigidbody;
        }
        else {
            move_in_direction = move_in_direction_as_transform;
        }
    }

    public override void add_child(IChild_of_group compound_object) {
        Contract.Requires(compound_object is ALeg);
        ALeg leg = compound_object as ALeg;
        _legs.Add(leg);
        leg.transform.SetParent(transform, false);
    }

    public Type child_type() {
        return typeof(ILeg);
    }

    protected override void init_child_list() {
        _legs = new List<ALeg>();
    }
  
    

    public override void shift_center(Vector2 in_shift) {
        foreach (ILeg leg in legs) {
            leg.transform.localPosition += (Vector3)in_shift;
            //leg.optimal_position_standing = (leg.optimal_position_standing + in_shift);
        }
    }
    #endregion

    

    #region ITransporter
    
    public static float belly_friction_multiplier = 0.9f;
    
    public float possible_impulse { get; set; }

    private void init_possible_impulse() {
        if (moving_strategy is null) {
            possible_impulse = 0f;
        }
        float impulse = 0;
        foreach (ILeg leg in legs) {
            if (!leg.is_up) {
                impulse += leg.provided_impulse;
            }
        }
        if (moving_strategy.belly_touches_ground()) {
            impulse *= belly_friction_multiplier;
        }
        possible_impulse = impulse;
    }



    private const float rotate_faster_than_move = 100f;

    public float possible_rotation {
        get { 
            return this.GetComponent<Turning_element>().rotation_acceleration;
            //return possible_impulse * rotate_faster_than_move; 
        }
        set{ Contract.Assert(false, "set possible_impulse instead");}
    }

    public Quaternion direction_quaternion {
        get { return transform.rotation; }
    }

    public transport.Transporter_commands command_batch { get; } = new transport.Transporter_commands();


    private delegate void Move_in_direction(Vector2 moving_direction);
    private Move_in_direction move_in_direction;
        
    public void move_in_direction_as_rigidbody(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * possible_impulse * rvi.Time.deltaTime);
        //rigid_body.MovePosition((Vector2)transform.position + delta_movement);
        rigid_body.AddForce(delta_movement*10000f);
    }
    public void move_in_direction_as_transform(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * possible_impulse * rvi.Time.deltaTime);
        transform.position += (Vector3)delta_movement;
    }
    
    public void rotate_to_direction(Quaternion face_direction) {

        turning_element.rotation_acceleration = possible_rotation;
        turning_element.target_rotation = face_direction;
        turning_element.rotate_to_desired_direction();
    }
    
    
    void FixedUpdate() {
        execute_commands();
        move_legs();
    }
    
    protected void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        if (turning_element != null) {
            rotate_to_direction(command_batch.face_direction_quaternion);
        }
    }
    #endregion
    
    
    #region Creeping_leg_group itself
    
    /* legs that are enough to be stable if they are on the ground */
    public List<Stable_leg_group> stable_leg_groups = new List<Stable_leg_group>();

    public strategy.Moving_strategy moving_strategy {
        get { return _moving_strategy; }
        set {
            _moving_strategy = value;
            init_possible_impulse();
        }
    }
    private strategy.Moving_strategy _moving_strategy;
    
    

    private Turning_element turning_element;
    
    public IReadOnlyList<ILeg> legs {
        set {
            _legs = value as List<ALeg>;
            guess_moving_strategy();
        }
        get {
            return _legs as IReadOnlyList<ILeg>;
        }
    }
    public ILeg left_hind_leg {
        get { return legs[0];}
        set { _legs[0] = value as ALeg; }
    }
    public ILeg left_front_leg {
        get { return legs[1];}
        set { _legs[1] = value as ALeg; }
    }
    public ILeg right_hind_leg {
        get { return legs[2];}
        set { _legs[2] = value as ALeg; }
    }
    public ILeg right_front_leg {
        get { return legs[3];}
        set { _legs[3] = value as ALeg; }
    }
    [SerializeField]
    public List<ALeg> _legs = new List<ALeg>();

    private Rigidbody2D rigid_body { get; set; }
    
    public void guess_moving_strategy() {
        if (this.stable_leg_groups.Count > 1) {
            moving_strategy = new strategy.Stable(legs, this);
        } else
        if (legs.Count > 1 ) {
            moving_strategy = new strategy.Grovelling(legs, this);
        } else if (legs.Count == 1) {
            moving_strategy = new strategy.Faltering(legs, this);
        }
    }


    private void move_legs() {
        foreach (ILeg leg in legs) {
            leg.set_desired_position(get_optimal_position_for(leg)); 
            if (leg.is_up) {
                if ((leg as MonoBehaviour).name == "leg_l_f") {
                    bool test = true;
                }
                move_in_the_air(leg);
            } else {
                if ((leg as MonoBehaviour).name == "leg_l_f") {
                    bool test = true;
                }
                moving_strategy.move_on_the_ground(leg);
            }
        }
    }
    
    private void move_in_the_air(ILeg leg) {
        if (leg.has_reached_aim()) {
            put_down(leg);
        } else {
            leg.move_segments_towards_desired_direction();
        }
    }

    private void put_down(ILeg leg) {
        Contract.Requires(leg.is_up);
        leg.put_down();
        possible_impulse += leg.provided_impulse;
    }

    
    private Vector2 get_optimal_position_for(ILeg leg) {
        Vector2 shift_to_moving_direction =
            command_batch.moving_direction_vector *
            leg.moving_offset_distance * leg.transform.lossyScale.x;

        /* leg.set_desired_position(
           leg.optimal_position_standing + 
            shift_to_moving_direction
        ); */

        return leg.optimal_position_standing + 
                shift_to_moving_direction;
    }

    


    bool can_move() {
        foreach (ILeg leg in legs) {
            if (!leg.is_up) {
                return true;
            }
        }
        return false;
    }

    

    public void OnDrawGizmos() {
        foreach (ILeg leg in legs) {
            if (leg != null) {
                if (Application.isPlaying) {
                    leg.draw_positions();
                    leg.draw_desired_directions();
                }
                leg.draw_directions(Color.white);
            }
        }
        //draw_moving_direction();
    }

    /*private void draw_moving_direction() {
        UnityEngine.Debug.DrawLine(
            this.user_of_equipment.transform.position, 
            (Vector2)this.user_of_equipment.transform.position +
            control.moving_direction_vector,
            Color.green,
            1f
        );
    }*/

    #endregion

    }

}

