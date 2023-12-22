using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//todo this box is not axis alligned, will not work in all possible orientations of colliders
public class CustomBoxCollider : CustomCollider
{
    public Vector3 Dimensions;
    public bool UseObjectsAsMinMax = true;
    public Transform DirectMax;
    public Transform DirectMin;

    public Vector3 Max
    {
        get
        {
            if (UseObjectsAsMinMax)
            {
                if (DirectMax == null)
                {
                    return new Vector3(0, 0, 0);
                }
                return DirectMax.position;
            }
            //Vector3 t = transform.position;
            Vector3 dimensions = new Vector3(Dimensions.x / 2, Dimensions.y / 2, Dimensions.z / 2);
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale).MultiplyPoint3x4(dimensions);//dimensions;// * transform.rotation;
        }
    }

    public Vector3 Min
    {
        get
        {
            if (UseObjectsAsMinMax)
            {
                if (DirectMin == null)
                {
                    return new Vector3(0, 0, 0);
                }
                return DirectMin.position;
            }
            //Vector3 t = transform.position;
            Vector3 dimensions = new Vector3(-Dimensions.x / 2, -Dimensions.y / 2, -Dimensions.z / 2);
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale).MultiplyPoint3x4(dimensions);
        }
    }
    public Vector3 InverseTransformPointUnscaled(Transform transform, Vector3 position)
    {
        var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
        return worldToLocalMatrix.MultiplyPoint3x4(position);
    }



    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        if (UseObjectsAsMinMax)
        {
            ;
            //return;
        }

        //Gizmos.matrix = Matrix4x4.TRS(transform.position,transform.rotation,transform.lossyScale);        

        //Gizmos.DrawCube(Vector3.zero, Dimensions);
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawLine(Max, Min); //todo fix
        /*Gizmos.DrawLine(Max,Min);
        Gizmos.DrawLine(Max,Min);
        Gizmos.DrawLine(Max,Min);
        Gizmos.DrawLine(Max,Min);
        Gizmos.DrawLine(Max,Min);
        Gizmos.DrawLine(Max,Min);
        Gizmos.DrawLine(Max,Min);*/

#if UNITY_EDITOR

        var tr = transform;
        var pos = tr.position;
        var color = new Color(1, 0.8f, 0.4f, 1);
        //Handles.color = color; todo create box with handles
        //Handles.DrawWireDisc(pos, tr.up, 1.0f);
        // display object "value" in scene
        GUI.color = color;
        //Handles.Label(pos, .value.ToString("F1"));
#endif
    }


    /*
        public var tex : Texture;

     private var bgRect : Rect = Rect(300,300,500,500);
     private var handleRect : Rect;
     private var minX : float;
     private var minY : float;
     private var maxX : float;
     private var maxY : float;

     private var offset : Vector2;
     private var dragging = false;

    void Start() {
         handleRect = Rect(0,0,tex.width, tex.height);
         handleRect.center = bgRect.center;
         minX = bgRect.x + tex.width / 2.0;
         minY = bgRect.y + tex.height / 2.0;
         maxX = bgRect.x + bgRect.width - tex.width / 2.0;
         maxY = bgRect.y + bgRect.height - tex.height / 2.0;
     }

     void OnGUI() {
         Event e= Event.current;

         if (e.type == EventType.MouseDown && handleRect.Contains(e.mousePosition)) {
             offset = handleRect.center - e.mousePosition;
             dragging = true;
         }

         if (e.type == EventType.MouseDrag && dragging) {
             handleRect.center = e.mousePosition + offset;
             handleRect.center.x = Mathf.Clamp(handleRect.center.x, minX, maxX);
             handleRect.center.y = Mathf.Clamp(handleRect.center.y, minY, maxY);
             offset = handleRect.center - e.mousePosition;
         }

         if (e.type == EventType.MouseUp) {
             dragging = false;
         }

         GUI.Box(bgRect, "");
         GUI.DrawTexture(handleRect, tex);
     }*/
}
