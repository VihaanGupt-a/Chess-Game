using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;

public class ChessMultiplayerManager : MonoBehaviour
{

    private ISession currentSession;
    public bool IsInSession => currentSession != null;
    
    public System.Action<string> RoomCodeCreated;

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

    public async Task CreateRoom()
    {
        try
        {
            var options = new SessionOptions
            {
                MaxPlayers = 2
            }.WithRelayNetwork();

            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            Debug.Log($"Room Created! Code: {currentSession.Code}");

            RoomCodeCreated?.Invoke(currentSession.Code);
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not create room: {e}");
        }
    }

    public async Task JoinRoom(string roomCode)
    {
        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(roomCode);
            Debug.Log($"Joined room: {currentSession.Code}");
        }

        catch (Exception e)
        {
            Debug.LogError($"Could not join room: {e}");
        }
    }

}
