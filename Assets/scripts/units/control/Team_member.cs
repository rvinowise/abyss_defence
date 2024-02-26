// using System.Collections.Generic;
// using System;
// using UnityEngine;
// using UnityEngine.Serialization;
//
//
// namespace rvinowise.unity {
//
// [Serializable]
// public class Team_member: MonoBehaviour {
//
//     public Team team;
//     
//     public delegate void EvendHandler(Team_member unit);
//     public event EvendHandler on_destroyed;
//     void Awake() {
//         team.add_unit(this);
//     }
//     
//     public void notify_about_appearance() {
//         team.add_unit(this);
//     }
//     
//     public void notify_about_destruction() {
//         team.remove_unit(this);
//         on_destroyed?.Invoke(this);
//     }
//     
//     
//
// }
//
//
// }