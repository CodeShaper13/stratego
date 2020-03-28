using UnityEngine;
using UnityEngine.Networking;

public class NetHandlerServer : NetHandlerBase {

    private Board board;

    public NetHandlerServer(Board board) : base() {
        this.board = board;
    }

    protected override void registerHandlers() {
        this.registerMsg<MessageAddPiece>();
        this.registerMsg<MessageMovePiece>();
        this.registerMsg<MessageRemovePiece>();
        this.registerMsg<MessageSurrender>();
        this.registerMsg<MessageIsReady>();
    }

    public void setIsReady(MessageIsReady msg, ConnectedPlayer sender) {
        sender.setReady(msg.ready);
    }

    public void addPiece(MessageAddPiece msg) {
        if(this.board.gameState == GameStates.SETUP) {
            //TODO make sure the player doesn't have too many of that pieces.
            if(this.board.getCellFromIndex(msg.cellIndex).getCurrentPiece() == null) {
                this.board.addPiece(msg.pieceId, msg.cellIndex, msg.teamId);
            }
        }
    }

    public void movePiece(MessageMovePiece msg, ConnectedPlayer sender) {
        Cell previous = this.board.getCellFromIndex(msg.previousCell);
        Cell destination = this.board.getCellFromIndex(msg.destinationCell);
        Piece movingPiece = previous.getCurrentPiece();

        // Check if this is a "attack" move.
        Piece destinationPiece = destination.getCurrentPiece();
        if(destinationPiece != null) {
            // Start the attack effect.
            this.board.attack = new Attack(this.board, movingPiece, destinationPiece, sender);
        } else {
            // Cell is empty, just move the piece to the cell.
            movingPiece.setDestination(destination.getPos());
        }
        this.board.nextTurn();
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

    public void playerSurrender(MessageSurrender msg, ConnectedPlayer sender) {
        this.board.eliminatePlayer(sender, EliminationReson.SURRENDER);
    }

    private void registerMsg<T>() where T : AbstractMessageClient, new() {
        NetworkServer.RegisterHandler(
            new T().getID(), delegate (NetworkMessage netMsg) {
                T msg = netMsg.ReadMessage<T>();
                ConnectedPlayer cp = CustomNetworkManager.getSingleton().connectedPlayers.getFromNetworkConnection(netMsg.conn);
                msg.processMessage(cp, this);
        });
    }
}
