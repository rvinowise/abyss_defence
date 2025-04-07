using System;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity.music {

    public enum Music_layer_type {
        DRUMS,
        BASS,
        MELODY
    }
    public enum Fading_direction {
        NO, IN, OUT 
    }
    public class Sequential_track_group: MonoBehaviour {

        public Music_track current_track;
        //private Music_phrase current_phrase;
        private Music_phrase next_phrase;
        private Music_track next_track;


        public AudioSource audio_source_prefab;

        private AudioClip[] clips = new AudioClip[2];
        private AudioSource[] audio_sources = new AudioSource[2];

        private bool running = false;
        private int i_audio;

        public float volume = 1;

        void Start()
        {
            init_audio_sources();
            next_phrase_time = AudioSettings.dspTime + 2.0f;
        }

        
        public Fading_direction fading;
        public float fading_speed;

        private double next_phrase_time;
        private void Update() {
            update_fading();
            if (is_time_to_prepare_next_phrase())
            {
                Debug.Log("MUSIC: is_time_to_prepare_next_phrase");
                var scheduled_phrase = schedule_next_phrase(next_phrase_time);

                next_phrase_time += 60.0f / Track_player.bpm * scheduled_phrase.n_beats;
            }
        }
        
        private bool is_time_to_prepare_next_phrase() {
            double music_time = AudioSettings.dspTime;
            return music_time + Track_player.time_to_prepare_phrase > next_phrase_time;
        }

        private void update_fading() {
            if (fading==Fading_direction.IN) {
                change_audiosources_volume(fading_speed * Time.deltaTime);
                if (get_audiosources_volume() >= volume) {
                    fading = Fading_direction.NO;
                }
            } else if (fading==Fading_direction.OUT) {
                change_audiosources_volume(-fading_speed * Time.deltaTime);
                if (get_audiosources_volume() <= 0) {
                    fading = Fading_direction.NO;
                }
            }
        }

        private void change_audiosources_volume(float volume) {
            audio_sources[0].volume += volume;
            audio_sources[1].volume += volume;
        }
        private void set_audiosources_volume(float volume) {
            audio_sources[0].volume = volume;
            audio_sources[1].volume = volume;
        }
        private float get_audiosources_volume() {
            Contract.Assert(
                audio_sources[0].volume.Equals(audio_sources[1].volume), 
                "both audiosources should have the samew volume, they are just against lags");
            return audio_sources[0].volume;
        }

        private void init_audio_sources() {
            GameObject first_audiosource = audio_source_prefab.instantiate<AudioSource>().gameObject;
            first_audiosource.transform.parent = gameObject.transform;
            audio_sources[0] = first_audiosource.GetComponent<AudioSource>();
            
            
            GameObject second_audiosource = audio_source_prefab.instantiate<AudioSource>().gameObject;
            second_audiosource.transform.parent = gameObject.transform;
            audio_sources[1] = second_audiosource.GetComponent<AudioSource>();
            
            set_audiosources_volume(volume);
        }

        private Music_phrase schedule_next_phrase(double next_phrase_time) {
            Music_phrase scheduled_phrase;
            if ((next_track != null)&&(next_track != current_track)) {
                current_track = next_track;
                next_track = null;
                scheduled_phrase = current_track.start_from_first_phrase();
            } else { //keep looping through the ongoing track
                scheduled_phrase = current_track.goto_next_phrase();
            }
            audio_sources[i_audio].clip = scheduled_phrase.clip;
            audio_sources[i_audio].PlayScheduled(next_phrase_time);
            i_audio = 1 - i_audio;
            
            Debug.Log("MUSIC: i_audio=" + i_audio + " next_phrase_time=" + next_phrase_time + "i_phrase="+current_track.i_current_phrase);
            return scheduled_phrase;
        }

        public void set_next_track(Music_track track) {
            next_track = track;
        }

        public void quickly_switch_track(Music_track track) {
            
        }

        //private float previous_volume = 0;
        public void mute() {
            foreach (var audio_source in audio_sources) {
                audio_source.volume = 0;
            }
        }
        public void unmute() {
            foreach (var audio_source in audio_sources) {
                audio_source.volume = volume;
            }
        }


        public void fade(Fading_direction direction, float duration = 0) {
            Contract.Assert(duration >= 0);
            if (duration > 0) {
                fading = direction;
                fading_speed = volume / duration;
            }
            else if (direction == Fading_direction.IN) {
                set_audiosources_volume(volume);
                fading = Fading_direction.NO;
            }
            else if (direction == Fading_direction.OUT) {
                set_audiosources_volume(0);
                fading = Fading_direction.NO;
            }
        }

    }
}