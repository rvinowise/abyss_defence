using System;
using System.Linq;


namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{

   

    public static int get_n_frames(
        this UnityEngine.Experimental.U2D.Animation.SpriteLibrary in_library
    ) {
        UnityEngine.Experimental.U2D.Animation.SpriteLibraryAsset asset = in_library.spriteLibraryAsset;
        String category = asset.GetCategoryNames().First();
        return asset.GetCategoryLabelNames(category).Count();

    }
    
}

}