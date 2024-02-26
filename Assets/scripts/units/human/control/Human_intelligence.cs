

namespace rvinowise.unity {

public class Human_intelligence:Intelligence {

    public Humanoid user;
    public Arm_pair arm_pair;

    public int current_equipped_set;
    protected override void Awake() {
        base.Awake();
        
        user = GetComponent<Humanoid>();
    }

}
}