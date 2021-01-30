using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace rvinowise.unity.music {
    public class Music_player: MonoBehaviour {

        public Music_track track_advance;
        public Music_track track_retreat;
        public Music_track track_tension;

        public Music_track current_track;
        private Music_phrase current_phrase;
        private Music_phrase next_phrase;
        private Music_track next_track;


        private AudioSource audio_source;

        public float bpm = 100.0f;
        public int beats_per_phrase = 16;
        private AudioClip[] clips = new AudioClip[2];
        private AudioSource[] audio_sources = new AudioSource[2];

        private double next_phrase_time;
        private AudioSource[] audioSources = new AudioSource[2];
        private bool running = false;
        private int i_audio = 0;

        public State_analyzer state;

        void Start()
        {
            next_phrase_time = AudioSettings.dspTime + 2.0f;
            init_audio_sources();
            //transition_to_track(current_track);
        }

        private void init_audio_sources() {
            for (int i = 0; i < 2; i++)
            {
                GameObject child = new GameObject("Audio_source");
                child.transform.parent = gameObject.transform;
                audioSources[i] = child.AddComponent<AudioSource>();
            }
        }

        private float time_to_prepare_phrase = 1f;
        void Update() {

            if (state.enemy_is_close()) {
                set_next_track(track_tension);
            } else {
                if (state.player_advanses()) {
                    set_next_track(track_advance);
                } else if (state.player_retreats()) {
                    set_next_track(track_retreat);
                }
            }
            

            if (is_time_to_prepare_next_phrase())
            {
                Debug.Log("is_time_to_prepare_next_phrase");
                schedule_next_phrase();

                next_phrase_time += 60.0f / bpm * 16;
            }
        }

        
        
        private bool is_time_to_prepare_next_phrase() {
            
            double music_time = AudioSettings.dspTime;
            return music_time + time_to_prepare_phrase > next_phrase_time;
        }
       
        private void schedule_next_phrase() {
            if ((next_track != null)&&(next_track != current_track)) {
                current_track = next_track;
                next_track = null;
                audioSources[i_audio].clip = current_track.start_from_first_phrase().clip;
            } else {
                audioSources[i_audio].clip = current_track.goto_next_phrase().clip;
            }
            audioSources[i_audio].PlayScheduled(next_phrase_time);
            i_audio = 1 - i_audio;
            Debug.Log("i_audio=" + i_audio + " next_phrase_time=" + next_phrase_time + "i_phrase="+current_track.i_current_phrase);
            
        }

        public void set_next_track(Music_track track) {
            next_track = track;
            
            //current_track.
        }
    }
}