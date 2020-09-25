using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FIXME cambiar el orthographic size en runtime entra con conflicto con el plugin Pixel Perfect Camera
public class CameraControls : MonoBehaviour
{
    // Zoom levels
    [SerializeField] private float mDefaultZoom;
    [SerializeField] private float mMinZoom;
    [SerializeField] private float mMaxZoom;
    [SerializeField] private float mZoomStep;

    private CinemachineVirtualCamera vCam;
    
    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.m_Lens.OrthographicSize = mDefaultZoom;
    }

    private void Update()
    {
        // Check mouse wheel input
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // Check zoom level
            if (vCam.m_Lens.OrthographicSize > mMaxZoom)
            {
                DOTween.To(
                    x => vCam.m_Lens.OrthographicSize = x,
                    vCam.m_Lens.OrthographicSize,
                    vCam.m_Lens.OrthographicSize - mZoomStep,
                    0.2f
                );
                //rVcam.m_Lens.OrthographicSize -= mZoomStep;
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // Check zoom level
            if (vCam.m_Lens.OrthographicSize < mMinZoom)
            {
                DOTween.To(
                    x => vCam.m_Lens.OrthographicSize = x,
                    vCam.m_Lens.OrthographicSize,
                    vCam.m_Lens.OrthographicSize + mZoomStep,
                    0.2f
                );
                //rVcam.m_Lens.OrthographicSize += mZoomStep;
            }
        }
    }
}