public class MessageShowEliminationMsg : AbstractMessageServer {

    public string s1;
    public string s2;

    public MessageShowEliminationMsg() { }

    public MessageShowEliminationMsg(string s1, string s2) {
        this.s1 = s1;
        this.s2 = s2;
    }

    public override short getID() {
        return 2006;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.showUI(this);
    }
}
