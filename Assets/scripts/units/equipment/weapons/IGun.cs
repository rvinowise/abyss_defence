namespace rvinowise.unity {

public interface IGun {

    void pull_trigger();
    void release_trigger();

    float time_to_readiness();

}
}