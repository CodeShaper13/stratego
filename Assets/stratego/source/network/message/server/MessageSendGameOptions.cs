using UnityEngine.Networking;

public class MessageSendGameOptions : AbstractMessageServer {

    private GameOptions options;

    public MessageSendGameOptions() { }

    public MessageSendGameOptions(GameOptions options) {
        this.options = options;
    }

    public override void Deserialize(NetworkReader reader) {
        this.options = new GameOptions(reader);
    }

    public override void Serialize(NetworkWriter writer) {
        this.options.serializeGameOptions(writer);
    }

    public override short getID() {
        return 2009;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.setGameOptions(this);
    }

    public GameOptions getOptions() {
        return this.options;
    }
}
