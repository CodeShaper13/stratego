public abstract class NetHandlerBase {

    public NetHandlerBase() {
        this.registerHandlers();
    }

    protected abstract void registerHandlers();
}
