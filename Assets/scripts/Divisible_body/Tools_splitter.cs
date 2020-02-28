using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using UnityEditor;
using geometry2d;
using rvinowise.units;
using rvinowise.units.equipment;

namespace rvinowise.units.equipment {
public static class Tools_splitter {
    internal static void split_controllers_of_tools(
        GameObject src_object,
        IEnumerable<GameObject> piece_objects) {

        User_of_equipment user_of_equipment = src_object.GetComponent<User_of_equipment>();
        if (!user_of_equipment) {
            return;
        }

        copy_equipment_controllers(user_of_equipment, piece_objects);

        distribute_tools_to_pieces(user_of_equipment, piece_objects);

        foreach (var piece_object in piece_objects) {
            remove_unnecessary_tool_controllers(piece_object);
            init_tool_controllers(piece_object);
        }

    }

    private static void distribute_tools_to_pieces(
        User_of_equipment user_of_equipment,
        IEnumerable<GameObject> piece_objects) {

        for (int i_tool_controller = 0;
            i_tool_controller < user_of_equipment.equipment_controllers.Count;
            i_tool_controller++) {
            IEquipment_controller distributed_tool_controller =
                user_of_equipment.equipment_controllers[i_tool_controller];
            foreach (units.Tool tool in distributed_tool_controller.tools) {
                foreach (GameObject piece_object in piece_objects) {
                    // each collider must have only one path (simple polygon)
                    if (tool_is_inside_object(tool, piece_object)) {
                        attach_tool_to_object(piece_object, i_tool_controller, tool);
                        break;
                    }
                }
            }
        }
    }

    private static void copy_equipment_controllers(
        User_of_equipment src_user_of_equipment, 
        IEnumerable<GameObject> piece_objects) 
    {
        foreach (var piece in piece_objects) {
            var dst_user_of_equipment = piece.GetComponent<User_of_equipment>();
            Contract.Requires(!dst_user_of_equipment.equipment_controllers.Any());
            dst_user_of_equipment.copy_equipment_controllers_from(src_user_of_equipment);
        }
    }

    private static void remove_unnecessary_tool_controllers(GameObject game_object) {
        User_of_equipment user_of_equipment = game_object.GetComponent<User_of_equipment>();
        user_of_equipment.remove_empty_controllers();
    }

    private static void init_tool_controllers(GameObject game_object) {
        User_of_equipment user_of_equipment =
            game_object.GetComponent<User_of_equipment>();
        user_of_equipment.init_equipment_controllers();
    }

    private static bool tool_is_inside_object(units.Tool tool, GameObject game_object) {
        PolygonCollider2D collider = game_object.GetComponent<PolygonCollider2D>();
        if (
            System.Convert.ToBoolean(
                ClipperLib.Clipper.PointInPolygon(
                    Clipperlib_coordinates.float_coord_to_int(tool.attachment),
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
        Tool tool) {
        User_of_equipment piece_user_of_equipment =
            piece_object.GetComponent<User_of_equipment>();
        IEquipment_controller piece_tool_controller =
            piece_user_of_equipment.equipment_controllers[i_tool_controller];
        piece_tool_controller.add_tool(tool);
    }
}

}