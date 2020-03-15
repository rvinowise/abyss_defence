using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms.strategy {

public class Strategy_builder {

    public IList<Strategy> strategies;
    private IUse_strategies user;

    public Strategy_builder(IUse_strategies user) {
        this.user = user;
    }
    

    public Strategy_builder add<TStrategy>() {
        
        return this;
    }
}
}