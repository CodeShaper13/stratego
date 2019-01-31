using System;
using UnityEngine;

public class MessageSetPieceVisible : AbstractMessageClient {

    public GameObject piece;
    public bool visibleValue;

    public MessageSetPieceVisible() { }

    public MessageSetPieceVisible(Piece piece, bool visibleValue) {
        this.piece = piece.gameObject;
        this.visibleValue = visibleValue;
    }

    public override short getID() {
        return 2003;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.revealPieceValue(this);
    }
}
