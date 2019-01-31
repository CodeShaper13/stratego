public class MessageChangTeurn : AbstractMessageClient {

    public int i;
    public bool isMyTurn;

    public MessageChangTeurn() { }

    public MessageChangTeurn(int i, bool flag) {
        this.i = i;
        this.isMyTurn = flag;
    }

    public override short getID() {
        return 2002;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.tellTurn(this);
    }
}
