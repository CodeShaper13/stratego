public class MessageMissingPlayers : AbstractMessageServer {

    public bool missingPlayer;

    public MessageMissingPlayers() { }

    public MessageMissingPlayers(bool missingPlayer) {
        this.missingPlayer = missingPlayer;
    }

    public override short getID() {
        return 2011;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.toggleMissingPlayerScreen(this);
    }
}
