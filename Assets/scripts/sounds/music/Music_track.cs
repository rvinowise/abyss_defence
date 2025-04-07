using System;
using System.Linq;


namespace rvinowise.unity.music {
    /* progressing melody when doing one thing for a long time,
    eg: "moving_forward", "seeing_enemies" */
    [Serializable]
    public class Music_track {
        public Music_phrase[] phrases; 

        public int i_current_phrase;

        public Music_phrase start_from_first_phrase() {
            i_current_phrase = 0;
            return phrases[i_current_phrase];
        }
        public Music_phrase goto_next_phrase() {
            if (++i_current_phrase == phrases.Length){
                i_current_phrase = 0;
            }
            return phrases[i_current_phrase];
        }

        public Music_phrase get_current_phrase() {
            return phrases[i_current_phrase];
        }
    }
}