    using UnityEngine;
    using Unity.Services.Core;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Netcode;
    using System;
    using TMPro;

    public class LobbyManager : MonoBehaviour
    {
        private Lobby _hostLobby;
        private Lobby _joinLobby;
        private float _heartbeatTimer;
        private float lobbyupdateTimer;

        public TMP_Text playerNamesText;

        private string playerName = "Za";
        public static event Action OnLobbyCreated;
        public static event Action OnLobbyJoined;

        public bool _isHost = false;
        public static LobbyManager Instance { get; private set; }
        public static NetworkManager NetworkManager { get; private set; }

        private bool isTestingLocally = false;
        private CustomLocalLobby _localLobby; // For local testing

        private void Awake()
    {
        // Ensure there's only ONE NetworkManager
        if (NetworkManager.Singleton == null)
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Subscribe to the client connection event
                // NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
            return;
        }
    
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to the client connection event
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        else
        {
            Debug.LogWarning("⚠️ Duplicate LobbyManager detected and destroyed!");
            Destroy(gameObject);
        }
    }




        private async void Start()
        {
            // isTestingLocally = Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer;

            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                playerName = "Za" + UnityEngine.Random.Range(1000, 9999);

                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);


            }else{
                Debug.Log("Player is already signed in as: " + AuthenticationService.Instance.PlayerId);

            }


            Debug.Log("Player Name: " + playerName);    
            
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdates();
        }

        private async Task HandleLobbyHeartbeat()
        {
            if (_hostLobby != null)
            {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f)
                {
                    _heartbeatTimer = 15f;
                    try
                    {
                        await Lobbies.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                    }
                    catch (LobbyServiceException e)
                    {
                        Debug.LogError("Failed to heartbeat lobby: " + e.Message);
                    }
                }

            }
        }

        public string GetLobbyCode()
        {
            if (isTestingLocally)
            {
                return _localLobby?.LobbyCode ?? "No Lobby Code Available";
            }
            return _hostLobby != null ? _hostLobby.LobbyCode : "No Lobby Code Available";
        }

        public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
        {
            if (isTestingLocally)
            {
                _localLobby = new CustomLocalLobby();
                _isHost = true;
                Debug.Log("Created local test lobby: " + _localLobby.LobbyCode);
                OnLobbyCreated?.Invoke();
                return;
            }

            try
            {
                
                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject> //here we can set the player role in the lobby and other data
                        {
                            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)},
                            {"Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "DefaultCharacter") },
                            // {
                            //     "PlayerRole", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Host")
                            // }
                        }
                    },
                    
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "DefaultMode") } //here we can set the game mode
                    }
                };

                _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                _joinLobby = _hostLobby;
                printPlayers(_hostLobby);
                _isHost = true;
                Debug.Log("Lobby created! Lobby Code: " + _hostLobby.LobbyCode);
                OnLobbyCreated?.Invoke();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to create lobby: " + e.Message);
            }
        }

        public async Task JoinLobbyByCode(string lobbyCode)
        {
            if (isTestingLocally)
            {
                Debug.Log("Joined local test lobby: " + _localLobby.LobbyCode);
                return;
            }

            try
            {
                JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };
                
                
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinOptions);
                _joinLobby = lobby;
                Debug.Log("Joined lobby with code: " + _joinLobby.LobbyCode);
                OnLobbyJoined?.Invoke();
                printPlayers(_joinLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to join lobby: " + e.Message);
            }
        }


        public async Task UpdatePlayerCharacter(string selectedCharacter){
            try{
                await LobbyService.Instance.UpdatePlayerAsync(_joinLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, selectedCharacter) }
                    }
                });

                Debug.Log("Updated player character to: " + selectedCharacter);
            }

            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to update player data: " + e.Message);
            }
        }

        public string GetPlayerCharacter(string playerId){
            if (_joinLobby == null)
            {
                Debug.LogWarning("No lobby joined.");

                return "DefaultCharacter";
            }

            foreach (Player player in _joinLobby.Players)
            {
                if (player.Id == playerId && player.Data.ContainsKey("Character"))
                {
                    Debug.Log("Player character: " + player.Data["Character"].Value);
                    return player.Data["Character"].Value;
                }
            }

            return "DefaultCharacter";
        }

        private void PrintPlayers(){
            printPlayers(_joinLobby);
        }


        public void StartGame()
        {
            if (_isHost)
            {
                Debug.Log("Host is starting the game...");
                NetworkManager.Singleton.StartHost();

                string sceneToLoad = "map-creation";

                if (NetworkManager.Singleton.SceneManager != null)
                {
                    Debug.Log("🔄 Loading Scene: " + sceneToLoad);
                    NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);
                }
                else
                {
                    Debug.LogError("⚠️ SceneManager is null! Ensure 'Use Scene Management' is enabled in NetworkManager.");
                }
            }
            else
            {
                Debug.Log("Client is joining the game...");
                if (NetworkManager.Singleton == null)
                {
                    Debug.LogError("⚠️ No NetworkManager found! Clients must not create their own NetworkManager.");
                    return;
                }
                NetworkManager.Singleton.StartClient();
            }
        }


        private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("🎮 New client connected: " + clientId);

            // Force new players to the correct scene
            NetworkManager.Singleton.SceneManager.LoadScene("NPCMoveTesting", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

        


        public async void ListLobbies()
        {
            try
            {
                QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
                foreach (Lobby lobby in response.Results)
                {
                    Debug.Log("Lobby: " + lobby.LobbyCode);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to query lobbies: " + e.Message);
                return;
            }
        }

        private void printPlayers(Lobby lobby){
            Debug.Log("Players in lobby: " + lobby.LobbyCode);  
            playerNamesText.text = "";
            foreach (Player player in lobby.Players)
            {
                Debug.Log("Player: " + player.Id + " " + player.Data["PlayerName"].Value + " " + player.Data["Character"].Value);
                playerNamesText.text += player.Data["PlayerName"].Value + "\n";
                // Debug.Log("Player: " + player.Id + " " + player.Data["Character"].Value);
            
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (_joinLobby != null)
            {
                lobbyupdateTimer -= Time.deltaTime;
                if (lobbyupdateTimer <= 0)
                {
                    lobbyupdateTimer = 5f;
                    try
                    {
                        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinLobby.Id);
                        _joinLobby = lobby;
                        printPlayers(_joinLobby);
                    }
                    catch (LobbyServiceException e)
                    {
                        Debug.LogError("Failed to update lobby: " + e.Message);
                    }
                }
            }
        }



        private async void UpdatePlayerName(string newPlayerName){
            try{
                playerName = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(_joinLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to update player data: " + e.Message);
            }
        }

        private async void LeaveLobby(){
            try{
                await LobbyService.Instance.RemovePlayerAsync(_joinLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to leave lobby: " + e.Message);
            }
        }

        private async void KickPlayer(){
            try {
                await LobbyService.Instance.RemovePlayerAsync(_joinLobby.Id,_joinLobby.Players[1].Id);
            } catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to kick player: " + e.Message);
            }
        }

        private async void DeleteLobby(){
            try{
                await LobbyService.Instance.DeleteLobbyAsync(_joinLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to delete lobby: " + e.Message);
            }
        }



        private async void UpdateLobbyGameMode(string gameMode)
        {
            try {
                _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
                }
            });

            _joinLobby = _hostLobby;

            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to update lobby data: " + e.Message);

            }
            
        }

        private Player GetPlayer(){
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                    { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "wizard-sheet_0") }

                }
            };
        }

    }

    class CustomLocalLobby
    {
        public string LobbyCode { get; private set; }
        public List<string> Players { get; private set; }

        public CustomLocalLobby()
        {
            LobbyCode = "LOCAL_" + UnityEngine.Random.Range(1000, 9999);
            Players = new List<string> { "LocalHost" };
        }
    }