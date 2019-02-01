public class MessageChangeTurn : AbstractMessageClient {

    public int i;
    public bool isMyTurn;

    public MessageChangeTurn() { }

    public MessageChangeTurn(int i, bool isTurn) {
        this.i = i;
        this.isMyTurn = isTurn;
    }

    public override short getID() {
        return 2002;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.tellTurn(this);
    }
}
