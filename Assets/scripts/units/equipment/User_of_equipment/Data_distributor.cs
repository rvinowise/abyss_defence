using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts {

public partial class User_of_equipment {
    public static class Data_distributor {
        
        public static void distribute_data_across(
            User_of_equipment base_user,
            IEnumerable<User_of_equipment> other_users) 
        {
            IList<IList<Equipment_controller>> controllers_of_type = new List<IList<Equipment_controller>>();
            for(int i_base_controller = 0; 
                i_base_controller < base_user.equipment_controllers.Count; 
                i_base_controller++) 
            {
                controllers_of_type.Add(new List<Equipment_controller>());

                foreach (var other_user in other_users) {
                    Contract.Requires(
                        base_user.equipment_controllers.Count == other_user.equipment_controllers.Count
                    );
                    controllers_of_type[i_base_controller].Add(
                        other_user.equipment_controllers[i_base_controller]
                    );
                
                }
            }
            for (int i_type = 0; i_type < base_user.equipment_controllers.Count; i_type++) {
                base_user.equipment_controllers[i_type].distribute_data_across(controllers_of_type[i_type]);
            }
        }
        
    }
}
}