using System.Collections;
using System.Collections.Generic;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public class Attachable_body : MonoBehaviour {
    public Span_component limb_f_l_attachment;
    public Span_component limb_m_l_attachment;
    public Span_component limb_b_l_attachment;
   
    public Span_component limb_f_r_attachment;
    public Span_component limb_m_r_attachment;
    public Span_component limb_b_r_attachment;
    
    public Transform head_attachment;

    public Intelligence intelligence;

    public List<Transform> disposed_after_attachment;




}

}