using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace rvinowise.unity {

public class Holding_place: MonoBehaviour
{
    public bool is_main;
    public Tool tool;
    public Hand_gesture grip_gesture = Hand_gesture.Relaxed;
    
    public Quaternion grip_direction_quaternion {
        get {
            return transform.localRotation;
            //return grip_direction.to_quaternion();
        }
    }
    public Vector2 place_on_tool {
        set { transform.localPosition = value; }
        get { return transform.localPosition; }
    }
    public Degree grip_direction {
        set { transform.localRotation = value.to_quaternion(); }
        get { return transform.localRotation.to_degree(); }
    }

    
    
    public Hand holding_hand { get; private set; }


    public Vector2 position => transform.position;

    public Quaternion rotation => transform.rotation;
    

    public static Holding_place main(Tool in_tool) {
        Holding_place holding_place = Holding_place.create(in_tool.transform);
        holding_place.is_main = true;
        holding_place.grip_gesture = Hand_gesture.Grip_of_vertical;

        return holding_place;
    }
    public static Holding_place secondary(Tool in_tool) {
        Holding_place holding_place = Holding_place.create(in_tool.transform);
        holding_place.is_main = false;
        holding_place.grip_gesture = Hand_gesture.Grip_of_vertical;

        return holding_place;
    }

    public static Holding_place create(Transform in_parent) {
        Tool parent_tool =  in_parent.GetComponent<Tool>();
        Contract.Requires(parent_tool  != null, "the parent of a Holding_place must be a Tool");
        
        GameObject game_object = new GameObject();
        game_object.transform.parent = in_parent;
        Holding_place holding_place = game_object.add_component<Holding_place>();
        holding_place.tool = parent_tool;
        
        return holding_place;
    }
    public static Holding_place create(
        bool _is_main,
        Hand_gesture _hand_gesture,// = Hand_gesture.Relaxed,
        Vector2 _position,// = Vector2.zero,
        Quaternion _rotation// = Quaternion.identity
    ) {
        GameObject game_object = new GameObject();
        Holding_place holding_place = game_object.add_component<Holding_place>();
        holding_place.is_main = _is_main;
        holding_place.grip_gesture = _hand_gesture;
        holding_place.transform.localPosition = _position;
        holding_place.transform.localRotation = _rotation;

        return holding_place;
    }
    

    
    public void set_parenting_for_holding(Hand in_hand) {
        holding_hand = in_hand;
        if (is_main) {
            if (tool.transform == transform) {
                simply_attach_to_parent(in_hand);
            }
            else {
                calculate_parenting_based_on_holding_place(in_hand);
            }
        }
    }

    private void calculate_parenting_based_on_holding_place(Hand in_hand) {
        tool.transform.SetParent(in_hand.transform, false);

        tool.transform.localPosition = transform.localPosition;

        tool.transform.localRotation = transform.localRotation;
        

    }

    private void simply_attach_to_parent(Hand in_hand) {
        transform.SetParent(in_hand.valuable_point.transform, false);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector2.zero;
    }

    public void drop_from_hand() {
        holding_hand = null;
        if (is_main) {
            tool.transform.SetParent(null, true);

            tool.drop_from_hand();
        }
    }
    
}
}