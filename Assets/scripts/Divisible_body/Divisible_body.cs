using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using geometry2d;
using units;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public partial class Divisible_body : MonoBehaviour
{
    public Sprite inside;
    
    
    public void split_by_ray(Ray2D ray_of_split) {

        List<Polygon> collider_pieces = Polygon_splitter.split_polygon_by_ray(
            new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0)),
            transform.InverseTransformRay(ray_of_split)
        );
        
        Sprite body = gameObject.GetComponent<SpriteRenderer>().sprite;
        
        IEnumerable<GameObject> piece_objects = create_objects_for_colliders(
            collider_pieces, body, inside
        );

        split_controllers_of_tools(piece_objects);
        Destroy(gameObject);
    }

    private IEnumerable<GameObject> create_objects_for_colliders(
        IEnumerable<Polygon> collider_pieces,
        Sprite body,
        Sprite inside
    ) 
    {
        List<GameObject> piece_objects = new List<GameObject>();    
        foreach(Polygon collider_piece in collider_pieces) {
            Texture2D texture_piece = 
                Texture_splitter.create_texture_with_insides_for_polygon(
                    body,
                    inside,
                    collider_piece
                );

            piece_objects.Add(
                create_gameobject_from_polygon_and_texture(
                    collider_piece, texture_piece
                )
            );
        }
        return piece_objects;
    }

    private GameObject create_gameobject_from_polygon_and_texture(
        Polygon polygon, Texture2D texture) 
    {
        GameObject created_part = Instantiate(
            gameObject, transform.position, transform.rotation);
        
        body_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        Sprite sprite = Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            body_sprite.to_texture_coordinates(body_sprite.pivot), 
            body_sprite.pixelsPerUnit
            );
        
        created_part.GetComponent<PolygonCollider2D>().SetPath(0, polygon.points);
        created_part.GetComponent<SpriteRenderer>().sprite = sprite;
        return created_part;
    }

    private void split_controllers_of_tools(IEnumerable<GameObject> piece_objects) {
        User_of_tools user_of_tools = GetComponent<User_of_tools>();
        if (!user_of_tools) {
            return;
        }
        foreach (Tool_controller tool_controller in user_of_tools.tool_controllers) { 
            foreach(units.Tool tool in tool_controller.tools) {
                foreach(GameObject piece_object in piece_objects) { 
                    // each collider must have only one path (simple polygon)
                    PolygonCollider2D collider = piece_object.GetComponent<PolygonCollider2D>();
                    if (
                        System.Convert.ToBoolean(
                            ClipperLib.Clipper.PointInPolygon(
                                Clipperlib_coordinates.float_coord_to_int(tool.attachment), 
                                Clipperlib_coordinates.float_coord_to_int(new Polygon(collider.GetPath(0)))
                            )
                        )
                    ) {
                        //User_of_tools piece_user_of_tools = piece_object.GetComponent<User_of_tools>();
                        //piece_user_of_tools.get_tool_controller<tool_controller>();
                    }
                }
            }
        }
        
    }

    /*private IEnumerable<ITool_controller> get_tool_controllers() {
        IEnumerable<IUser_of_tools> users_of_tools = 
            GetComponents<IUser_of_tools>();
        foreach(IUser_of_tools user_of_tools in users_of_tools) { // Creature (usually only one)
            IEnumerable<ITool_controller> tool_controllers =
                user_of_tools.tool_controllers;
            foreach(ITool_controller tool_controller in tool_controllers) { // Leg_controller (maybe also Weapon_controller)
                IEnumerable<units.Tool> tools = tool_controller.tools;
                foreach(units.Tool tool in tools) {
                    Debug.Log("tool: "+tool);
                }
            }
        }

         //IEnumerable<ITool_controller> tool_controllers = GetComponents<ITool_controller>();

         return tool_controllers;
    }*/
    



    void Start()
    {
    }

    void Update()
    {

    }

     
}
