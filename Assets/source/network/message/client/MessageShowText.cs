public class MessageShowText : AbstractMessageClient {

    public string message;
    public float duration;
    public bool inCorner;

    public MessageShowText() { }

    public MessageShowText(string message, float duration = 1f) {
        this.message = message;
        this.duration = duration;
    }

    public MessageShowText showInCorner() {
        this.inCorner = true;
        return this;
    }

    public override short getID() {
        return 2001;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.showText(this);
    }
}