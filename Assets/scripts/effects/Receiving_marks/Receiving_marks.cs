using UnityEngine;

using Point = UnityEngine.Vector2;

namespace rvinowise.unity {

public class Receiving_marks : MonoBehaviour
{

    public Persistent_residue_sprite_holder holder;


    public void add_mark(
        Point position,
        Quaternion rotation
    ) {
        holder.add_piece(position,rotation);
    }
}

}