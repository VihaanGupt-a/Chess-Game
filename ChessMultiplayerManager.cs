using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class ChessMultiPlayerManager : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log(
                $"Multiplayer services ready! Player ID: " + AuthenticationService.Instance.PlayerId
            );
        }

        catch (Exception e)
        {
            Debug.LogError($"Multiplayer Initialization failed : {e}");
        }
    }
}
