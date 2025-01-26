using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity.music {
    public class Music_composer: MonoBehaviour {

        public Music_track melody_advance;
        public Music_track melody_retreat;
        public Music_track melody_tension;

        public Music_track drums_calm;
        public Music_track drums_tension;

        public Music_track bass_calm;
        public Music_track bass_tension;


        public Music_player music_player;

        public State_analyzer state;



        void Update() {

            if (state.enemy_is_close()) {
                music_player.set_next_track(melody_tension);
            } else {
                if (state.player_advanses()) {
                    music_player.set_next_track(melody_advance);
                } else if (state.player_retreats()) {
                    music_player.set_next_track(melody_retreat);
                }
            }

        }

   
    }
}