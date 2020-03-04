using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using geometry2d;
using rvinowise.units;
using rvinowise.units.equipment;
using rvinowise.rvi.contracts;

namespace rvinowise.units.equipment {
public static class Tools_splitter {
    internal static void split_controllers_of_tools(
        GameObject src_object,
        IEnumerable<GameObject> piece_objects) 
    {

        User_of_equipment user_of_equipment = src_object.GetComponent<User_of_equipment>();
        if (!user_of_equipment) {
            return;
        }

        IList<User_of_equipment> piece_users = get_users_of_tools_from(piece_objects);

        copy_equipment_controllers(user_of_equipment, piece_users);

        distribute_tools_to_pieces(user_of_equipment, piece_objects);

        User_of_equipment.Data_distributor.distribute_data_across(user_of_equipment, piece_users);

        foreach (var piece_object in piece_objects) {
            remove_unnecessary_tool_controllers(piece_object);
            //init_tool_controllers(piece_object);
        }

    }

    private static IList<User_of_equipment> get_users_of_tools_from(IEnumerable<GameObject> piece_objects) {
        IList<User_of_equipment> all_users = new List<User_of_equipment>();
        foreach (var piece in piece_objects) {
            all_users.Add(piece.GetComponent<User_of_equipment>());
        }
        return all_users;
    }

    private static void copy_equipment_controllers(
        User_of_equipment src_user_of_equipment, 
        IList<User_of_equipment> piece_users) 
    {
        foreach (var dst_user_of_equipment in piece_users) {
            Contract.Requires(!dst_user_of_equipment.equipment_controllers.Any());
            dst_user_of_equipment.add_equipment_controllers_after(src_user_of_equipment);
        }
    }

    private static void distribute_tools_to_pieces(
        User_of_equipment user_of_equipment,
        IEnumerable<GameObject> piece_objects) {

        for (int i_tool_controller = 0;
            i_tool_controller < user_of_equipment.equipment_controllers.Count;
            i_tool_controller++) 
        {
            Equipment_controller distributed_tool_controller =
                user_of_equipment.equipment_controllers[i_tool_controller];
            foreach (units.Child tool in distributed_tool_controller.tools) {
                foreach (GameObject piece_object in piece_objects) {
                    if (tool_is_inside_object(tool, piece_object)) {
                        attach_tool_to_object(piece_object, i_tool_controller, tool);
                        break;
                    }
                }
            }
        }
    }

    

    private static void remove_unnecessary_tool_controllers(GameObject game_object) {
        User_of_equipment user_of_equipment = game_object.GetComponent<User_of_equipment>();
        user_of_equipment.remove_empty_controllers();
    }

    private static bool tool_is_inside_object(units.Child child, GameObject game_object) {
        PolygonCollider2D collider = game_object.GetComponent<PolygonCollider2D>();
        Contract.Requires(collider.pathCount == 1, "only simple polygons");
        if (
            System.Convert.ToBoolean(
                ClipperLib.Clipper.PointInPolygon(
                    Clipperlib_coordinates.float_coord_to_int(child.attachment),
                    Clipperlib_coordinates.float_coord_to_int(new Polygon(collider.GetPath(0)))
                )
            )
        ) {
            return true;
        }
        return false;
    }

    private static void attach_tool_to_object(
        GameObject piece_object,
        int i_tool_controller,
        Child child) 
    {
        User_of_equipment piece_user_of_equipment =
            piece_object.GetComponent<User_of_equipment>();
        Equipment_controller piece_tool_controller =
            piece_user_of_equipment.equipment_controllers[i_tool_controller];
        piece_tool_controller.add_tool(child);
    }
}

}