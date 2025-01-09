namespace rvinowise.unity {

public interface IGun {

    void pull_trigger();
    void release_trigger();

    bool can_fire();

    int get_loaded_ammo();

}
}