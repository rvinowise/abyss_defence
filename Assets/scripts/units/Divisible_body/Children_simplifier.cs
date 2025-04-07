using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;

namespace rvinowise.unity {

public partial class Divisible_body {
public static class Children_simplifier {
    
    public static void simplify_pieces_without_children(List<Divisible_body> pieces) {
        foreach (var piece in pieces) {
            delete_childless_children_groups(piece);
            if (is_piece_without_children_groups(piece)) {
                destroy_destructable_piece(piece);
            }
        }
    }

    private static List<IChildren_group> children_marked_for_deletion = new List<IChildren_group>();
    public static void delete_childless_children_groups(Divisible_body piece) {
        for(int i_group=0;i_group< piece.children_groups.Count; i_group++) {
            var group = piece.children_groups[i_group];
            if (is_children_group_useless(group)) {
                children_marked_for_deletion.Add(group);
                // piece.children_groups.RemoveAt(i_group);
                // Object.Destroy(group as Component);
            }
        }

        foreach (var child in children_marked_for_deletion) {
            Object.Destroy(child as Component);
        }
        piece.children_groups.RemoveAll(
            child => children_marked_for_deletion.Contains(child)
        );
        
        
        children_marked_for_deletion.Clear();
    }
    
    public static bool is_children_group_useless(IChildren_group group) {
        return !group.get_children().Any();
    }
    public static bool is_piece_without_children_groups(Divisible_body piece) {
        return !piece.children_groups.Any();
    }
    
    public static bool is_piece_useless(Divisible_body piece) {
        bool piece_has_children = false;
        foreach (var children_group in piece.children_groups) {
            if (children_group.get_children().Any()) {
                piece_has_children = true;
                break;
            }
        }
        return !piece_has_children;
    }

    public static void destroy_activities_in_piece(Divisible_body piece) {
        Object.Destroy(piece.GetComponent<Targetable>());
        Object.Destroy(piece.GetComponent<Intelligence>());
        foreach (var children_group in piece.GetComponents<Abstract_children_group>()) {
            Object.Destroy(children_group);
        }
        piece.children_groups.Clear();
    }
    
    public static void destroy_destructable_piece(Divisible_body piece) {
        var damaged = piece.GetComponent<Damage_receiver>();
        damaged?.start_dying();
    }
}

}
}