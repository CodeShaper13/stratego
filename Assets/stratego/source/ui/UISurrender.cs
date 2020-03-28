public class UISurrender : UIBase {

    public void callback_surrenderConfirm() {
        Player.localPlayer.sendMessageToServer(new MessageSurrender());
        this.manager.closeCurrent();
    }
}
