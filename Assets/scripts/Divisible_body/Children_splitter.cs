using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using geometry2d;
using rvinowise.units;
using rvinowise.units.parts;
using rvinowise.rvi.contracts;

namespace rvinowise.units.parts {
public static class Children_splitter {
    internal static void split_children_groups(
        GameObject src_object,
        IEnumerable<GameObject> piece_objects) 
    {

        IChildren_groups_host user_of_equipment = src_object.GetComponent<IChildren_groups_host>();
        if (user_of_equipment == null) {
            return;
        }

        IList<IChildren_groups_host> piece_users = get_users_of_tools_from(piece_objects);


        distribute_children_to_pieces(user_of_equipment, piece_objects);

        Children_groups_host.Data_distributor.distribute_data_across(user_of_equipment, piece_users);

        foreach (var piece_object in piece_objects) {
            remove_unnecessary_tool_controllers(piece_object);
            //init_tool_controllers(piece_object);
        }

    }

    private static IList<IChildren_groups_host> get_users_of_tools_from(IEnumerable<GameObject> piece_objects) {
        IList<IChildren_groups_host> all_users = new List<IChildren_groups_host>();
        foreach (var piece in piece_objects) {
            all_users.Add(piece.GetComponent<IChildren_groups_host>());
        }
        return all_users;
    }

    
    

    private static void distribute_children_to_pieces(
        IChildren_groups_host IChildren_groups_host,
        IEnumerable<GameObject> piece_objects) {

        for (int i_children_group = 0;
            i_children_group < IChildren_groups_host.children_groups.Count;
            i_children_group++) 
        {
            Children_group distributed_children_controller =
                IChildren_groups_host.children_groups[i_children_group];
            foreach (units.Child tool in distributed_children_controller.children) {
                foreach (GameObject piece_object in piece_objects) {
                    if (tool_is_inside_object(tool, piece_object)) {
                        attach_tool_to_object(piece_object, i_children_group, tool);
                        break;
                    }
                }
            }
        }
    }

    

    private static void remove_unnecessary_tool_controllers(GameObject game_object) {
        /*Children_groups_host user_of_equipment = game_object.GetComponent<Children_groups_host>();
        user_of_equipment.remove_empty_controllers();*/
    }

    private static bool tool_is_inside_object(units.Child child, GameObject game_object) {
        PolygonCollider2D collider = game_object.GetComponent<PolygonCollider2D>();
        Contract.Requires(collider.pathCount == 1, "only simple polygons");
        if (
            System.Convert.ToBoolean(
                ClipperLib.Clipper.PointInPolygon(
                    Clipperlib_coordinates.float_coord_to_int(child.local_position),
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
        int i_children_group,
        Child child) 
    {
        IChildren_groups_host piece_children_groups_host =
            piece_object.GetComponent<IChildren_groups_host>();
        Children_group piece_children_group =
            piece_children_groups_host.children_groups[i_children_group];
        piece_children_group.add_child(child);
    }
}

}