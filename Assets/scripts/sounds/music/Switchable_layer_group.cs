using System.Collections.Generic;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Assertions.Must;


namespace rvinowise.unity.music {

    
    public class Switchable_layer_group: MonoBehaviour {


        public List<Sequential_track_group> music_layers;



        public void set_next_track(Music_track track) {
            //next_track = track;
        }

        public void quickly_switch_track(Sequential_track_group played_music_layer) {
            foreach (var music_layer in music_layers) {
                music_layer.mute();
            }
            played_music_layer.unmute();
        }

    }
}