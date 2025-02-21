using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ServerMiniGameManager : NetworkBehaviour
{
    public static ServerMiniGameManager Instance;
    private Dictionary<ulong, string> playerPowerups = new Dictionary<ulong, string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterMiniGameCompletionServerRpc(ulong playerId)
    {
        Debug.Log($"🏆 Player {playerId} won a minigame!");

        // Give a random power-up
        string[] powerupPool = { "BlurVision", "ScrambleJournal", "SlowTime", "SpeedBoost", "RevealFalseClues" };
        string randomPowerup = powerupPool[Random.Range(0, powerupPool.Length)];
        playerPowerups[playerId] = randomPowerup;

        NotifyPowerupUsedClientRpc(randomPowerup, playerId);
    }

    [ClientRpc]
    private void NotifyPowerupUsedClientRpc(string powerup, ulong userId)
    {
        Debug.Log($"🔔 Power-up {powerup} used by Player {userId}!");
    }

    public string GetPlayerPowerup(ulong playerId)
    {
        return playerPowerups.ContainsKey(playerId) ? playerPowerups[playerId] : null;
    }

    public void ConsumePowerup(ulong playerId)
    {
        if (playerPowerups.ContainsKey(playerId))
        {
            Debug.Log($"🛑 Power-up {playerPowerups[playerId]} consumed by Player {playerId}");
            playerPowerups.Remove(playerId);
        }
    }
}
