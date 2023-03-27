using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float originalZoomValue = 6;
    private float originalSlideValue = 0;
    private float maximumZoomValue = 3;
    private float restartDelay = 0.5f;
    private bool resettingZoom = false;

    void FixedUpdate()
    {
        if(transform.position.y > maximumZoomValue)
        {
            float zoomValue = transform.position.y - 0.5f * Time.deltaTime;
            float slideValue = transform.position.x + 0.5f * Time.deltaTime;
            transform.position = new Vector3(slideValue, zoomValue, transform.position.z);
        } else if(!resettingZoom)
        {
            StartCoroutine(resetZoom());
        }
    }

    IEnumerator resetZoom()
    {
        resettingZoom = true;
        yield return new WaitForSeconds(restartDelay);
        transform.position = new Vector3(originalSlideValue, originalZoomValue, transform.position.z);
        resettingZoom = false;
    }
}
