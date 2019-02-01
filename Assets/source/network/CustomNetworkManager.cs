using System;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    [SerializeField]
    private Board board;

    private NetworkDiscovery discovery;

    private void Start() {
        this.discovery = this.GetComponent<NetworkDiscovery>();
    }

    public override void OnStartServer() {
        base.OnStartServer();

        this.discovery.Initialize();
        this.discovery.StartAsServer();

        this.board.init();
    }

    public override void OnStopServer() {
        base.OnStopServer();

        print("Shutting down server...");

        this.discovery.StopBroadcast();

        this.board.saveGame();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        Team team = this.board.availibleTeams.getRandomTeam();
        int teamId = team.getId();

        Transform baseOrgin;
        int j = this.board.availibleTeams.teamToSliceMapping[team];
        baseOrgin = this.board.slices[j].getOrgin();

        GameObject playerGameObj = GameObject.Instantiate(this.playerPrefab, baseOrgin.transform.position, baseOrgin.transform.rotation);
        Player player = playerGameObj.GetComponent<Player>();
        NetworkServer.AddPlayerForConnection(conn, playerGameObj, playerControllerId);

        // Set the Players Team.
        player.controllingTeamID = team.getId();

        ConnectedPlayer cp = new ConnectedPlayer(conn, player);

        this.board.allPlayers.Add(cp);

        int i = GameOptions.maxPlayers - this.board.allPlayers.Count;
        this.board.sendMessageToAll(new MessageShowText("Waiting for " + i + " more Player(s)",  1f).showInCorner());

        // Check if we have enough players
        if(i == 0) {
            print("i == 0");
            // We do
            this.board.gameState = 1; // Setup.
            this.board.sendMessageToAll(new MessageShowText("Setup Phase"));
            this.board.sendMessageToAll(new MessageShowText(string.Empty).showInCorner());
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);

        //TODO use the method in Map to get the connected player.
        for(int i = this.board.allPlayers.Count - 1; i >= 0; i--) {
            ConnectedPlayer connectedPlayer = this.board.allPlayers[i];
            if(connectedPlayer.getConnection().connectionId == conn.connectionId) {
//                this.board.allPlayers[i].connection = null;

                this.board.allPlayers.RemoveAt(i);
                //this.board.savePlayer(connectedPlayer);
//                this.board.availibleTeams.freeTeam(connectedPlayer.getTeam());
                return;
            }
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);

        Main.singleton.state = 1;
    }
}