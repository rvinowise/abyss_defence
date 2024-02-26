using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

public partial class Leg2 {
    
    
    #region optimization
    /* faster calculation but not precise */
    public void hold_onto_ground_FAST(Vector2 holding_point) {
        tibia.direct_to(holding_point);
        femur.set_direction(
            femur.transform.degrees_to(holding_point)+
            (90f-femur.transform.sqr_distance_to(holding_point)*1440f)
        );
    }
    #endregion
    
    public new class Debug: Limb2.Debug {
        private readonly Leg2 leg;

        protected override Limb2 limb2 {
            get { return leg; }
        }

        public Debug(Leg2 parent):base(parent) {
            leg = parent;
        }

        
    }
    public Leg2.Debug debug {
        get { return _debug; }
        private set { _debug = value; }
    }
    private Leg2.Debug _debug;

    protected override Limb2.Debug debug_limb {
        get { return _debug; }
    }
    
}
}