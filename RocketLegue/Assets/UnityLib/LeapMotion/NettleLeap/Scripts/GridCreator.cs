using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//places all of its children in a grid pattern
public class GridCreator : MonoBehaviour
{
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetZ = 0;
    [SerializeField] private float spacingX = 0;
    [SerializeField] private float spacingZ = 0;

    //[SerializeField] private int rows = 5;
    [SerializeField] private int columns = 5;
    [SerializeField] private bool isSkipDisabledObjects = true;
    [Header("todo")]
    [SerializeField] private bool EnableSmoothGridCreation = true;
    private float smoothGridCreationTime = 2f;
    private bool isSmoothGridCreating = false;


    private void PlaceObjectsOnGrid()
    {
        Vector3 position = new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z + offsetZ);
        float currX = 0;
        float currZ = 0;
        int currRow = 0;
        int currCol = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            var go = transform.GetChild(i);
            if(isSkipDisabledObjects && !go.gameObject.activeSelf) continue;

            currX = spacingX * currCol + transform.localPosition.x;
            currZ = spacingZ * currRow + transform.localPosition.z;

            go.transform.localPosition = new Vector3(currX + offsetX, transform.localPosition.y, currZ + offsetZ);

            currCol++;
            if (!(currCol < columns))
            {
                currCol = 0;
                currRow++;
            }
        }

#if UNITY_EDITOR
        //EditorWindow view = EditorWindow.GetWindow<SceneView>(); todo repaint gizmos(for bounding box debug update)
        //view.//Repaint();
#endif
    }

    public void FormGrid()
    {
        /*if(EnableSmoothGridCreation){
            if(!isSmoothGridCreating){
                StopAllCoroutines();
                StartCoroutine(SmoothGridSpacing());
            }
        } else {
            PlaceObjectsOnGrid();
        }*/

        PlaceObjectsOnGrid();
    }

    float maxX = 0f;
    float maxZ = 0f;
    int maxDepth = 2;
    int currentDepth = 0;

    public void CalculateSpacingFromBounds()
    {

        Vector3 magnitude = Vector3.zero;
        /*
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Renderer>()==null){
                for (int j = 0; j < transform.GetChild(i).childCount; j++)
                {
                    if(transform.GetChild(i).GetChild(j).GetComponent<Renderer>() == null)
                        continue;
                }
            } else {
                magnitude = transform.GetChild(i).GetComponent<Renderer>().bounds.extents;
            }

            if(magnitude.x > maxX){
                maxX = magnitude.x;
            }
            if(magnitude.z > maxZ){
                maxZ = magnitude.z;
            }
        }*/
        currentDepth = 0;
        FindMaxBoundsExtentsFromChilds(transform);

        spacingX = maxX * 2;
        spacingZ = maxZ * 2;
        FormGrid();
    }

    private void FindMaxBoundsExtentsFromChilds(Transform root)
    {
        currentDepth++;
        if (currentDepth > maxDepth)
            return;
        for (int i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i).GetComponent<Renderer>();
            if (t != null)
            {
                var extents = t.bounds.extents;
                if (extents.x > maxX)
                {
                    maxX = extents.x;
                }
                if (extents.z > maxZ)
                {
                    maxZ = extents.z;
                }
            }
            else if (transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
            {
                var skinnedRenderer = transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                var extents = skinnedRenderer.bounds.extents;
                if (extents.x > maxX)
                {
                    maxX = extents.x;
                }
                if (extents.z > maxZ)
                {
                    maxZ = extents.z;
                }
            }
            FindMaxBoundsExtentsFromChilds(transform.GetChild(i));

        }
        currentDepth--;
    }

    public void OnValidate()
    {
        FormGrid();
    }

    void Update()
    {
        FormGrid();
    }

    private IEnumerator SmoothGridSpacing()
    {
        float timer = 0f;
        float initSpacingX = spacingX;
        float initSpacingZ = spacingZ;
        isSmoothGridCreating = true;

        while (timer < smoothGridCreationTime)
        {
            timer += Time.deltaTime;
            //spacingX = Mathf.SmoothStep(initSpacingX,targetX,timer/smoothGridCreationTime);
            //spacingZ = Mathf.SmoothStep(initSpacingZ,targetZ,timer/smoothGridCreationTime);
            PlaceObjectsOnGrid();
            yield return null;
        }

        isSmoothGridCreating = false;
    }
}
