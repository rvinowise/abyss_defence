using UnityEngine;


namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{
    
    public static float get_clip_length(
        this Animator anim, 
        //string clipName
        int clip_id
    ) {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips) { 
            /* if(clip.clipName == clipName) 
                return clip.length; */
            if(clip.GetInstanceID() == clip_id) 
                return clip.length;
        }
        return 0.0f;
    }
    
    
}

}