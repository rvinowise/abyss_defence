using rvinowise.unity.geometry2d;


namespace rvinowise.unity.units.parts.limbs {

public class Limb3: Limb2 {

    public virtual Segment segment3 { get; set; } //an appendage at the end, to direct the children

    
    public override void rotate_to_desired_directions() {
        base.rotate_to_desired_directions();
        segment3.rotate_to_desired_direction();
    }
    
    public override void jump_to_desired_directions() {
        base.jump_to_desired_directions();
        segment3.jump_to_desired_direction();
    }

    public virtual void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        segment3.target_rotation = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    
    public override void preserve_possible_rotations() {
        base.preserve_possible_rotations();
        segment3.preserve_possible_rotations();
    }
    
    public override bool at_desired_rotation() {
        return (
            base.at_desired_rotation() &&
            this.segment3.at_desired_rotation()
        );

    }
}
}