public abstract class AbstractMessageClient : AbstractMessage<NetHandlerServer> {

    public abstract void processMessage(ConnectedPlayer sender, NetHandlerServer handler);
}
