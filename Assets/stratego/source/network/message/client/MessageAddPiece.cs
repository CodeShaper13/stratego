
/// <summary>
/// Sent to the server to add a piece to the game during startup.
/// </summary>
public class MessageAddPiece : AbstractMessageClient {

    public int cellIndex;
    public int pieceId;
    public int teamId;

    public MessageAddPiece() { }

    public MessageAddPiece(Cell cell, EnumPieceType pieceType, Team team) {
        this.cellIndex = cell.cellIndex;
        this.pieceId = (int)pieceType;
        this.teamId = team.getId();
    }

    public override short getID() {
        return 1001;
    }

    public override void processMessage(ConnectedPlayer sender, NetHandlerServer handler) {
        handler.addPiece(this);
    }
}
