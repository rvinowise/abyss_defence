﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts {

public partial class Children_groups_host {
    public static class Data_distributor {
        
        public static void distribute_data_across(
            IChildren_groups_host base_user,
            IEnumerable<IChildren_groups_host> other_users) 
        {
            IList<IList<Children_group>> controllers_of_type = new List<IList<Children_group>>();
            for(int i_base_controller = 0; 
                i_base_controller < base_user.children_groups.Count; 
                i_base_controller++) 
            {
                controllers_of_type.Add(new List<Children_group>());

                foreach (var other_user in other_users) {
                    Contract.Requires(
                        base_user.children_groups.Count == other_user.children_groups.Count
                    );
                    controllers_of_type[i_base_controller].Add(
                        other_user.children_groups[i_base_controller]
                    );
                
                }
            }
            for (int i_type = 0; i_type < base_user.children_groups.Count; i_type++) {
                base_user.children_groups[i_type].distribute_data_across(controllers_of_type[i_type]);
            }
        }
        
    }
}
}