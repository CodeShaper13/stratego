/// <summary>
/// Base class for message that should go to a client.
/// </summary>
public abstract class AbstractMessageServer : AbstractMessage<NetHandlerClient> {

    public abstract void processMessage(NetHandlerClient handler);
}
