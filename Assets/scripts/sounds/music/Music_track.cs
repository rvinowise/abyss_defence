using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace rvinowise.unity.music {
    /* progressing melody when doing one thing for a long time,
    eg: "moving_forward", "seeing_enemies" */
    [Serializable]
    public class Music_track {
        public string name;
        public Music_phrase[] phrases; 

        public int i_current_phrase;

        public Music_phrase start_from_first_phrase() {
            i_current_phrase = 0;
            return phrases[i_current_phrase];
        }
        public Music_phrase goto_next_phrase() {
            if (++i_current_phrase == phrases.Count()){
                i_current_phrase = 0;
            }
            return phrases[i_current_phrase];
        }
    }
}