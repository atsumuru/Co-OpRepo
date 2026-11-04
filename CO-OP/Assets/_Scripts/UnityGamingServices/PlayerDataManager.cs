using UnityEngine;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using System;

public class PlayerDataManager : MonoBehaviour
{
    private MyModuleBindings MyModuleBindings;
    public LoginManager LoginManager;
    public string PlayerName;

    void Start()
    {
        LoginManager.PlayerSignedIn += InitializePlayer;

        MyModuleBindings = new MyModuleBindings(CloudCodeService.Instance);
    }

    private async void InitializePlayer()
    {
        try
        {
            var resultFromCloud = await MyModuleBindings.SayHello(PlayerName);
            Debug.Log($"{resultFromCloud}");
        }

        catch (CloudCodeException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void SaveNewPlayerName()
    {
        try
        {
            PlayerName = await MyModuleBindings.HandleNewPlayerNameEntry(PlayerName);
            Debug.Log($"Saved new player name in the cloud: {PlayerName}");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnDisable()
    {
        LoginManager.PlayerSignedIn -= InitializePlayer;
    }    
}
