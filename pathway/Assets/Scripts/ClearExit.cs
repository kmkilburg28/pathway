using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearExit : MonoBehaviour {

    MeshRenderer meshRenderer;
    List<Collider> insideList;
    Vector3 lastPosition;

    // Use this for initialization
    void Start ()
    {
        insideList = new List<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        DisableInsideFloorBlocks();
        if (!GameController.gameController.developerMode)
        {
            this.enabled = false;
        }
        else
        {
            lastPosition = transform.position;
        }
    }

    private void Update()
    {
        if (transform.childCount > 0 && !lastPosition.Equals(transform.position))
        {
            // Enable and disable colliding floor blocks
            EnableOutsideFloorBlocks();
            DisableInsideFloorBlocks();
        }
    }

    void EnableOutsideFloorBlocks()
    {
        int i = 0;
        while (i < insideList.Count)
        {
            if (!meshRenderer.bounds.Contains(insideList[i].transform.position))
            {
                insideList[i].gameObject.SetActive(true);
                insideList.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    void DisableInsideFloorBlocks()
    {
        Collider[] currentInside = Physics.OverlapBox(
            transform.position,
            meshRenderer.bounds.size / 2.1f
        );

        for (int i = 0; i < currentInside.Length; i++)
        {
            if (currentInside[i].tag == "Floor" && NewColliderNotInside(currentInside[i]))
            {
                insideList.Add(currentInside[i]);
                currentInside[i].gameObject.SetActive(false);
            }
        }
    }

    private bool NewColliderNotInside(Collider newCollider)
    {
        for (int i = 0; i < insideList.Count; i++)
        {
            if (newCollider.gameObject == insideList[i].gameObject)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < insideList.Count; i++)
        {
            insideList[i].gameObject.SetActive(true);
        }
    }
}
