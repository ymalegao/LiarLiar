using UnityEngine;
using Unity.Netcode;

public class PowerupEffects : NetworkBehaviour
{
    public static PowerupEffects Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private ulong GetOpponentId(ulong playerId)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key != playerId)
                return client.Key;
        }
        return playerId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivatePowerupServerRpc(ulong playerId, string powerup)
    {
        ulong targetId = GetOpponentId(playerId);
        NotifyPowerupUsedClientRpc(powerup, playerId);

        switch (powerup)
        {
            case "BlurVision":
                BlurPlayerScreenClientRpc(targetId);
                break;
            case "ScrambleJournal":
                ScramblePlayerJournalClientRpc(targetId);
                break;
            case "SlowTime":
                SlowPlayerMovementClientRpc(targetId);
                break;
            case "SpeedBoost":
                SpeedBoostPlayerClientRpc(playerId);
                break;
            case "RevealFalseClues":
                RevealFalseCluesClientRpc(targetId);
                break;
        }

        ServerMiniGameManager.Instance.ConsumePowerup(playerId);
    }

    [ClientRpc]
    private void NotifyPowerupUsedClientRpc(string powerup, ulong userId)
    {
        Debug.Log($"🔔 Power-up {powerup} used by Player {userId}!");
    }

    [ClientRpc]
    private void BlurPlayerScreenClientRpc(ulong targetId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetId)
        {
            Debug.Log("👀 Your screen is now blurred!");
            // Implement screen blur effect
        }
    }

    [ClientRpc]
    private void ScramblePlayerJournalClientRpc(ulong targetId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetId)
        {
            Debug.Log("📖 Your journal is scrambled!");
            // Implement scrambling effect
        }
    }

    [ClientRpc]
    private void SlowPlayerMovementClientRpc(ulong targetId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetId)
        {
            Debug.Log("🐌 Your movement is slowed!");
            // Implement movement slow
        }
    }

    [ClientRpc]
    private void SpeedBoostPlayerClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            Debug.Log("⚡ You have a speed boost!");
            // Implement speed boost
        }
    }

    [ClientRpc]
    private void RevealFalseCluesClientRpc(ulong targetId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetId)
        {
            Debug.Log("🕵️ False clues have been revealed!");
            // Implement false clue highlighting
        }
    }
}
