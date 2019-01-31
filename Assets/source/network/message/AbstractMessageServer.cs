public abstract class AbstractMessageServer : AbstractMessage<NetHandlerServer> {

    public abstract void processMessage(ConnectedPlayer sender, NetHandlerServer handler);
}
