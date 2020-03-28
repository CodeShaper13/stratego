using UnityEngine;

/// <summary>
/// Sent to the server to signal that this player is ready (or now no longer ready) to start the game.
/// </summary>
public class MessageIsReady : AbstractMessageClient {

    [SerializeField]
    public bool ready;

    public MessageIsReady() { }

    public MessageIsReady(bool isReady) {
        this.ready = isReady;
    }

    public override short getID() {
        return 1005;
    }

    public override void processMessage(ConnectedPlayer sender, NetHandlerServer handler) {
        handler.setIsReady(this, sender);
    }
}
