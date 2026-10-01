using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace rvinowise.unity {

public class Play_loop :
    MonoBehaviour,
    IInput_receiver
{
    public const string main_menu_scene = "main_menu";

    const int overlay_sort_order = 400;

    Player_human player;
    Damage_receiver player_body;
    readonly List<Scene_transition> exits = new List<Scene_transition>();
    int enemies_at_start;
    int last_enemy_count = -1;
    bool session_started;
    bool player_dead;
    bool area_cleared;
    bool paused;
    bool overlay_owns_input;
    bool holding_time_scale;
    float time_scale_before_pause = 1f;

    Canvas canvas;
    Text hud_text;
    GameObject overlay_root;
    Text overlay_title;
    Text overlay_body;
    Button primary_button;
    Button secondary_button;
    Button menu_button;
    Button quit_button;
    Text primary_label;
    Text secondary_label;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void spawn_in_play_scenes() {
        if (Object.FindObjectOfType<Play_loop>() != null) {
            return;
        }
        if (Object.FindObjectOfType<Player_input>() == null) {
            return;
        }
        new GameObject("Play_loop").AddComponent<Play_loop>();
    }

    void OnDestroy() {
        unbind_player();
        restore_time();
    }

    void Start() {
        build_ui();
        StartCoroutine(begin_session_next_frame());
    }

    IEnumerator begin_session_next_frame() {
        yield return null;
        yield return null;
        bind_current_scene();
    }

    void bind_current_scene() {
        bind_player();
        find_exits();
        enemies_at_start = count_living_enemies();
        last_enemy_count = enemies_at_start;
        session_started = player != null;

        if (enemies_at_start > 0) {
            set_exits_active(false);
        }
        else {
            area_cleared = true;
            set_exits_active(true);
        }

        refresh_hud();
        hide_overlay();
    }

    void bind_player() {
        unbind_player();
        player = Player_input.find_player();
        if (player == null) {
            return;
        }
        var humanoid = player.GetComponent<Humanoid>();
        player_body = humanoid != null ? humanoid.damageable_body : null;
        if (player_body != null) {
            player_body.on_destroyed += on_player_destroyed;
        }
    }

    void unbind_player() {
        if (player_body != null) {
            player_body.on_destroyed -= on_player_destroyed;
        }
        player = null;
        player_body = null;
    }

    void find_exits() {
        exits.Clear();
        exits.AddRange(Object.FindObjectsOfType<Scene_transition>(true));
    }

    void Update() {
        if (!session_started || overlay_root == null) {
            return;
        }

        if (!player_dead && UnityEngine.Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) {
                resume_game();
            }
            else {
                pause_game();
            }
            return;
        }

        if (player_dead || paused) {
            return;
        }

        int living_enemies = count_living_enemies();
        if (living_enemies != last_enemy_count) {
            last_enemy_count = living_enemies;
            if (enemies_at_start > 0 && living_enemies <= 0 && !area_cleared) {
                on_area_cleared();
            }
            refresh_hud();
        }
    }

    int count_living_enemies() {
        var units = Object.FindObjectsOfType<Intelligence>();
        int living = 0;
        foreach (var unit in units) {
            if (unit == null || unit == player) {
                continue;
            }
            if (player != null && player.team != null) {
                if (player.team.is_enemy(unit.gameObject)) {
                    living++;
                }
            }
            else if (unit is Computer_intelligence) {
                living++;
            }
        }
        return living;
    }

    void on_player_destroyed(Damage_receiver _) {
        if (player_dead) {
            return;
        }
        player_dead = true;
        paused = false;
        freeze_time();
        capture_input();
        show_overlay(
            "You died",
            "The line broke. Try the same map again, or leave to the main menu.",
            "Restart",
            restart_scene,
            Application.CanStreamedLevelBeLoaded(main_menu_scene) ? "Main menu" : null,
            go_to_main_menu
        );
        refresh_hud();
    }

    void on_area_cleared() {
        area_cleared = true;
        set_exits_active(true);
        if (exits.Count == 0) {
            freeze_time();
            capture_input();
            show_overlay(
                "Area clear",
                "No hostiles remain.",
                "Restart",
                restart_scene,
                Application.CanStreamedLevelBeLoaded(main_menu_scene) ? "Main menu" : null,
                go_to_main_menu
            );
        }
        else {
            hide_overlay();
            release_input();
        }
        refresh_hud();
    }

    void set_exits_active(bool active) {
        foreach (var exit in exits) {
            if (exit != null) {
                exit.gameObject.SetActive(active);
            }
        }
    }

    void pause_game() {
        if (player_dead) {
            return;
        }
        paused = true;
        freeze_time();
        capture_input();
        show_overlay(
            "Paused",
            "Esc resumes. Restart reloads this map.",
            "Resume",
            resume_game,
            "Restart",
            restart_scene
        );
    }

    void resume_game() {
        if (player_dead) {
            return;
        }
        paused = false;
        restore_time();
        hide_overlay();
        release_input();
    }

    void restart_scene() {
        restore_time();
        hide_overlay();
        release_input();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void go_to_main_menu() {
        restore_time();
        hide_overlay();
        release_input();
        if (Application.CanStreamedLevelBeLoaded(main_menu_scene)) {
            SceneManager.LoadScene(main_menu_scene);
        }
        else {
            restart_scene();
        }
    }

    void quit_game() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void freeze_time() {
        if (holding_time_scale) {
            return;
        }
        if (Time.timeScale > 0f) {
            time_scale_before_pause = Time.timeScale;
        }
        Time.timeScale = 0f;
        holding_time_scale = true;
    }

    void restore_time() {
        if (!holding_time_scale) {
            return;
        }
        Time.timeScale = time_scale_before_pause > 0f ? time_scale_before_pause : 1f;
        holding_time_scale = false;
    }

    void refresh_hud() {
        if (hud_text == null) {
            return;
        }
        if (!session_started) {
            hud_text.text = "";
            return;
        }
        if (player_dead) {
            hud_text.text = "Enter: restart   Esc: menu";
            return;
        }
        if (enemies_at_start > 0 && !area_cleared) {
            hud_text.text = $"Hostiles left: {Mathf.Max(0, last_enemy_count)}   Esc: pause";
            return;
        }
        if (exits.Count > 0) {
            hud_text.text = "Exit unlocked — walk into the gate and press Enter   Esc: pause";
            return;
        }
        hud_text.text = "Esc: pause";
    }

    public bool is_finished { get; set; }

    public bool process_input() {
        if (!overlay_owns_input) {
            is_finished = true;
            return false;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space)) {
            if (paused && !player_dead) {
                resume_game();
            }
            else if (player_dead || (area_cleared && exits.Count == 0)) {
                restart_scene();
            }
        }
        return true;
    }

    void capture_input() {
        if (Player_input.instance == null) {
            return;
        }
        overlay_owns_input = true;
        is_finished = false;
        if (!overlay_already_receiving_input()) {
            Player_input.instance.add_input_receiver(this);
        }
    }

    bool overlay_already_receiving_input() {
        return overlay_owns_input && !is_finished;
    }

    void release_input() {
        overlay_owns_input = false;
        is_finished = true;
    }

    void hide_overlay() {
        if (overlay_root != null) {
            overlay_root.SetActive(false);
        }
    }

    void show_overlay(
        string title,
        string body,
        string primary,
        UnityEngine.Events.UnityAction primary_action,
        string secondary,
        UnityEngine.Events.UnityAction secondary_action
    ) {
        overlay_root.SetActive(true);
        overlay_title.text = title;
        overlay_body.text = body;
        primary_label.text = primary;
        primary_button.onClick.RemoveAllListeners();
        primary_button.onClick.AddListener(primary_action);

        bool has_secondary = !string.IsNullOrEmpty(secondary) && secondary_action != null;
        secondary_button.gameObject.SetActive(has_secondary);
        if (has_secondary) {
            secondary_label.text = secondary;
            secondary_button.onClick.RemoveAllListeners();
            secondary_button.onClick.AddListener(secondary_action);
        }

        bool has_menu = Application.CanStreamedLevelBeLoaded(main_menu_scene);
        menu_button.gameObject.SetActive(has_menu);
        menu_button.onClick.RemoveAllListeners();
        menu_button.onClick.AddListener(go_to_main_menu);
        quit_button.onClick.RemoveAllListeners();
        quit_button.onClick.AddListener(quit_game);
    }

    void build_ui() {
        var font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        canvas = new GameObject("Play_loop_canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = overlay_sort_order;
        canvas.transform.SetParent(transform, false);
        var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        hud_text = create_text(canvas.transform, "hud", 28, TextAnchor.UpperLeft, FontStyle.Bold);
        var hud_rect = hud_text.rectTransform;
        hud_rect.anchorMin = new Vector2(0f, 1f);
        hud_rect.anchorMax = new Vector2(1f, 1f);
        hud_rect.pivot = new Vector2(0f, 1f);
        hud_rect.anchoredPosition = new Vector2(36f, -28f);
        hud_rect.sizeDelta = new Vector2(-72f, 80f);
        hud_text.font = font;

        overlay_root = new GameObject("overlay");
        overlay_root.transform.SetParent(canvas.transform, false);
        var overlay_rect = overlay_root.AddComponent<RectTransform>();
        stretch_full(overlay_rect);
        var dim = overlay_root.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.62f);

        var panel = new GameObject("panel");
        panel.transform.SetParent(overlay_root.transform, false);
        var panel_rect = panel.AddComponent<RectTransform>();
        panel_rect.anchorMin = new Vector2(0.5f, 0.5f);
        panel_rect.anchorMax = new Vector2(0.5f, 0.5f);
        panel_rect.sizeDelta = new Vector2(720f, 520f);
        var panel_image = panel.AddComponent<Image>();
        panel_image.color = new Color(0.07f, 0.08f, 0.1f, 0.94f);

        overlay_title = create_text(panel.transform, "title", 48, TextAnchor.MiddleCenter, FontStyle.Bold);
        overlay_title.font = font;
        place(overlay_title.rectTransform, 0f, 180f, 640f, 70f);

        overlay_body = create_text(panel.transform, "body", 26, TextAnchor.MiddleCenter, FontStyle.Normal);
        overlay_body.font = font;
        overlay_body.color = new Color(0.85f, 0.85f, 0.85f);
        place(overlay_body.rectTransform, 0f, 90f, 640f, 90f);

        primary_button = create_button(panel.transform, font, out primary_label);
        place(primary_button.GetComponent<RectTransform>(), 0f, 0f, 360f, 56f);

        secondary_button = create_button(panel.transform, font, out secondary_label);
        place(secondary_button.GetComponent<RectTransform>(), 0f, -70f, 360f, 56f);

        menu_button = create_button(panel.transform, font, out var menu_label);
        menu_label.text = "Main menu";
        place(menu_button.GetComponent<RectTransform>(), 0f, -140f, 360f, 56f);

        quit_button = create_button(panel.transform, font, out var quit_label);
        quit_label.text = "Quit";
        place(quit_button.GetComponent<RectTransform>(), 0f, -210f, 360f, 56f);

        overlay_root.SetActive(false);
    }

    static Button create_button(Transform parent, Font font, out Text label) {
        var go = new GameObject("button");
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.color = new Color(0.18f, 0.22f, 0.28f, 1f);
        var button = go.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(0.28f, 0.36f, 0.45f, 1f);
        colors.pressedColor = new Color(0.12f, 0.16f, 0.2f, 1f);
        button.colors = colors;
        label = create_text(go.transform, "label", 24, TextAnchor.MiddleCenter, FontStyle.Bold);
        label.font = font;
        stretch_full(label.rectTransform);
        return button;
    }

    static Text create_text(
        Transform parent,
        string name,
        int size,
        TextAnchor alignment,
        FontStyle style
    ) {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.fontSize = size;
        text.alignment = alignment;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        go.AddComponent<RectTransform>();
        return text;
    }

    static void stretch_full(RectTransform rect) {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    static void place(RectTransform rect, float x, float y, float width, float height) {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
    }
}

}
