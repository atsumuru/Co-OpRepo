
using Unity.Netcode;
using UnityEngine;

public class ResourceDropZone : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private int m_maxResources = 1;

    [SerializeField]
    private SelectionOutline m_selectionOutline;

    private NetworkVariable<int> m_resourceCount = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public int ResourceCount => m_resourceCount.Value;

    public bool Interact(ObjectType objectType)
    {
        if (IsServer == false)
            return false;

        // Only accept star resources.
        if (objectType != ObjectType.Star)
            return false;

        // Do not accept resources when the crate is full.
        if (m_resourceCount.Value >= m_maxResources)
            return false;

        m_resourceCount.Value++;

        return true;
    }


    public void ToggleSelection(bool isSelected)
    {
        if (m_selectionOutline != null)
        {
            m_selectionOutline.ToggleOutline(isSelected);
        }
    }
}
