using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject m_woodPrefab, m_stonePrefab, m_starPrefab;

    public void SpawnResource(ObjectType type, Vector3 position)
    {
        if(IsServer == false)return;

        NetworkObject resource = type == ObjectType.Wood ? m_woodPrefab : m_stonePrefab;

        // 25% chance to spawn a star resource.
        if (Random.Range(0, 100) < 25)
        {
            resource = m_starPrefab;
            Debug.Log("Spawning star resource!");
        }
        else if (type == ObjectType.Wood)
        {
            resource = m_woodPrefab;
        }
        else
        {
            resource = m_stonePrefab;
        }

        // Check the selected prefab.
        if (resource == null)
        {
            Debug.LogError("Selected resource prefab is NULL!");
            return;
        }

        Quaternion rotation;

        if (resource == m_starPrefab)
        {
            rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            // Random Y rotation for wood and stone.
            rotation = Quaternion.Euler(
                0f,
                Random.Range(0, 360),
                0f
            );
        }

        GameObject instance = Instantiate(resource.gameObject, position, Quaternion.Euler(0,UnityEngine.Random.Range(0,360),0));

        // Keep the resource in the spawner's scene.
        SceneManager.MoveGameObjectToScene(instance, gameObject.scene);

        // Activate the resource if the prefab is inactive.
        instance.SetActive(true);

        NetworkObject networkObject = instance.GetComponent<NetworkObject>();

        // Scene currentScene = gameObject.scene;

        networkObject.Spawn();
    }
}
