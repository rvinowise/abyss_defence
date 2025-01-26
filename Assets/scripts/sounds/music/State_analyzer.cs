using System;
using System.Linq;
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
            ) < 80f;
        }

        public bool player_retreats() {

            return !player_advanses();
        }

        public bool enemy_is_close() {
            const float close_distance = 5f;
            var closest_enemy = player.team.get_enemies_closest_to(player.transform.position);
            if (
                (closest_enemy.Any())&&
                (closest_enemy.First().Item2 < close_distance)
                )
            {
                return true;
            }
            return false;

            //return closest_enemy.exists && closest_enemy.distance < close_distance;
        }
    }
        
}