using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Direct_projectile_launcher: 
    MonoBehaviour,
    ITeam_member,
    IUsed_by_intelligence
{
    
    public Rigidbody2D projectile_prefab;
    public float projectile_force = 18f;
    public Transform muzzle;
    public Transform spark_prefab;
    public Tool tool;
    public Team team;
    public Intelligence intelligence;
    
    public AudioClip shot_sound;
    public AudioSource audio_source;

    private void Start() {
        if (GetComponentInParent<Intelligence>() is {} intelligence) {
            attach_to_team(team);
            attach_to_intelligence(intelligence);
        }
    }
    
    public void attach_to_team(Team in_team) {
        this.team = in_team;
    }
    public void attach_to_intelligence(Intelligence in_intelligence) {
        this.intelligence = in_intelligence;
    }

    public Rigidbody2D get_projectile() {
        Rigidbody2D new_projectile = projectile_prefab.instantiate<Rigidbody2D>(
            muzzle.position, muzzle.rotation
        );
        if (new_projectile.GetComponent<Targetable>() is { } targetable_projectile) {
            team.add_targetable(targetable_projectile);
        }
        if (new_projectile.GetComponent<Damage_dealer>() is { } damage_dealer) {
            if (intelligence == null) {
                Debug.LogError($"intelligence is null in {gameObject.name}, {GetType().FullName}");
            } else {
                damage_dealer.set_attacker(intelligence.transform);
            }
        }
        
        
        Transform new_spark = spark_prefab.get_from_pool<Transform>();
        new_spark.copy_physics_from(muzzle);
        
        return new_projectile;
    }
    
    public Rigidbody2D fire() {
        var new_projectile = get_projectile();
        if (new_projectile == null) {
            return null;
        }
        //new_projectile.damage_dealer?.set_attacker(tool.main_holding.holding_hand.arm.pair.user.transform);
        
        propell_projectile(new_projectile);
        
        if ((audio_source != null)&&(shot_sound != null)) {
            audio_source.PlayOneShot(shot_sound);
        }
        
        return new_projectile;
    }
    
    public void propell_projectile(Rigidbody2D projectile) {
        projectile.AddForce(transform.rotation.to_vector() * projectile_force, ForceMode2D.Impulse);
        //projectile.store_last_physics();
    }
    
}
}