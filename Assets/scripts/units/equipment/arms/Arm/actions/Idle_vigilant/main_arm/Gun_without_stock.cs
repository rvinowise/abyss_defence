using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.weapons.guns;


namespace rvinowise.unity.units.parts.limbs.arms.actions.idle_vigilant.main_arm {

public class Gun_without_stock: actions.Action_of_arm
{

    /* parameters given by the user */
    private Transform target;
    private transport.ITransporter transporter; // movements of arms depend on where the body is moving
    
    /* inner parameters */
    private Quaternion upper_arm_offset_turn = Quaternion.identity;
    private Quaternion forearm_turn = Quaternion.identity;
    private Gun held_gun;
    
    
    
    public static Gun_without_stock create(
        Transform in_target,
        transport.ITransporter in_transporter
    ) {
        Gun_without_stock action = (Gun_without_stock)pool.get(typeof(Gun_without_stock));
        action.target = in_target;
        action.transporter = in_transporter;
        return action;
    }
    
    
    public Gun_without_stock() {
        
    }
    
    const float shoulder_thickness = 0.15f;
    private float distance_shoulder_to_wrist;

    protected override void on_start_execution() {
        if (arm.held_tool is Gun gun) {
            held_gun = gun;

            if (gun.has_stock) {
                distance_shoulder_to_wrist = held_gun.stock_length - arm.hand.length + shoulder_thickness;
            }
            else {
                distance_shoulder_to_wrist = arm.length/2f;
            }
            upper_arm_offset_turn =
                Side.turn_quaternion(
                    arm.folding_side,
                    unity.geometry2d.Triangles.get_quaternion_by_lengths(
                        arm.upper_arm.length,
                        distance_shoulder_to_wrist,
                        arm.forearm.length
                    )
                );
        }
    }

    private Vector2 position_of_wrist;
    public override void update() {
        base.update();
        
        var direction_to_target = arm.upper_arm.transform.quaternion_to(target.position);

        var body_wants_to_turn = new Degree(
            transporter.command_batch.face_direction_degrees -    
            transporter.direction_quaternion.to_float_degrees()
        ).use_minus();
        
        arm.upper_arm.target_rotation = 
            determine_desired_direction_of_upper_arm(direction_to_target, body_wants_to_turn);


        
        arm.forearm.target_rotation = 
            determine_desired_direction_of_forearm(direction_to_target, body_wants_to_turn);


        arm.hand.target_rotation =
            direction_to_target;
        
        arm.rotate_to_desired_directions();
    }

    
    private Quaternion determine_desired_direction_of_upper_arm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        
         Quaternion desired_direction =
            direction_to_target * upper_arm_offset_turn;

        if (body_wants_to_turn.side() == Side_type.LEFT) {
            desired_direction *= body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }

    
    private Quaternion determine_desired_direction_of_forearm(
        Quaternion direction_to_target, 
        Degree body_wants_to_turn
    ) {
        position_of_wrist = 
            arm.upper_arm.position + 
            (
                direction_to_target *
                (Vector2.right * distance_shoulder_to_wrist)
            );

        Quaternion direction_to_wrist =
            arm.upper_arm.desired_tip.quaternion_to(position_of_wrist);

        Quaternion desired_direction = direction_to_wrist;
        if (body_wants_to_turn.side() == Side_type.LEFT) {
            desired_direction = 
                direction_to_wrist * 
                body_wants_to_turn.to_quaternion().multiplied(1.1f).inverse();
        }

        return desired_direction;
    }



}
}