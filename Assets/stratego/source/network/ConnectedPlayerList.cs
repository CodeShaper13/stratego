using System.Collections.Generic;
using UnityEngine.Networking;

public class ConnectedPlayerList : List<ConnectedPlayer> {

    public ConnectedPlayer getFromNetworkConnection(NetworkConnection conn) {
        foreach(ConnectedPlayer cp in this) {
            if(cp.getConnection().connectionId == conn.connectionId) {
                return cp;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the ConnectedPlayer who controls the passed Piece.
    /// </summary>
    public ConnectedPlayer getFromPiece(Piece piece) {
        foreach(ConnectedPlayer player in this) {
            if(player.getTeam() == piece.getTeam()) {
                return player;
            }
        }
        return null;
    }

    /// <summary>
    /// Sends the passed message to all of the ConnectedPlayers.
    /// </summary>
    public void sendMessage(AbstractMessageServer message) {
        foreach(ConnectedPlayer cp in this) {
            cp.sendMessage(message);
        }
    }
}
