using rvinowise.unity.extensions;

//using static UnityEngine.Input;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.weapons.guns.common;


namespace rvinowise.unity.units.control.spider {

public class Computer_spider: 
    Strategic_intelligence
{

    private Creeping_leg_group leg_group;
    private bool is_dying = false;

    protected override void Start() {
        base.Start();
        init_components();
    }

    private void init_components() {
        leg_group = transporter as Creeping_leg_group;
    }
 
    

    public override void start_dying(Projectile damaging_projectile) {
        is_dying = true;
        transporter.command_batch.moving_direction_vector = damaging_projectile.last_physics.velocity.normalized;
        transporter.command_batch.face_direction_quaternion = damaging_projectile.last_physics.velocity.to_quaternion();

    }

    


}

}