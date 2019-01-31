public abstract class AbstractMessageClient : AbstractMessage<NetHandlerClient> {

    public abstract void processMessage(NetHandlerClient handler);
}
