using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using geometry2d;
using units;
using Tool = units.Tool;


public static class Tools_splitter
{
    internal static void split_controllers_of_tools(
        GameObject src_object,
        IEnumerable<GameObject> piece_objects) {
        User_of_tools user_of_tools = src_object.GetComponent<User_of_tools>();
        if (!user_of_tools) {
            return;
        }
        for (int i_tool_controller = 0; 
            i_tool_controller <  user_of_tools.tool_controllers.Count;
            i_tool_controller++)
        {
            ITool_controller distributed_tool_controller = 
                user_of_tools.tool_controllers[i_tool_controller];
            foreach(units.Tool tool in distributed_tool_controller.tools) {
                foreach(GameObject piece_object in piece_objects) { 
                    // each collider must have only one path (simple polygon)
                    if (tool_is_inside_object(tool, piece_object)) {
                        attach_tool_to_object(piece_object, i_tool_controller, tool);
                        break;
                    }
                }
            }
        }

        foreach (var piece_object in piece_objects) {
            remove_unnecessary_tool_controllers(piece_object);
            init_tool_controllers(piece_object);
        }

    }

    private static void remove_unnecessary_tool_controllers(GameObject game_object) {
        User_of_tools user_of_tools = game_object.GetComponent<User_of_tools>();
        user_of_tools.remove_empty_controllers();
    }

    private static void init_tool_controllers(GameObject game_object) {
            User_of_tools user_of_tools =
                game_object.GetComponent<User_of_tools>();
            user_of_tools.init_tool_controllers();
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

    private static void attach_tool_to_object(GameObject piece_object, int i_tool_controller, Tool tool) {
        User_of_tools piece_user_of_tools =
            piece_object.GetComponent<User_of_tools>();
        ITool_controller piece_tool_controller =
            piece_user_of_tools.tool_controllers[i_tool_controller];
        piece_tool_controller.add_tool(tool);
    }
}

