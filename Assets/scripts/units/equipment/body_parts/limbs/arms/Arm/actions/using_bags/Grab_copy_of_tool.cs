using rvinowise.contracts;
using rvinowise.unity;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Grab_copy_of_tool: Grab_tool {

    protected IExpendable_equipment expendable_equipment;
    
    public static Grab_copy_of_tool create(
        Arm in_arm,
        Baggage in_bag, 
        Tool in_tool,
        IExpendable_equipment expendable_equipment
    ) {
        var action = (Grab_copy_of_tool)object_pool.get(typeof(Grab_copy_of_tool));
        action.add_actor(in_arm);
        
        action.hand = in_arm.hand;
        action.bag = in_bag;
        action.tool = in_tool;
        action.expendable_equipment = expendable_equipment;
        return action;
    }
    public Grab_copy_of_tool() {
        
    }


    protected override void take_new_tool() {
        if (expendable_equipment.withdraw_equipment_for_one_use()) {
            var withdrawn_tool = Object.Instantiate(tool);
            withdrawn_tool.gameObject.SetActive(true);
            hand.attach_holding_part(withdrawn_tool.main_holding);
        }
    }

   

}
}