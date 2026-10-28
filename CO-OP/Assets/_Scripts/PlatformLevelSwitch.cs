
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformLevelSwitch : MonoBehaviour
{
    [SerializeField]
    private string m_nextSceneName = "Scene1";

    private bool m_isChangingScene;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Water platform touched by: " + other.name);

        NetworkManager manager = NetworkManager.Singleton;

        if (manager == null || !manager.IsServer)
        {
            return;
        }

        NetworkObject player =
            other.GetComponentInParent<NetworkObject>();

        if (player == null || !player.IsPlayerObject)
        {
            Debug.Log("Collider does not belong to a player.");
            return;
        }

        if (m_isChangingScene)
        {
            return;
        }

        if (!manager.IsListening || manager.SceneManager == null)
        {
            Debug.LogError("Netcode is not ready for scene switching.");
            return;
        }

        SceneEventProgressStatus result =
            manager.SceneManager.LoadScene(
                m_nextSceneName,
                LoadSceneMode.Single
            );

        if (result == SceneEventProgressStatus.Started)
        {
            m_isChangingScene = true;
            Debug.Log("Loading scene: " + m_nextSceneName);
        }
        else
        {
            Debug.LogError("Scene load failed: " + result);
        }
    }
}
