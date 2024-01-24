namespace rvinowise.unity.units.parts.limbs.init{

public static class Initializer {

    public static void mirror(Limb2 dst, Limb2 src) {
        // the base direction_quaternion is to the right
        dst.segment1.mirror_from(src.segment1);
        dst.segment2.mirror_from(src.segment2);

        //dst.folding_direction = -src.folding_direction; //! it's not needed because it's inferred from the angles
    }
    
    public static void mirror(Limb3 dst, Limb3 src) {
        mirror((Limb2)dst, (Limb2)src);
        dst.segment3.mirror_from(src.segment3);
    }

    
    
}
}