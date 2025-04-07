using System.Collections.Generic;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity.music {
    public class Track_player: MonoBehaviour {


        public static float bpm = 120.0f;
        //public int beats_per_phrase = 16;

        private bool running = false;
        private int i_audio = 0;

        public List<Sequential_track_group> sequential_tracks;

        void Start()
        {
            init_audio_layers();
        }

        private void init_audio_layers() {
            
        }

        public static float time_to_prepare_phrase = 1f;
        void Update() {
            
            
        }
        

        public void set_next_track(Music_track track) {
            //next_track = track;
        }

        public void remove_layer() {
            
        }

        public static void transition_between_tracks(
            Sequential_track_group track_out,    
            Sequential_track_group track_in
        ) {
            track_out.fade(Fading_direction.OUT, 0);
            track_in.fade(Fading_direction.IN, 0);
        }
        public static void transition_between_tracks(
            Sequential_track_group track_out,    
            Sequential_track_group track_in,
            float duration
        ) {
            track_out.fade(Fading_direction.OUT, duration);
            track_in.fade(Fading_direction.IN, duration);
        }
    }
}