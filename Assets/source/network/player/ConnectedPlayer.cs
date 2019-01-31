using System;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ConnectedPlayer {

    private Player playerObject;
    public NetworkConnection connection;
    public string username;

    public ConnectedPlayer(NetworkConnection connection, Player player) {
        this.connection = connection;
        this.playerObject = player;
    }

    public NetworkConnection getConnection() {
        return this.connection;
    }

    /// <summary>
    /// Returns a reference to the Player GameObject.
    /// </summary>
    /// <returns></returns>
    public Player getPlayer() {
        return this.playerObject;
    }

    /// <summary>
    /// Sends a message to this player.
    /// </summary>
    public void sendMessage(AbstractMessageClient msg) {
        this.connection.Send(msg.getID(), msg);
    }

    /// <summary>
    /// If true, this player has disconnected.
    /// </summary>
    public bool isMissing() {
        return this.connection != null;
    }
}
