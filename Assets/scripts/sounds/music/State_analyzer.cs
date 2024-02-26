using System;
using rvinowise.unity.extensions;
using UnityEngine;


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
            var closest_enemy = Object_finder.instance.get_closest_enemy(player);
            return closest_enemy.exists && closest_enemy.distance < close_distance;
        }
    }
        
}