using System;
using System.Collections;
using System.Collections.Generic;
using extesions;
using UnityEngine;
using rvinowise;
using Random = UnityEngine.Random;


namespace rvinowise.units.parts.weapons.guns {

public class Desert_eagle: Pistol {

    //public override float weight { set; get; } = 1f;
    public override float stock_length { get; } = 0f;

    private Saved_physics last_physics = new Saved_physics();


    void FixedUpdate()
    {
        //last_physics.velocity = rigid_body.velocity;
        last_physics.position = transform.position;
    }

    void Update() {
        
    }

    private static readonly int animation_fire = Animator.StringToHash("fire");

    private float frames_to_shell_ejection = 15f;

    public float frames_to_seconds(AnimatorStateInfo animation_state, float frames) {
        return frames / 60f / animation_state.speed;
    }

    private static float recoil_force = 1f;
    protected override GameObject fire() {
        animator.SetTrigger("fire");
        var new_projectile = base.fire();
        propell_projectile(new_projectile);
        
        StartCoroutine(
            delayed_action_coroutine(
                frames_to_seconds(
                    animator.GetNextAnimatorStateInfo(0), frames_to_shell_ejection
                ), 
                eject_bullet_shell
            )
        );
        
        //this.rigid_body.AddForce(-transform.rotation.to_vector()*recoil_force);
        this.recoil_receiver.velocity += -transform.rotation.to_vector() * recoil_force;
        
        return new_projectile;
    }

    private float projectile_force = 10f;

    private void propell_projectile(GameObject projectile) {
        Rigidbody2D rigid_body = projectile.GetComponent<Rigidbody2D>();
        rigid_body.AddForce(transform.rotation.to_vector() * projectile_force, ForceMode2D.Impulse);
    }

    /*void delayed_action(float time, Action action) {
        StartCoroutine(action);
    }*/
    
    IEnumerator delayed_action_coroutine(float time, Action action)
    {
        Debug.Log("test");
        yield return new WaitForSeconds(time);

        action();
    }
    private void eject_bullet_shell() {
        float ejection_force = 5f;
        GameObject new_shell = GameObject.Instantiate(
            bullet_shell, 
            shell_ejector.position,
            transform.rotation
        );
        Vector2 ejection_vector = transform.rotation * Vector2.down * ejection_force;
        Vector2 gun_vector = (Vector2)this.transform.position - last_physics.position;
        
        var rigidbody = new_shell.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(ejection_vector + gun_vector);
        rigidbody.AddTorque(-460f + Random.value*100f);
    }
}
}