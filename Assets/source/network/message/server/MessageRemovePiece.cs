public class MessageRemovePiece : AbstractMessageServer {

    public int cellIndex;

    public MessageRemovePiece() { }

    public MessageRemovePiece(int cellIndex) {
        this.cellIndex = cellIndex;
    }

    public override short getID() {
        return 1002;
    }

    public override void processMessage(ConnectedPlayer sender, NetHandlerServer handler) {
        handler.removePiece(this);
    }
}
