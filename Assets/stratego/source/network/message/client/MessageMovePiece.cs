public class MessageMovePiece : AbstractMessageClient {

    public int previousCell;
    public int destinationCell;
    
    public MessageMovePiece() { }

    public MessageMovePiece(Cell previous, Cell destination) {
        this.previousCell = previous.cellIndex;
        this.destinationCell = destination.cellIndex;
    }

    public override short getID() {
        return 1003;
    }

    public override void processMessage(ConnectedPlayer sender, NetHandlerServer handler) {
        handler.movePiece(this, sender);
    }
}
