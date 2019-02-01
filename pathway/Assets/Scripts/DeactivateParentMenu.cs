using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateParentMenu : MonoBehaviour {

	public void CloseParentMenu()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
