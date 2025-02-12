using UnityEngine;


namespace rvinowise.unity.music {
    public class Music_player: MonoBehaviour {


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
        private bool running = false;
        private int i_audio = 0;


        void Start()
        {
            next_phrase_time = AudioSettings.dspTime + 2.0f;
            init_audio_sources();
        }

        private void init_audio_sources() {
            for (int i = 0; i < 2; i++)
            {
                GameObject child = new GameObject("Audio_source");
                child.transform.parent = gameObject.transform;
                audio_sources[i] = child.AddComponent<AudioSource>();
            }
        }

        private float time_to_prepare_phrase = 1f;
        void Update() {
            
            if (is_time_to_prepare_next_phrase())
            {
                Debug.Log("is_time_to_prepare_next_phrase");
                schedule_next_phrase();

                next_phrase_time += 60.0f / bpm * beats_per_phrase;
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
                audio_sources[i_audio].clip = current_track.start_from_first_phrase().clip;
            } else {
                audio_sources[i_audio].clip = current_track.goto_next_phrase().clip;
            }
            audio_sources[i_audio].PlayScheduled(next_phrase_time);
            i_audio = 1 - i_audio;
            Debug.Log("i_audio=" + i_audio + " next_phrase_time=" + next_phrase_time + "i_phrase="+current_track.i_current_phrase);
            
        }

        public void set_next_track(Music_track track) {
            next_track = track;
            
        }
    }
}