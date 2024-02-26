using rvinowise.unity.extensions;



namespace rvinowise.unity {

public class Computer_spider: 
    Computer_intelligence
{

    public override void start_dying(Projectile damaging_projectile) {
        transporter.command_batch.moving_direction_vector = damaging_projectile.last_physics.velocity.normalized;
        transporter.command_batch.face_direction_quaternion = damaging_projectile.last_physics.velocity.to_quaternion();

    }

    


}

}