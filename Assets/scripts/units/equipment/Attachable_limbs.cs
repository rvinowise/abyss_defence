using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using UnityEngine;


namespace rvinowise.unity {

public class Attachable_limbs : MonoBehaviour 

{
    public GameObject limb_f_l;
    public GameObject limb_m_l;
    public GameObject limb_b_l;
    
    public GameObject limb_f_r;
    public GameObject limb_m_r;
    public GameObject limb_b_r;
    
    public GameObject attacker_object;
    public GameObject transporter_object;

    public void attach_limbs_to_body(Attachable_body attachable_body) {
        //attachable_body.intelligence.transporter = transporter_object.GetComponent<IActor_transporter>();
        //attachable_body.intelligence.transporter.set_moved_body(attachable_body.GetComponent<Turning_element>());
        //attachable_body.intelligence.attacker = attacker_object.GetComponent<IAttacker>();
        transform.parent = attachable_body.transform;
        transform.localRotation = Vector2.right.to_quaternion();
        transform.localPosition = Vector3.zero;

        var ilimb_f_l = limb_f_l.GetComponent<ILimb>();
        var ilimb_m_l = limb_m_l.GetComponent<ILimb>();
        var ilimb_b_l = limb_b_l.GetComponent<ILimb>();
        var ilimb_f_r = limb_f_r.GetComponent<ILimb>();
        var ilimb_m_r = limb_m_r.GetComponent<ILimb>();
        var ilimb_b_r = limb_b_r.GetComponent<ILimb>();
        
        if (ilimb_f_l != null) {
            attach_limb_to_body(
                limb_f_l.transform, ilimb_f_l.get_root_segment(), transform, attachable_body.limb_f_l_attachment);
        }
        if (ilimb_m_l != null) {
            attach_limb_to_body(
                limb_m_l.transform, ilimb_m_l.get_root_segment(), transform, attachable_body.limb_m_l_attachment);
        }
        if (ilimb_b_l != null) {
            attach_limb_to_body(
                limb_b_l.transform, ilimb_b_l.get_root_segment(), transform, attachable_body.limb_b_l_attachment);
        }
        if (ilimb_f_r != null) {
            attach_limb_to_body(
                limb_f_r.transform, ilimb_f_r.get_root_segment(), transform, attachable_body.limb_f_r_attachment);
        }
        if (ilimb_m_r != null) {
            attach_limb_to_body(
                limb_m_r.transform, ilimb_m_r.get_root_segment(), transform, attachable_body.limb_m_r_attachment);
        }
        if (ilimb_b_r != null) {
            attach_limb_to_body(
                limb_b_r.transform, ilimb_b_r.get_root_segment(), transform, attachable_body.limb_b_r_attachment);
        }
        Object.Destroy(this);
    }
    
    public void attach_limb_to_body(Transform limb, Turning_element root_segment, Transform body, Span_component attachment_point) {
        limb.parent = body;
        limb.localPosition = attachment_point.transform.localPosition;
        root_segment.transform.localRotation = attachment_point.transform.localRotation;
        root_segment.possible_span.min = attachment_point.span.min;
        root_segment.possible_span.max = attachment_point.span.max;
        root_segment.possible_span.goes_through_switching_degrees = attachment_point.span.goes_through_switching_degrees;
    }
}


}