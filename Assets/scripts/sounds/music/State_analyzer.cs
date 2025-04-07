using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.music {

    public class State_analyzer: MonoBehaviour {

        public Turning_element turning_player;
        public Transform player_transform {
            get {return turning_player.transform;}
        }

        public Intelligence player_intelligence;
        public Intelligence player;
        public Arm_pair player_arm_pair;
        
        public Transform target;


        private void Update() {
            target = get_target_from_aiming(player_arm_pair);
        }

        private Transform get_target_from_aiming(Arm_pair arm_pair) {
            var left_target = arm_pair.left_arm.get_target();
            var right_target = arm_pair.right_arm.get_target();

            var targets = (IEnumerable<Transform>) new[] {left_target, right_target};
            var closest_target = Finding_objects.find_closest_component(player_arm_pair.transform.position, targets);
            return closest_target;
        }

        public bool player_advanses() {
            if (target != null) {
                return Mathf.Abs(
                    turning_player.transform.rotation.to_degree().angle_to(
                        turning_player.transform.degrees_to(target.position)
                    )
                ) < 80f;
            }
            return false;
        }

        public bool player_retreats() {
            if (target != null) {
                return !player_advanses();
            }
            return false;
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

        public bool player_reloads_gun() {
            if (player_arm_pair.left_arm.actor.current_action == null) {
                Debug.LogError($"action of left_arm is null");
            }
            if (player_arm_pair.right_arm.actor.current_action == null) {
                Debug.LogError($"action of right_arm is null");
            }
            
            if (
                player_arm_pair.left_arm.actor.current_action?.is_part_of_action(typeof(Reload_pistol_simple)) == true
            ) {
                return true;
            }
            if (
                player_arm_pair.right_arm.actor.current_action?.is_part_of_action(typeof(Reload_pistol_simple)) == true
            ) {
                return true;
            }
            return false;
        }
        public bool player_reloads_both_guns() {
            if (
                player_arm_pair.left_arm.actor.current_action.GetType() == typeof(Reload_pistol_simple)
                &&
                player_arm_pair.right_arm.actor.current_action.GetType() == typeof(Reload_pistol_simple)
            ) {
                return true;
            }
            return false;
        }
    }
        
}