using UnityEngine;
using System.Collections;

public class MessageStartSpectating : AbstractMessageClient {

    public MessageStartSpectating() { }

    public override short getID() {
        return 2004;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.startSpectating(this);
    }
}
