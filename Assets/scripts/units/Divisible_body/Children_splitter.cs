using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;

namespace rvinowise.unity {

public partial class Divisible_body {
public static class Children_splitter {
    internal static void split_children_groups(
        IChildren_groups_host src_host_of_children,
        List<Divisible_body> piece_objects) 
    {
        
        distribute_children_to_pieces(src_host_of_children, piece_objects);
        destroy_undistributed_tools(src_host_of_children);
        
        Data_distributor.distribute_data_across(
            src_host_of_children, 
            get_users_of_tools_from(piece_objects)
        );

        Children_simplifier.simplify_pieces_without_children(piece_objects);
    }

    private static void connect_devices_to_intelligence(List<Divisible_body> piece_objects) {
        foreach (var piece in piece_objects) {
            if (piece.GetComponent<Intelligence>() is {} intelligence) {
                intelligence.init_devices();
            }
        }
    }

    private static IList<IChildren_groups_host> get_users_of_tools_from(IEnumerable<Divisible_body> piece_objects) {
        IList<IChildren_groups_host> all_users = new List<IChildren_groups_host>();
        foreach (var piece in piece_objects) {
            all_users.Add(piece.GetComponent<IChildren_groups_host>());
        }
        return all_users;
    }

    
    

    public static void distribute_children_to_pieces(
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

    public static void distribute_children_to_pieces(
        List<Polygon_with_tools> polygons_with_tools
    ) {
        foreach (var polygon_with_tools in polygons_with_tools) {
            polygon_with_tools.divisible_body.children_groups
        }
    }


    public class Polygon_with_tools {
        public Polygon polygon;
        //public Dictionary<IChildren_group, IChild_of_group> group_to_tools;
        public List<List<IChild_of_group>> tools;
        public bool attached_to_something;
        public Divisible_body divisible_body;
        public Polygon_with_tools(Polygon polygon, int tool_groups_amount) {
            tools = new List<List<IChild_of_group>>(tool_groups_amount);
            this.polygon = polygon;
            attached_to_something = false;
        }
    }
    public static List<Polygon_with_tools> distribute_children_to_polygons(
        IChildren_groups_host children_groups_host,
        List<Polygon> piece_polygons
    ) {
        var tool_groups_amount = children_groups_host.children_groups.Count;
        var result = polygons_to_polygon_with_tools(piece_polygons, tool_groups_amount);
        
        for (int i_children_group = 0;
             i_children_group < tool_groups_amount;
             i_children_group++) 
        {
            IChildren_group distributed_children_group =
                children_groups_host.children_groups[i_children_group];

            foreach (IChild_of_group tool in distributed_children_group.children_stashed_from_copying) {
                Contract.Requires(tool!= null, "children prepared to distribution should be valid");
                foreach (var polygon_with_tools in result) {
                    if (tool_is_inside_polygon(tool, polygon_with_tools.polygon)) {
                        polygon_with_tools.tools[i_children_group].Add(tool);
                        
                        if (distributed_children_group is Attachment_points_group) {
                            polygon_with_tools.attached_to_something = true;
                        }
                        
                        break;
                    }
                }
            }
        }
        return result;
    }

    public static List<Polygon_with_tools> polygons_to_polygon_with_tools(
        IEnumerable<Polygon> polygons,
        int tools_amount
    ) 
    {
        List<Polygon_with_tools> result = new List<Polygon_with_tools>();
        var tool_groups_amount = tools_amount;
        foreach (var polygon in polygons) {
            var polygon_with_tools = new Polygon_with_tools(polygon, tool_groups_amount);
            result.Add(polygon_with_tools);
        }
        return result;
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
            tool_is_inside_polygon(child, new Polygon(collider.GetPath(0)))
        ) {
            return true;
        }
        return false;
    }
    private static bool tool_is_inside_polygon(IChild_of_group child, Polygon polygon) {
        if (
            System.Convert.ToBoolean(
                ClipperLib.Clipper.PointInPolygon(
                    Clipperlib_coordinates.float_coord_to_int(child.transform.localPosition),
                    Clipperlib_coordinates.float_coord_to_int(polygon)
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
        IChildren_group piece_children_group =
            piece_object.children_groups[i_children_group];
        piece_children_group.add_child(child);
    }
}

}
}