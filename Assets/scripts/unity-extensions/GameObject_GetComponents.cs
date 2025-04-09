using System;
using System.Collections.Generic;
using UnityEngine;

namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{

    public static T GetComponentInDirectChildren<T>(this Component parent) where T : Component
    {
        return parent.GetComponentInDirectChildren<T>(false);
    }

    public static T GetComponentInDirectChildren<T>(this Component parent, bool includeInactive) where T : Component
    {
        foreach (Transform transform in parent.transform)
        {
            if (includeInactive || transform.gameObject.activeInHierarchy)
            {
                T component = transform.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }
        return null;
    }

    public static List<T> GetComponentsInDirectChildren<T>(this Component parent) where T : Component
    {
        return parent.GetComponentsInDirectChildren<T>(false);
    }

    public static List<T> GetComponentsInDirectChildren<T>(this Component parent, bool includeInactive) where T : Component
    {
        List<T> tmpList = new List<T>();
        foreach (Transform transform in parent.transform)
        {
            if (includeInactive || transform.gameObject.activeInHierarchy)
            {
                tmpList.AddRange(transform.GetComponents<T>());
            }
        }
        return tmpList;
    }

    public static T GetComponentInSiblings<T>(this Component sibling) where T : Component
    {
        return sibling.GetComponentInSiblings<T>(false);
    }

    public static T GetComponentInSiblings<T>(this Component sibling, bool includeInactive) where T : Component
    {
        Transform parent = sibling.transform.parent;
        if (parent == null) return null;
        foreach (Transform transform in parent)
        {
            if (includeInactive || transform.gameObject.activeInHierarchy)
            {
                if (transform != sibling)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }
        return null;
    }

    public static List<T> GetComponentsInSiblings<T>(this Component sibling) where T : Component
    {
        return sibling.GetComponentsInSiblings<T>(false);
    }

    public static List<T> GetComponentsInSiblings<T>(this Component sibling, bool includeInactive) where T : Component
    {
        Transform parent = sibling.transform.parent;
        if (parent == null) return null;
        List<T> tmpList = new List<T>();
        foreach (Transform transform in parent)
        {
            if (includeInactive || transform.gameObject.activeInHierarchy)
            {
                if (transform != sibling)
                {
                    tmpList.AddRange(transform.GetComponents<T>());
                }
            }
        }
        return tmpList;
    }

    public static T GetComponentInDirectParent<T>(this Component child) where T : Component
    {
        Transform parent = child.transform.parent;
        if (parent == null) return null;
        return parent.GetComponent<T>();
    }

    public static T[] GetComponentsInDirectParent<T>(this Component child) where T : Component
    {
        Transform parent = child.transform.parent;
        if (parent == null) return null;
        return parent.GetComponents<T>();
    }

    public static Tuple<T1,T2>[] get_components_with_interfaces<T1, T2>(this Component component) {
        var interfaces1 = component.GetComponentsInChildren<T1>();
        var needed_components = new List<Tuple<T1,T2>>();
        foreach (var interface1 in interfaces1) {
            if (interface1 is T2 interface2) {
                needed_components.Add(new Tuple<T1,T2>(interface1,interface2));
            }
        }
        return needed_components.ToArray();
    }

    public static List<TNeeded_component> get_components_in_children_stop_at_component
        <TNeeded_component, TStoppong_component>
        (this Component root) 
    {
        var found_components = new List<TNeeded_component>();
        scan_components_recursive(root);
        return found_components;
        
        void scan_components_recursive(Component root) {
            found_components.AddRange(root.GetComponents<TNeeded_component>());
            foreach (Transform child in root.transform) {
                if (child.GetComponent<TStoppong_component>() is null ) {
                    scan_components_recursive(child);
                }
            }
        }
    }
}
}