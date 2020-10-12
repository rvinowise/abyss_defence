using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using extensions.pooling;
using rvinowise.rvi.contracts;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public static partial class Unity_extension
{

   

    public static int get_n_frames(
        this SpriteLibrary in_library
    ) {
        SpriteLibraryAsset asset = in_library.spriteLibraryAsset;
        String category = asset.GetCategoryNames().First();
        return asset.GetCategoryLabelNames(category).Count();

    }
    
}
