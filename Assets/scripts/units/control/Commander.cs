using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;


namespace rvinowise.unity.units.control {

/* Global intelligence, controlling all units of a team */
public class Commander: 
MonoBehaviour 
{
    
    List<Intelligence> units; 

    public Transform enemy;

    public static Commander instance{get;private set;}
    void Awake () {
        Contract.Requires(instance == null, "singleton");
        instance = this;
    }


    public void on_unit_iddling(Strategic_intelligence in_intelligence) {
        in_intelligence.unit_commands.attack_target = enemy;
    }


}
}