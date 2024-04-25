using System;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[RequireComponent(typeof(ParticleSystem))]
public class Destroyable_particle_burst: MonoBehaviour {

    public List<ParticleSystem> particle_systems = new List<ParticleSystem>();
    private float longest_particle_system_lifetime;


    private void Awake() {
        
        foreach (var particle_system in particle_systems) {
            if (longest_particle_system_lifetime < particle_system.main.duration) {
                longest_particle_system_lifetime = particle_system.main.duration;
            }
        }
    }

    void Start() {
        Destroy(gameObject,longest_particle_system_lifetime);
    }

    
    
}

}