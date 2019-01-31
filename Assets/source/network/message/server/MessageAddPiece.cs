public class MessageAddPiece : AbstractMessageServer {

    public int cellIndex;
    public int pieceId;
    public int teamId;

    public MessageAddPiece() { }

    public MessageAddPiece(int cellIndex, int pieceId, int teamId) {
        this.cellIndex = cellIndex;
        this.pieceId = pieceId;
        this.teamId = teamId;
    }

    public override short getID() {
        return 1001;
    }

    public override void processMessage(ConnectedPlayer sender, NetHandlerServer handler) {
        handler.addPiece(this);
    }
}
