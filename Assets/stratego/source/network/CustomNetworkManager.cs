using fNbt;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager {

    private GameOptions currentOptions;
    private NetworkDiscovery discovery;
    private Board board;
    /// <summary> A List of all the connected players. </summary>
    public ConnectedPlayerList connectedPlayers;
    /// <summary> If true, at least one of the players in the game is not connected. </summary>
    public bool missingPlayers;

    public static CustomNetworkManager getSingleton() {
        return (CustomNetworkManager)CustomNetworkManager.singleton;
    }

    private void Awake() { // Override Awake(), there is a reason but I can't remember why...
        NetworkManager.singleton = this;
        this.discovery = this.GetComponent<NetworkDiscovery>();
    }

    #region Server Callbacks

    public override void OnStartServer() {
        base.OnStartServer();

        print("Starting Server");

        this.connectedPlayers = new ConnectedPlayerList();
        this.missingPlayers = false;

        // Start broadcasting the game so other players can see it.
        this.discovery.Initialize();
        this.discovery.StartAsServer();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
        // Get the Player's username.
        StringMessage im = new StringMessage();
        im.Deserialize(extraMessageReader);
        string username = im.value;

        // Create the Player object.
        Player player = GameObject.Instantiate(this.playerPrefab).GetComponent<Player>();
        NetworkServer.AddPlayerForConnection(conn, player.gameObject, playerControllerId);

        ConnectedPlayer newPlayer = null;

        bool makeNewPlayer = true;
        foreach(ConnectedPlayer cp in this.connectedPlayers) {
            if(cp.getUsername() == username) {
                // This player has been here before.
                
                // If a player with this username is already connected, kick the new joining player.
                if(cp.isConnected()) {
                    conn.Disconnect();
                    return;
                }

                // Update the existing Player.
                cp.setConnection(conn);
                newPlayer = cp;
                makeNewPlayer = false;
                break;
            }
        }

        if(makeNewPlayer) {
            newPlayer = new ConnectedPlayer(conn, username);
            this.connectedPlayers.Add(newPlayer);
        }        

        // Send the GameOptions to the newly connected Player, as everyone should have this.
        newPlayer.sendMessage(new MessageSendGameOptions(this.currentOptions));

        if(this.board.gameState == GameStates.WAITING) {
            // The connection happened while waiting for Players, start the game if there are enough players.
            if(this.getNeededPlayers() == 0) {
                // Start the game!
                this.board.startSetupPhase();
            } else {
                // Update the waiting player on how many more must join.
                this.sendNeedPlayersMsg();
            }
        } else if(this.board.gameState == GameStates.SETUP || this.board.gameState == GameStates.PLAYING) {
            if(makeNewPlayer) {
                newPlayer.setSpectating();
            } else {
                // Tell the rejoining player their team.
                this.board.sendTeamInfo(newPlayer);
                print("sending info");
            }
        } else if(this.board.gameState == GameStates.PLAYING) {
            if(!makeNewPlayer) {
                // Tell them who's turn it is.
                newPlayer.sendMessage(new MessageChangeTurn(this.board.getCurrentPlayer().getTeam()));
            }
        }

        this.updateMissingPlayersFlag();
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);

        // Remove the connection reference from the Player who disconencted.
        this.connectedPlayers.getFromNetworkConnection(conn).setConnection(null);

        this.sendNeedPlayersMsg();

        this.updateMissingPlayersFlag();
    }

    public override void OnStopServer() {
        base.OnStopServer();

        this.discovery.StopBroadcast();
    }

    #endregion

    #region Client Callbacks

    [ClientSideOnly]
    public override void OnStartClient(NetworkClient client) {
        new NetHandlerClient();
    }

    [ClientSideOnly]
    public override void OnClientConnect(NetworkConnection conn) {
        ClientScene.AddPlayer(conn, 0, new StringMessage(Username.getUsername()));
    }

    [ClientSideOnly]
    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        this.showDisconnectedScreen("Disconnected from Server", "Ok");
    }

    [ClientSideOnly]
    public override void OnClientError(NetworkConnection conn, int errorCode) {
        base.OnClientError(conn, errorCode);
        this.showDisconnectedScreen("Error: " + ((NetworkError)errorCode).ToString(), "Ok");
    }

    [ClientSideOnly]
    public override void OnStopClient() {
        base.OnStopClient();

        // Close the popups and go back to the title screen.
        GameObject.FindObjectOfType<UIManager>().closeCurrent();
        ScreenManager.singleton.showScreen(ScreenManager.singleton.screenMainMenu);
    }

    [ClientSideOnly]
    private void showDisconnectedScreen(string msg, string btnText) {
        ScreenInfo si = ScreenManager.singleton.screenInfo;
        si.setMessage(msg, btnText);
        ScreenManager.singleton.showScreen(si);
    }

    #endregion

    /// <summary>
    /// Starts up the game and creates a server to run it.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="boardNbt">The Nbt tag to read the board from.  Pass null for new games.</param>
    public void startGameServer(GameOptions options, NbtCompound boardNbt) {
        this.currentOptions = options;

        this.StartHost();
        this.board = BoardType.getBoardFromPlayerCount(this.currentOptions.maxPlayers.get()).instantiateBoardPrefab();
        this.board.allPlayers = this.connectedPlayers;

        if(boardNbt != null) {
            this.board.readFromNbt(boardNbt);
        } else {
            this.board.initNewGame(options);
        }

        NetworkServer.Spawn(this.board.gameObject);
    }

    /// <summary>
    /// Returns this GameOptions object being used in the current game.
    /// </summary>
    public GameOptions getGameOptions() {
        return this.currentOptions;
    }

    public Board getBoard() {
        return this.board;
    }

    /// <summary>
    /// Returns the NetworkDiscovery object.
    /// </summary>
    public NetworkDiscovery getDiscovery() {
        return this.discovery;
    }

    /// <summary>
    /// Returns how many Players are still needed to have enough to start the game.
    /// </summary>
    private int getNeededPlayers() {
        int count = 0;
        foreach(ConnectedPlayer cp in this.connectedPlayers) {
            if(cp.isConnected()) {
                count++;
            }
        }
        return this.currentOptions.maxPlayers.get() - count;
    }

    private void sendNeedPlayersMsg() {
        this.connectedPlayers.sendMessage(new MessageShowNeededPlayers(this.getNeededPlayers()));
    }

    private void updateMissingPlayersFlag() {
        bool missing = false;
        foreach(ConnectedPlayer player in this.connectedPlayers) {
            if(player.hasTeam() && !player.isConnected()) {
                missing = true;
                break;
            }
        }
        this.missingPlayers = missing;
        this.connectedPlayers.sendMessage(new MessageMissingPlayers(missing));
    }
}