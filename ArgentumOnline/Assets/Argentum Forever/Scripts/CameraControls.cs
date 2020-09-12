using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FIXME cambiar el orthographic size en runtime entra con conflicto con el plugin Pixel Perfect Camera
public class CameraControls : MonoBehaviour
{
    // Zoom levels
    [SerializeField] private float mDefaultZoom = 5f;
    [SerializeField] private float mMinZoom = 7f;
    [SerializeField] private float mMaxZoom = 1.5f;
    [SerializeField] private float mZoomStep = 0.25f;

    #region component cache
    private CinemachineVirtualCamera rVcam;
    #endregion

    #region unity loop
    private void Awake()
    {
        // Load the component cache
        SetupComponentCache();

        // Set the default zoom level
        rVcam.m_Lens.OrthographicSize = mDefaultZoom;
    }

    private void Update()
    {
        // Check mouse wheel input
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // Check zoom level
            if (rVcam.m_Lens.OrthographicSize > mMaxZoom)
            {
                rVcam.m_Lens.OrthographicSize -= mZoomStep;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // Check zoom level
            if (rVcam.m_Lens.OrthographicSize < mMinZoom)
            {
                rVcam.m_Lens.OrthographicSize += mZoomStep;
            }
        }
    }
    #endregion

    private void SetupComponentCache()
    {
        rVcam = GetComponent<CinemachineVirtualCamera>();
    }
}
