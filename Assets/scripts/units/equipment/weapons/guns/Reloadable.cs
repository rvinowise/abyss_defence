

using UnityEngine;


namespace rvinowise.unity {
public class Reloadable: 
    MonoBehaviour
    ,IReloadable 
{
    public AudioClip eject_magazine_sound;
    public AudioClip insert_magazine_sound;
    
    
    public int ammo_qty;
    public int max_ammo_qty;
    
    public Ammunition ammo_prefab;
    public Ammo_compatibility ammo_compatibility;
    
    
    public delegate void EventHandler();
    public event EventHandler on_ammo_changed = delegate{};

    
    public virtual void insert_ammunition(Ammo_compatibility ammo, int rounds_amount) {
        ammo_qty += rounds_amount;
        on_ammo_changed();
    }

    public void spend_ammo(int amount) {
        ammo_qty -= amount;
        on_ammo_changed();
    }

    public Ammo_compatibility get_ammo_compatibility() => ammo_compatibility;

    public int get_loaded_ammo() {
        return ammo_qty;
    }

    public int get_lacking_ammo() {
        return max_ammo_qty - ammo_qty;
    }

    // public int get_loaded_ammo() {
    //     
    // }
    //
    // public int get_lacking_ammo() {
    //     
    // }
    //
    // public void insert_ammunition(Ammo_compatibility ammo, int amount) {
    //     
    // }
}
}