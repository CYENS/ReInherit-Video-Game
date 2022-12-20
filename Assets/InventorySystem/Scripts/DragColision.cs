using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TreeEditor;
using UnityEngine;

/***
 * This class is used for changing the opacity of the gameobject when it collides.
 * This script is attached to the dragging gameobjects
 ***/
public class DragColision : MonoBehaviour
{
    private Material ObjectMaterial;
    private DragColision dragColisionScript;
    private Color tempColor;


    private void Awake()
    {
        // Get the material from the MeshRenderer component of the object
        ObjectMaterial = GetComponent<MeshRenderer>().material;      
        // Get the "DragColision" script
        dragColisionScript = GetComponent<DragColision>();

    }

    private void OnTriggerEnter(Collider collision)
    {
        // A collision has occurred
        if(collision.name != "Floor" && dragColisionScript.enabled == true)
        {
            changeOpcacity(.25f);
        }
            
    }

    //void OnTriggerStay(Collider collision)
    //{
    //    // Still colliding
    //}

    private void OnTriggerExit(Collider collision)
    {
        // Collision has ended
        Debug.Log("colision exit");

        changeOpcacity(1f);
        
    }

    // This function changes the opacity of the dragging gameobject
    private void changeOpcacity(float opacityNumber)
    {
        tempColor = ObjectMaterial.color;
        tempColor.a = opacityNumber;
        ObjectMaterial.color = tempColor;
    }
}
