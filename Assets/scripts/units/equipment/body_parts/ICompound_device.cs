using System.Collections.Generic;


namespace rvinowise.unity {

public interface IIntelligence_device {
    
}

public interface ICompound_device {

    void add_child_devices(IEnumerable<IIntelligence_device> child_devices);
}

}