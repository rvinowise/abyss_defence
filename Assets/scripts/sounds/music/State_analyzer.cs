using System;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.management;
using rvinowise.unity.units;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs;
using UnityEngine;
using UnityEngine.Audio;

namespace rvinowise.unity.music {

    [Serializable]
    public class State_analyzer {

        public Turning_element turning_player;
        public Transform player_transform {
            get {return turning_player.transform;}
        }

        public Intelligence player_intelligence;
        public Intelligence player;
        public Transform target;
        
        public bool player_advanses() {
            return Mathf.Abs(
                turning_player.transform.rotation.to_degree().angle_to(
                    turning_player.transform.degrees_to(target.position)
                )
            ) < 45f;
        }

        public bool player_retreats() {

            return !player_advanses();
        }

        public bool enemy_is_close() {
            const float close_distance = 2f;
            return Object_finder.instance.get_closest_enemy(player).distance < close_distance;
        }
    }
        
}