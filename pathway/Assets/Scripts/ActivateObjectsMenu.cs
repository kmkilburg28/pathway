using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectsMenu : MonoBehaviour {

    void Start()
    {
        transform.parent.transform.GetChild(transform.parent.transform.childCount - 1).gameObject.SetActive(false);
    }

    public void OpenObjectsMenu()
    {
        transform.parent.transform.GetChild(transform.parent.transform.childCount - 1).gameObject.SetActive(true);
    }
}
