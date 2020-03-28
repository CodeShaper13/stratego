using UnityEngine;

public class MessageGameBegin : AbstractMessageServer {

    public MessageGameBegin() { }

    public override short getID() {
        return 2007;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.onGameBegin(this);
    }
}
