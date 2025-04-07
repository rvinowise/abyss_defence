#define RVI_DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Experimental.U2D.Animation;
using System.Linq;
using rvinowise.rvi;
using rvinowise.unity.extensions.pooling;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public interface ILeaving_persistent_residue_on_texture: 
ILeaving_persistent_residue 
{
    
    void prepare_for_taking_photo();
    bool is_residue_on_texture_holder(Persistent_residue_texture_holder texture_holder);

}

}