using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : NetworkBehaviour
{
    private Camera m_camera;

    Vector3 m_offsetFromPlayer;
    Vector3 m_originPosition;

    [SerializeField]
    private float m_horizontalOffset = 0f;

    private bool m_offsetInitialized = false;
    private Coroutine m_recenterCoroutine;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner == false)
            return;

        SceneManager.sceneLoaded += OnSceneLoaded;

        FindCamera();

        if (m_camera != null && m_offsetInitialized == false)
        {
            m_originPosition = m_camera.transform.position;

            // save the original camera offset only once
            m_offsetFromPlayer = m_camera.transform.position - transform.position;

            m_offsetInitialized = true;
        }
    }
 
    private void FindCamera()
    {
        m_camera = Camera.main;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsOwner == false)
            return;

        m_horizontalOffset = 0f;

        if (m_recenterCoroutine != null)
        {
            StopCoroutine(m_recenterCoroutine);
        }

        m_recenterCoroutine = StartCoroutine(RecenterCamera());
    }

    private IEnumerator RecenterCamera()
    {
        // wait for the new scene to finish initializing
        yield return null;
        yield return new WaitForEndOfFrame();

        RecenterNow();

        m_recenterCoroutine = null;
    }

    public void RecenterNow()
    {
        if (IsOwner == false)
            return;

        FindCamera();

        if (m_camera == null || m_offsetInitialized == false)
            return;

        m_horizontalOffset = 0f;

        m_camera.transform.position = transform.position + m_offsetFromPlayer;
    }

    public override void OnNetworkDespawn()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (m_recenterCoroutine != null)
        {
            StopCoroutine(m_recenterCoroutine);
            m_recenterCoroutine = null;
        }

        if (IsOwner && m_camera != null)
        {
            m_camera.transform.position = m_originPosition;
        }

        m_camera = null;
        base.OnNetworkDespawn();
    }

    private void LateUpdate()
    {
        if (IsOwner == false)
            return;

        // Find the new scene's camera if the old one was destroyed.
        if (m_camera == null)
        {
            FindCamera();

            if (m_camera == null || m_offsetInitialized == false)
                return;
        }

        // keep following the player continuously
        Vector3 targetPosition = transform.position + m_offsetFromPlayer;
        targetPosition.x += m_horizontalOffset;
        m_camera.transform.position = targetPosition;
    }
}
