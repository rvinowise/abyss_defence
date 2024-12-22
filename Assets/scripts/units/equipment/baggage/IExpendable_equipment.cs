using System;
using System.Collections.Generic;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.contracts;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public interface IExpendable_equipment {
    //public Tool tool;


    bool withdraw_equipment_for_one_use();

}


}
 