using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms.strategy {

public abstract class Strategy {

    protected Arm arm;
    public Strategy next;

    protected Strategy(Arm in_arm) {
        arm = in_arm;
    }
    public abstract void update();
    public virtual void start() {}
    protected virtual void end() {}

    protected void start_next() {
        this.end();
        arm.strategy = next;
        next?.start();
    }

}
}