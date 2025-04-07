using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
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

    public void attach_appendages_to_body(Attachable_body attachable_body) {
        
        if (attacker_object) {
            attach_group_of_limbs_to_body(attacker_object, attachable_body);
        }
        if (transporter_object) {
            attach_group_of_limbs_to_body(transporter_object, attachable_body);
        }

        transform.localRotation = Vector2.right.to_quaternion();
        transform.localPosition = Vector3.zero;

        
        if (limb_f_l != null) {
            attach_appendage_to_body(
                limb_f_l.transform, transform, attachable_body.limb_f_l_attachment);
        }
        if (limb_m_l != null) {
            attach_appendage_to_body(
                limb_m_l.transform,  transform, attachable_body.limb_m_l_attachment);
        }
        if (limb_b_l != null) {
            attach_appendage_to_body(
                limb_b_l.transform,  transform, attachable_body.limb_b_l_attachment);
        }
        if (limb_f_r != null) {
            attach_appendage_to_body(
                limb_f_r.transform,  transform, attachable_body.limb_f_r_attachment);
        }
        if (limb_m_r != null) {
            attach_appendage_to_body(
                limb_m_r.transform,  transform, attachable_body.limb_m_r_attachment);
        }
        if (limb_b_r != null) {
            attach_appendage_to_body(
                limb_b_r.transform,  transform, attachable_body.limb_b_r_attachment);
        }
        Object.Destroy(this);
    }
    
    public void attach_appendage_to_body(Transform appendage, Transform body, Span_component attachment_point) {
        appendage.localPosition = attachment_point.transform.localPosition;
        appendage.localRotation = attachment_point.transform.localRotation;
        
        if (appendage.GetComponent<ILimb>() is {} limb) {
            attach_limb_to_body(limb, appendage, attachment_point);
        }
        
    }
    
    public void attach_limb_to_body(ILimb limb, Transform limb_transform, Span_component attachment_point) {
        Turning_element root_segment = limb.get_root_segment();
        
        root_segment.transform.localRotation = attachment_point.allowed_direction.to_quaternion();
        root_segment.possible_span.min = attachment_point.span.min;
        root_segment.possible_span.max = attachment_point.span.max;
        root_segment.possible_span.goes_through_switching_degrees = attachment_point.span.goes_through_switching_degrees;
    }

    

    private void attach_group_of_limbs_to_body(GameObject group, Attachable_body attachable_body) {
        group.transform.parent = attachable_body.transform;
        group.transform.localPosition = new Vector3(0,0,0.1f);
        group.transform.localRotation = Quaternion.identity;
    }
    
}


}