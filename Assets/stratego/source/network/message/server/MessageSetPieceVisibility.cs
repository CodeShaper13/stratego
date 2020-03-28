using UnityEngine;

public class MessageSetPieceVisible : AbstractMessageServer {

    public GameObject piece;
    public bool visibleValue;

    public MessageSetPieceVisible() { }

    public MessageSetPieceVisible(Piece piece, bool visible) {
        this.piece = piece.gameObject;
        this.visibleValue = visible;
    }

    public override short getID() {
        return 2003;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.revealPieceValue(this);
    }
}
