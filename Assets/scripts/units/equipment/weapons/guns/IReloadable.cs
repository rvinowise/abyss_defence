

namespace rvinowise.unity {
public interface IReloadable {
    int get_loaded_ammo();
    int get_lacking_ammo();
    //void insert_ammunition(int amount);
    void insert_ammunition(Ammo_compatibility ammo, int amount);
    //Ammo_compatibility get_ammo_compatibility();
}
}