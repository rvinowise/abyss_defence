using System;
using System.Collections.Generic;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.contracts;
using UnityEngine.Serialization;


namespace rvinowise.unity {



public class Expendable_equipment: 
    MonoBehaviour,
    IExpendable_equipment 
{
    public int available_amount;
    public int one_use_amount = 1;

    public bool withdraw_equipment_for_one_use() {
        if (available_amount >= one_use_amount) {
            available_amount -= one_use_amount;
            return true;
        }
        return false;
    }
}

}
 