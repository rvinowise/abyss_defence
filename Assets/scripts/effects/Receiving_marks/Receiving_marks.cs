using rvinowise.unity.effects.persistent_residue;
using UnityEngine;

using Point = UnityEngine.Vector2;

namespace rvinowise.unity.effects.marks {

public class Receiving_marks : MonoBehaviour
{

    public Persistent_residue_sprite_holder holder;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void add_mark(
        Point position,
        Quaternion rotation
    ) {
        holder.add_piece(position,rotation);
    }
}

}