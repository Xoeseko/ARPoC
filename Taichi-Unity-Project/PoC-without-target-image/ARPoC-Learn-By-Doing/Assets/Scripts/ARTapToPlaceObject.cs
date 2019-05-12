using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool playAreaSet = false;
    public GameObject FirstAnimation;
    public GameObject SecondAnimation;
    public GameObject welcomeMessage;
    public GameObject warningMessage;
    public GameObject boxingText;
    public GameObject taichiText;
    public GameObject instructions;
    public float fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        if (!playAreaSet)
        {
            UpdatePlacementIndicator();
        }
        else
        {
            welcomeMessage.SetActive(false);
            checkAlwaysAPlane();
        }

        //Check if a plane was detected and the screen was touched if so. place an object and block the playarea
        if(placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
            playAreaSet = true;
        }
    }

    private void PlaceObject()
    {
        if(SecondAnimation.activeInHierarchy)
        {
            StartCoroutine(textForTime(boxingText));
            SecondAnimation.SetActive(false);
            FirstAnimation.SetActive(true);
            FirstAnimation.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            StartCoroutine(textForTime(taichiText));
            FirstAnimation.SetActive(false);
            SecondAnimation.SetActive(true);
            SecondAnimation.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        StartCoroutine(textForTime(instructions));


    }

    private void UpdatePlacementIndicator()
    {
        if(placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if(placementPoseIsValid)
        {

            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            //Using normalized vector because camera
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }

    }

    private IEnumerator textForTime(GameObject text)
    {
        text.SetActive(true);
        yield return new WaitForSeconds(fadeTime);
        text.SetActive(false);

    }

    private void checkAlwaysAPlane()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        if(hits.Count < 1)
        {
            warningMessage.SetActive(true);
        }
        else
        {
            warningMessage.SetActive(false);
        }
    }
}
