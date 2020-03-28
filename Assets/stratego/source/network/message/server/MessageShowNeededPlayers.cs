public class MessageShowNeededPlayers : AbstractMessageServer {

    public int players;

    public MessageShowNeededPlayers() { }

    public MessageShowNeededPlayers(int players) {
        this.players = players;
    }

    public override short getID() {
        return 2001;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.setWaitingScreenText(this);
    }
}