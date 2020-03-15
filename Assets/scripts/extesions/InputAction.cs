/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using UnityEngine.InputSystem;


public static partial class Unity_extension {
    public static void register(
        this InputAction action, 
        System.Action<InputAction.CallbackContext> callback) 
    {
        action.Enable();
        action.performed += callback;
    }
    
    public static void unregister(
        this InputAction action, 
        System.Action<InputAction.CallbackContext> callback) 
    {
        action.Disable();
        action.performed -= callback;
    }

}*/