using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;

namespace rvinowise.unity {

public static class Children_splitter {
    internal static void split_children_groups(
        IChildren_groups_host src_host_of_children,
        List<Divisible_body> piece_objects) 
    {
        IList<IChildren_groups_host> piece_users = get_users_of_tools_from(piece_objects);
        
        distribute_children_to_pieces(src_host_of_children, piece_objects);
        destroy_undistributed_tools(src_host_of_children);

        Children_groups_host.Data_distributor.distribute_data_across(src_host_of_children, piece_users);

    }

    private static IList<IChildren_groups_host> get_users_of_tools_from(IEnumerable<Divisible_body> piece_objects) {
        IList<IChildren_groups_host> all_users = new List<IChildren_groups_host>();
        foreach (var piece in piece_objects) {
            all_users.Add(piece.GetComponent<IChildren_groups_host>());
        }
        return all_users;
    }

    
    

    private static void distribute_children_to_pieces(
        IChildren_groups_host children_groups_host,
        List<Divisible_body> piece_objects
    ) {
        for (int i_children_group = 0;
            i_children_group < children_groups_host.children_groups.Count;
            i_children_group++) 
        {
            IChildren_group distributed_children_group =
                children_groups_host.children_groups[i_children_group];
            foreach (IChild_of_group tool in distributed_children_group.children_stashed_from_copying) {
                Contract.Requires(tool!= null, "children prepared to distribution should be valid");
                foreach (Divisible_body piece_object in piece_objects) {
                    if (tool_is_inside_object(tool, piece_object)) {
                        attach_tool_to_object(piece_object, i_children_group, tool);
                        break;
                    }
                }
            }
        }
    }

    private static void destroy_undistributed_tools(IChildren_groups_host children_groups_host) {
        foreach(IChild_of_group tool in get_undistributed_tools(children_groups_host)) {
            GameObject.Destroy(tool.transform.gameObject);
        }
    }

    private static IEnumerable<IChild_of_group> get_undistributed_tools(IChildren_groups_host children_groups_host) {
        List<IChild_of_group> result = new List<IChild_of_group>();
        foreach (var group in children_groups_host.children_groups){
            foreach (IChild_of_group tool in group.children_stashed_from_copying) {
                if (tool.transform.parent == null) {
                    result.Add(tool);
                }
            }
        }
        return result.AsReadOnly();
    }


    private static bool tool_is_inside_object(IChild_of_group child, Divisible_body game_object) {
        PolygonCollider2D collider = game_object.GetComponent<PolygonCollider2D>();
        Contract.Requires(collider.pathCount == 1, "only simple polygons");
        if (
            System.Convert.ToBoolean(
                ClipperLib.Clipper.PointInPolygon(
                    Clipperlib_coordinates.float_coord_to_int(child.transform.localPosition),
                    Clipperlib_coordinates.float_coord_to_int(new Polygon(collider.GetPath(0)))
                )
            )
        ) {
            return true;
        }
        return false;
    }

    private static void attach_tool_to_object(
        Divisible_body piece_object,
        int i_children_group,
        IChild_of_group child) 
    {
        IChildren_groups_host piece_children_groups_host =
            piece_object.GetComponent<IChildren_groups_host>();
        Children_group piece_children_group =
            piece_children_groups_host.children_groups[i_children_group];
        piece_children_group.add_child(child);
    }
}

}