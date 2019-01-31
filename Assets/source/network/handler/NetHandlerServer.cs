using UnityEngine;
using UnityEngine.Networking;

public class NetHandlerServer : NetHandlerBase {

    private Board board;

    public NetHandlerServer(Board board) {
        this.board = board;
    }

    protected override void registerHandlers() {
        this.registerMsg<MessageAddPiece>();
        this.registerMsg<MessageMovePiece>();
        this.registerMsg<MessageRemovePiece>();
    }

    public void addPiece(MessageAddPiece msg) {
        int pieceType = msg.pieceId;
        Cell cell = this.board.getCellFromIndex(msg.cellIndex);
        Piece piece = GameObject.Instantiate(Piece.getPrefabFromType(pieceType)).GetComponent<Piece>();
        piece.teamId = msg.teamId;

        piece.setPosition(cell);

        piece.pieceType = pieceType;
        //if(pieceType > 0) {
        //    ((PieceNumbered)piece).number = pieceType;
        //}

        NetworkServer.Spawn(piece.gameObject);
    }

    public void movePiece(MessageMovePiece msg) {
        Cell previous = this.board.getCellFromIndex(msg.previousCell);
        Cell destination = this.board.getCellFromIndex(msg.destinationCell);
        Piece movingPiece = previous.getCurrentPiece();

        // Check if this is a "attack" move.
        Piece destinationPiece = destination.getCurrentPiece();
        if(destinationPiece != null) {
            // We are attacking!

            // Reveal the piece value to the player who initiated the attack.
            ConnectedPlayer cp = this.board.getConnectedPlayer(movingPiece);

            // Start the attack effect.
            this.board.attack = new Attack(movingPiece, destinationPiece, cp);
        } else {
            // Cell is empty, just move the piece to the cell.
            movingPiece.setDestination(destination);
        }
    }

    /// <summary>
    /// Removes a piece from the board.  This is used in the setup phase.
    /// </summary>
    public void removePiece(MessageRemovePiece msg) {
        Piece piece = this.board.getCellFromIndex(msg.cellIndex).getCurrentPiece();
        if(piece != null) {
            GameObject.Destroy(piece.gameObject);
        } else {
            Debug.Log("Trying to destory a null piece!?!");
        }
    }

    private void registerMsg<T>() where T : AbstractMessageServer, new() {
        NetworkServer.RegisterHandler(new T().getID(), delegate (NetworkMessage netMsg) {
            T msg = netMsg.ReadMessage<T>();
            msg.processMessage(this.board.connectedPlayerFromNetworkConnection(netMsg.conn), this);
        });
    }
}
