using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Board : NetworkBehaviour {

   
    [Header("")]

    /// <summary> The state of the game. 0 = waiting for players, 1 = setup, 2 = playing, 3 = end. </summary>
    [SyncVar]
    public int gameState;

    public List<Cell> allCells = new List<Cell>();

    /// <summary> A List of all the connected players. </summary>
    [ServerSideOnly]
    public List<ConnectedPlayer> allPlayers;
    [ServerSideOnly]
    public int turnIndex;
    [ServerSideOnly]
    public AvailibleTeams availibleTeams;
    [ServerSideOnly]
    private NetHandlerServer handler;
    [ServerSideOnly]
    public Attack attack;

    private void Awake() {
        // Modify the board to look better and save the dev some tedious work in the editor.
        float f = 1.1f;
        int index = 0;

        foreach(Slice slice in this.GetComponentsInChildren<Slice>()) {
            foreach(Transform t in slice.transform) {
                // Spread them out.
                t.localPosition = t.localPosition * f;
                Cell cell = t.gameObject.GetComponent<Cell>();
                cell.cellIndex = index++;

                string s = cell.GetComponent<MeshRenderer>().material.name.Replace("(Instance)", "").Trim();

                if(s == "Water") {
                    cell.isWater = true;
                }
                else if(s == "BaseRed") {
                    cell.teamID = Team.RED.getId();
                }
                else if(s == "BaseBlue") {
                    cell.teamID = Team.BLUE.getId();
                }
                else if(s == "BaseYellow") {
                    cell.teamID = Team.YELLOW.getId();
                }
                else if(s == "BaseGreen") {
                    cell.teamID = Team.GREEN.getId();
                }

                this.allCells.Add(cell);
            }
        }
    }

    public override void OnStartServer() {
        base.OnStartServer();

        this.allPlayers = new List<ConnectedPlayer>();

        this.availibleTeams = new AvailibleTeams(GameOptions.maxPlayers);

        this.handler = new NetHandlerServer(this);

        this.gameState = GameStates.WAITING;
        this.attack = null;

        // TODO loading happens here?
    }

    public override void OnStartClient() {
        base.OnStartClient();
    }

    private void Update() {
        if(this.isServer) {
            if(Input.GetKeyDown(KeyCode.F3)) {
                this.startGame();
            }

            if(!this.availibleTeams.moreRoom()) {
                bool allReady = true;
                foreach(ConnectedPlayer player in this.allPlayers) {
                    allReady = player.getPlayer().isReady;
                }
                if(allReady) {
                    this.startGame();
                }
            }

            if(this.gameState == GameStates.PLAYING) {
                if(this.attack != null) {
                    if(this.attack.update()) {
                        this.attack = null;
                        //this.nextTurn();
                    }
                }
            }
        }
    }

    private void startGame() {
        print("Starting Game...");
        this.gameState = GameStates.PLAYING;
        this.sendMessageToAll(new MessageShowText("The Game Has Begun", 1));
        this.turnIndex = Random.Range(0, this.allPlayers.Count) - 1;
        this.nextTurn();
    }

    public void nextTurn() {
        this.turnIndex += 1;
        if(this.turnIndex >= this.allPlayers.Count) {
            this.turnIndex = 0;
        }

        ConnectedPlayer playerWithTurn = this.allPlayers[this.turnIndex];

        for(int i = 0; i < this.allPlayers.Count; i++) {
            ConnectedPlayer connectedPlayer = this.allPlayers[i];
            
            connectedPlayer.sendMessage(new MessageChangTeurn(playerWithTurn.getPlayer().controllingTeamID, i == this.turnIndex));
        }
    }

    public ConnectedPlayer connectedPlayerFromNetworkConnection(NetworkConnection conn) {
        foreach(ConnectedPlayer connectedPlayer in this.allPlayers) {
            if(connectedPlayer.getConnection().connectionId == conn.connectionId) {
                return connectedPlayer;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the cell from the passed cell index/identifier.
    /// </summary>
    public Cell getCellFromIndex(int index) {
        return this.allCells[index];
    }

    public void saveGame() {
        //TODO implement
    }

    public ConnectedPlayer getConnectedPlayer(Piece piece) {
        foreach(ConnectedPlayer player in this.allPlayers) {
            if(player.getPlayer().controllingTeamID == piece.teamId) {
                return player;
            }
        }
        return null;
    }

    /// <summary>
    /// Sends a message to all of the connected players.
    /// </summary>
    [ServerSideOnly]
    public void sendMessageToAll(AbstractMessageClient message) {
        foreach(ConnectedPlayer cp in this.allPlayers) {
            print("Sending message to " + cp.getConnection());
            cp.sendMessage(message);
        }
    }
}
