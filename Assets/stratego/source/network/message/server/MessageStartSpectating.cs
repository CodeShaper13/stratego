/// <summary>
/// A message that will make the Player start spectating the game.
/// </summary>
public class MessageStartSpectating : AbstractMessageServer {

    public MessageStartSpectating() { }

    public override short getID() {
        return 2004;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.startSpectating(this);
    }
}
