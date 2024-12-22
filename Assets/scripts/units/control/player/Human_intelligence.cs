

namespace rvinowise.unity {

public class Human_intelligence:Intelligence {

    public Humanoid user;
    public Arm_pair arm_pair;
    public Toolset_equipper toolset_equipper;
    public Supertool_user supertool_user;

    public override void Awake() {
        base.Awake();
        
        user = GetComponent<Humanoid>();
        toolset_equipper = GetComponent<Toolset_equipper>();
    }

}
}