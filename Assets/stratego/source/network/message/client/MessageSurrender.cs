/// <summary>
/// Tells the server that this player is surrendering.
/// </summary>
public class MessageSurrender : AbstractMessageClient {

    public MessageSurrender() { }

    public override short getID() {
        return 1004;
    }

    public override void processMessage(ConnectedPlayer sender, NetHandlerServer handler) {
        handler.playerSurrender(this, sender);
    }
}
