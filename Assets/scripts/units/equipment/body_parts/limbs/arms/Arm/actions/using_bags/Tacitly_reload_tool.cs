using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Tacitly_reload_tool: Action {

    private Arm arm;
    private Baggage bag;
    protected IReloadable reloadable;
    protected Ammo_compatibility ammo;
    
    public static Tacitly_reload_tool create(
        Baggage in_bag, 
        IReloadable in_reloadable,
        Ammo_compatibility ammo
    ) {
        var action = (Tacitly_reload_tool)object_pool.get(typeof(Tacitly_reload_tool));
        
        action.bag = in_bag;
        action.reloadable = in_reloadable;
        action.ammo = ammo;
        
        return action;
    }
    
    public override void update() {
        base.update();
        var moved_ammo = bag.fetch_ammo_qty(ammo, reloadable.get_lacking_ammo());
        reloadable.insert_ammunition(ammo, moved_ammo);
        mark_as_completed();
    }

   

}
}