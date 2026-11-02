using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    private async void Awake()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Services Initializing");
            await UnityServices.InitializeAsync();
        }

        PlayerAccountService.Instance.SignedIn += SignInOrLinkWithUnity;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        if(!AuthenticationService.Instance.SessionTokenExists)
        {
            Debug.Log("Session Token not found.");
            return;
        }

        Debug.Log("Returning player signing in...");
        await SignInAnonymouslyAsync();
    }

    public async void StartAnonymousSignIn()
    {
        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async void StartUnitySignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void SignInWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            // Prompt the player with an error message.
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    private async void SignInOrLinkWithUnity()
    {
        try
        {
            // 1. Player is not yet authenticated, siging up with Unity
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Signing up with Unity Player Account...");
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("Successfully signed up with Unity Player Account.");
                return;
            }

            // 2. Player is authenticated, but does not uet have a Unity ID linked, so let's link
            if (!HasUnityID())
            {
                Debug.Log("Linking anonumous account to Unity.");
                await LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                Debug.Log("Successfully linked anonymous account!");
                return;
            }

            // 3. Player has authentication and a Unity ID
            Debug.Log("Player is already signed in to their Unity Player Account");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private bool HasUnityID()
    {
        return AuthenticationService.Instance.PlayerInfo.GetUnityId() != null;
    }
}
