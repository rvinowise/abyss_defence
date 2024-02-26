using System.Collections.Generic;
using UnityEngine;
using rvinowise.contracts;


namespace rvinowise.unity {

/* When a new unit appears, it's remembered by this class, so that other units can target it */
public class Team_units_list: 
MonoBehaviour 
{
    ISet<Team> teams;// = new HashSet<Team>(); 

    public static Team_units_list instance{get;private set;}
    void Awake () {
        Contract.Requires(instance == null, "singleton");
        instance = this;

        teams = new HashSet<Team>(GetComponentsInChildren<Team>());
    }

    public void add_unit_of_team(Intelligence unit, Team team) {
        team.add_unit(unit);
        
    }
    
 


}
}