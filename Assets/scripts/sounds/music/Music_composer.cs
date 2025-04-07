using System;
using System.Collections.Generic;
using rvinowise.contracts;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity.music {
    public class Music_composer: MonoBehaviour {

        //piano
        public Music_track melody_advance;
        public Music_track melody_retreat;
        public Music_track melody_tension;

        public Sequential_track_group sequential_melody;
        
        // public Sequential_track_group melody_advance_player;
        // public Sequential_track_group melody_retreat_player;
        // public Sequential_track_group melody_tension_player;
        
        //drums
        public Music_track drums_calm_track;
        public Music_track drums_tension_track;
        
        public Sequential_track_group drums_calm;
        public Sequential_track_group drums_tension;
        
        //bass
        public Music_track bass_calm_track;
        public Music_track bass_tension_track;
        
        public Sequential_track_group bass_calm;
        public Sequential_track_group bass_tension;

        
        public Track_player music_player;

        public State_analyzer state;

        public static Music_composer instance;

        private void Awake() {
            Contract.Assert(instance == null, "singleton");
            instance = this;
        }

        private void Start() {
            start_playing_default_tracks();
        }

        private void start_playing_default_tracks() {
            sequential_melody.set_next_track(melody_advance);
            //
            drums_calm.set_next_track(drums_calm_track);
            drums_tension.set_next_track(drums_tension_track);
            bass_calm.set_next_track(bass_calm_track);
            bass_tension.set_next_track(bass_tension_track);
        }

        void Update() {
            if (state.target != null) {
                if (state.enemy_is_close()) {
                    sequential_melody.set_next_track(melody_tension);
                    //switch_melody(melody_tension_player);
                    if (state.player_reloads_gun()) {
                        mute_drums();
                    }
                    else {
                        Track_player.transition_between_tracks(drums_calm, drums_tension, 0f);
                        Track_player.transition_between_tracks(bass_calm, bass_tension, 0f);
                    }
                }
                else { // enemy is not close
                    if (state.player_advanses()) {
                        sequential_melody.set_next_track(melody_advance);
                        //switch_melody(melody_advance_player);
                    }
                    else if (state.player_retreats()) {
                        sequential_melody.set_next_track(melody_retreat);
                        //switch_melody(melody_retreat_player);
                    }
                    Track_player.transition_between_tracks(drums_tension, drums_calm, 0f);
                    Track_player.transition_between_tracks(bass_tension, bass_calm, 0f);
                }
            }

        }

        // void switch_melody(Sequential_track_group needed_track) {
        //     var tracks = new[] {melody_advance_player, melody_retreat_player, melody_tension_player};
        //     foreach (var track in tracks) {
        //         if (track != needed_track) {
        //             track.fade(Fading_direction.OUT,0f);
        //         }
        //     }
        //     needed_track.fade(Fading_direction.IN,0f);
        // }

        void mute_drums() {
            drums_calm.fade(Fading_direction.OUT);
            drums_tension.fade(Fading_direction.OUT);
        }

        public void set_target(Transform target) {
            state.target = target;
        }

   
    }
}