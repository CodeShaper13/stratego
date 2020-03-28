using UnityEngine;

/// <summary>
/// Send to the client to tell them that a piece is moving
/// </summary>
public class MessageChangePieceLocation : AbstractMessageServer {

    public Vector3 moveTo;
    public GameObject piece;

    public MessageChangePieceLocation() { }

    public MessageChangePieceLocation(Piece piece, Cell destination) {
        this.piece = piece.gameObject;
        this.moveTo = destination.getPos();
    }

    public override short getID() {
        return 2010;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.changePieceLocation(this);
    }
}
