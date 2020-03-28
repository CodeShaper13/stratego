
/// <summary>
/// Sent to the server to tell it to remove a piece during the startup phase.
/// </summary>
public class MessageRemovePiece : AbstractMessageClient {

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
