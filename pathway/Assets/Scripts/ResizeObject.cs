using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeObject : MonoBehaviour {

    float emphasis = 0.0001f;

    Vector3 copyOfPosition;
    Quaternion copyOfQuaternion;
    Vector3 copyOfScale;


    // For moving
    Vector3 adjustmentDistance;
    bool adjustmentDisDetermined;

    public static bool alreadyAdjusting;

    private void OnMouseEnter()
    {
        if (!alreadyAdjusting)
        {
            Color hover = new Color(0f, 0f, 0f, 0.2f);
            GetComponent<MeshRenderer>().material.color = hover;
        }
    }
    private void OnMouseExit()
    {
        if (!alreadyAdjusting)
        {
            GetComponent<MeshRenderer>().material.color = Color.clear;
        }
    }

    private void OnMouseDown()
    {
        copyOfPosition = new Vector3(
            transform.parent.position.x,
            transform.parent.position.y,
            transform.parent.position.z 
        );
        copyOfQuaternion = new Quaternion(
            transform.parent.rotation.x,
            transform.parent.rotation.y,
            transform.parent.rotation.z,
            transform.parent.rotation.w
        );
        copyOfScale = new Vector3(
            transform.parent.localScale.x,
            transform.parent.localScale.y,
            transform.parent.localScale.z
        );

        transform.parent.GetComponent<Collider>().enabled = false;
        GameController.gameController.buildMode = true;
        alreadyAdjusting = true;
        adjustmentDisDetermined = false;
    }

    private void OnMouseUp()
    {
        if (!ToolPlacement.isPlacing)
        {
            if (!GameController.gameController.IsObjectPlaceable(transform.parent.gameObject))
            {
                ReturnToOriginal();
            }
            transform.parent.GetComponent<Collider>().enabled = true;
            GameController.gameController.buildMode = false;
            alreadyAdjusting = false;
            GameController.gameController.GetComponent<OutlineBox>().SignalPlaceability();
        }
    }

    // Grab top
    private void StartMove()
    {
        MeshRenderer parentMR = transform.parent.gameObject.GetComponent<MeshRenderer>();
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = Camera.main.transform.position.y;
        Vector3 followPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        float[] adjustments = GetXAndZAdjustment(followPosition, parentMR);

        adjustmentDistance = new Vector3(
            parentMR.transform.position.x - (Mathf.Round(followPosition.x) + adjustments[0]),
            transform.parent.position.y,
            parentMR.transform.position.z - (Mathf.Round(followPosition.z) + adjustments[1])
        );
        adjustmentDisDetermined = true;
    }
    private void ContinueMove()
    {
        MeshRenderer parentMR = transform.parent.gameObject.GetComponent<MeshRenderer>();
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = Camera.main.transform.position.y;

        Vector3 followPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        float[] adjustments = GetXAndZAdjustment(followPosition, parentMR);

        // Fit tool to grid
        transform.parent.position = new Vector3(
            Mathf.Round(followPosition.x) + adjustmentDistance.x + adjustments[0],
            transform.parent.position.y,
            Mathf.Round(followPosition.z) + adjustmentDistance.z + adjustments[1]
        );
    }
    private float[] GetXAndZAdjustment(Vector3 followPosition, MeshRenderer meshRenderer)
    {
        float xAdjustment = 0;
        float zAdjustment = 0;

        // Tool center tracks mouse
        // Create x adjustment
        if (Mathf.RoundToInt(meshRenderer.bounds.size.x) % 2 != 0)
        {
            int multiplyFactor = (Mathf.RoundToInt(followPosition.x - Mathf.FloorToInt(followPosition.x)) == 0 ? 1 : -1);
            xAdjustment += (0.5f * multiplyFactor);
        }

        // Create z adjustment
        if (Mathf.RoundToInt(meshRenderer.bounds.size.z) % 2 != 0)
        {
            int multiplyFactor = (Mathf.RoundToInt(followPosition.z - Mathf.FloorToInt(followPosition.z)) == 0 ? 1 : -1);
            zAdjustment += (0.5f * multiplyFactor);
        }
        return new float[] { xAdjustment, zAdjustment };
    }

    private void OnMouseDrag()
    {
        if (!ToolPlacement.isPlacing)
        {

            GameController.gameController.GetComponent<OutlineBox>().SignalPlaceability();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material.color = new Color(
                (meshRenderer.material.color.r > 0.1f ? 0.6f : 0f),
                0,
                0,
                0.4f
            );

            MeshRenderer parentMR = transform.parent.GetComponent<MeshRenderer>();
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.transform.position.y;
            Vector3 followPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            //Debug.Log(followPosition);

            if (gameObject.name.Substring(4).Equals("Top"))
            {
                if (adjustmentDisDetermined == false)
                {
                    StartMove();
                }
                ContinueMove();
            }
            else if (gameObject.name.Substring(4).Equals("North"))
            {
                followPosition.z = Mathf.Clamp(Mathf.Round(followPosition.z), -GameController.gameController.numOfRows, 0f);
                if (Mathf.RoundToInt(followPosition.z) != Mathf.RoundToInt(parentMR.bounds.max.z))
                {

                    float shift = (parentMR.bounds.min.z + followPosition.z) / 2f;
                    float scaleDistance;
                    if (followPosition.z >= 0f)
                    {
                        scaleDistance = followPosition.z - parentMR.bounds.max.z;
                    }
                    else
                    {
                        scaleDistance = Mathf.Abs(parentMR.bounds.max.z) - Mathf.Abs(followPosition.z);
                    }
                    float current = parentMR.bounds.size.z;
                    float scaleDistancePlusCurrent = scaleDistance + current;
                    if (!((scaleDistancePlusCurrent / current) * parentMR.bounds.size.z < 0.9f))
                    {
                        // Update object scale
                        if (Mathf.RoundToInt(transform.parent.transform.rotation.eulerAngles.y) % 180 == 0)
                        {
                            float newScale = transform.parent.transform.localScale.z * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                transform.parent.transform.localScale.x,
                                transform.parent.transform.localScale.y,
                                newScale
                            );
                        }
                        else
                        {
                            float newScale = transform.parent.transform.localScale.x * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                newScale,
                                transform.parent.transform.localScale.y,
                                transform.parent.transform.localScale.z
                            );
                        }

                        // Update object position
                        // Does not change upon rotation
                        transform.parent.position = new Vector3(
                            transform.parent.transform.position.x,
                            transform.parent.transform.position.y,
                            shift
                        );

                        // Update face location
                        transform.position = new Vector3(
                            transform.position.x,
                            transform.position.y,
                            parentMR.bounds.max.z + emphasis
                        );
                    }
                }
            }
            else if (gameObject.name.Substring(4).Equals("South"))
            {
                followPosition.z = Mathf.Clamp(Mathf.Round(followPosition.z), -GameController.gameController.numOfRows, 0f);
                if (Mathf.RoundToInt(followPosition.z) != Mathf.RoundToInt(parentMR.bounds.min.z))
                {
                    float shift = (parentMR.bounds.max.z + followPosition.z) / 2f;
                    float scaleDistance = Mathf.Abs(followPosition.z) - Mathf.Abs(parentMR.bounds.min.z);
                    float current = parentMR.bounds.size.z;
                    float scaleDistancePlusCurrent = scaleDistance + current;
                    if (!((scaleDistancePlusCurrent / current) * parentMR.bounds.size.z < 0.9f))
                    {
                        // Update object scale
                        if (Mathf.RoundToInt(transform.parent.transform.rotation.eulerAngles.y) % 180 == 0)
                        {
                            float newScale = transform.parent.transform.localScale.z * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                transform.parent.transform.localScale.x,
                                transform.parent.transform.localScale.y,
                                newScale
                            );
                        }
                        else
                        {
                            float newScale = transform.parent.transform.localScale.x * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                newScale,
                                transform.parent.transform.localScale.y,
                                transform.parent.transform.localScale.z
                            );
                        }

                        // Update object position
                        transform.parent.position = new Vector3(
                            transform.parent.transform.position.x,
                            transform.parent.transform.position.y,
                            shift
                        );

                        // Update face location
                        transform.position = new Vector3(
                            transform.position.x,
                            transform.position.y,
                            parentMR.bounds.min.z - emphasis
                        );
                    }
                }
            }
            else if (gameObject.name.Substring(4).Equals("East"))
            {
                followPosition.x = Mathf.Clamp(Mathf.Round(followPosition.x), 0f, GameController.gameController.numOfCols);
                if (Mathf.RoundToInt(followPosition.x) != Mathf.RoundToInt(parentMR.bounds.max.x))
                {
                    float shift = (parentMR.bounds.min.x + followPosition.x) / 2f;
                    float scaleDistance = Mathf.Abs(followPosition.x) - Mathf.Abs(parentMR.bounds.max.x);
                    float current = parentMR.bounds.size.x;
                    float scaleDistancePlusCurrent = scaleDistance + current;
                    if (!((scaleDistancePlusCurrent / current) * parentMR.bounds.size.x < 0.9f))
                    {
                        // Update object scale
                        if (Mathf.RoundToInt(transform.parent.transform.rotation.eulerAngles.y) % 180 == 0)
                        {
                            float newScale = transform.parent.transform.localScale.x * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                newScale,
                                transform.parent.transform.localScale.y,
                                transform.parent.transform.localScale.z
                            );
                        }
                        else
                        {
                            float newScale = transform.parent.transform.localScale.z * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                transform.parent.transform.localScale.x,
                                transform.parent.transform.localScale.y,
                                newScale
                            );
                        }

                        // Update object position
                        transform.parent.position = new Vector3(
                            shift,
                            transform.parent.transform.position.y,
                            transform.parent.transform.position.z
                        );

                        // Update face location
                        transform.position = new Vector3(
                            parentMR.bounds.max.x + emphasis,
                            transform.position.y,
                            transform.position.z
                        );
                    }
                }
            }
            else if (gameObject.name.Substring(4).Equals("West"))
            {
                followPosition.x = Mathf.Clamp(Mathf.Round(followPosition.x), 0f, GameController.gameController.numOfCols);
                if (Mathf.RoundToInt(followPosition.x) != Mathf.RoundToInt(parentMR.bounds.min.x))
                {
                    float shift = (parentMR.bounds.max.x + followPosition.x) / 2f;
                    float scaleDistance = Mathf.Abs(parentMR.bounds.min.x) - Mathf.Abs(followPosition.x);
                    float current = parentMR.bounds.size.x;
                    float scaleDistancePlusCurrent = scaleDistance + current;
                    if (!((scaleDistancePlusCurrent / current) * parentMR.bounds.size.x < 0.9f))
                    {
                        // Update object scale
                        if (Mathf.RoundToInt(transform.parent.transform.rotation.eulerAngles.y) % 180 == 0)
                        {
                            float newScale = transform.parent.transform.localScale.x * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                newScale,
                                transform.parent.transform.localScale.y,
                                transform.parent.transform.localScale.z
                            );
                        }
                        else
                        {
                            float newScale = transform.parent.transform.localScale.z * (scaleDistancePlusCurrent / current);
                            transform.parent.localScale = new Vector3(
                                transform.parent.transform.localScale.x,
                                transform.parent.transform.localScale.y,
                                newScale
                            );
                        }

                        // Update object position
                        transform.parent.position = new Vector3(
                            shift,
                            transform.parent.transform.position.y,
                            transform.parent.transform.position.z
                        );

                        // Update face location
                        transform.position = new Vector3(
                            parentMR.bounds.min.x - emphasis,
                            transform.position.y,
                            transform.position.z
                        );
                    }
                }
            }
        }
    }

    void ReturnToOriginal()
    {
        transform.parent.position = copyOfPosition;
        transform.parent.rotation = copyOfQuaternion;
        transform.parent.localScale = copyOfScale;

    }
}
