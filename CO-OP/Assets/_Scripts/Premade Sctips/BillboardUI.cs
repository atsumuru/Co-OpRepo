using UnityEngine;

/// <summary>
/// THis script makes the UI Document showing player tag / name always face the camera.
/// </summary>
public class BillboardUI : MonoBehaviour
{
    private Transform m_cameraTransform;

    private void Awake()
    {
        FindCamera();
    }

    private void FindCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            m_cameraTransform = mainCamera.transform;
        }
        else
        {
            m_cameraTransform = null;
        }
    }

    private void LateUpdate()
    {
        // Find the camera again if the old one was destroyed.
        if (m_cameraTransform == null)
        {
            FindCamera();
            if (m_cameraTransform == null)
                return;
        }

        Vector3 direction = transform.position - m_cameraTransform.position;

        // Keep the billboard upright.
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
