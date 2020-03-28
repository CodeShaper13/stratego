using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIPause : UIBase {

    [SerializeField]
    private Button saveAndExitButton;

    private Text text;

    public override void onAwake() {
        this.text = this.saveAndExitButton.GetComponentInChildren<Text>();
    }

    public override void onShow() {
        this.text.text = NetworkServer.active ? "Exit" : "Disconnect";
    }

    public void callback_resume() {
        this.manager.closeCurrent();
    }

    public void callback_options() {
        this.manager.closeCurrent();
        ScreenManager.singleton.showScreen(ScreenManager.singleton.screenOptions);
    }

    public void callback_saveAndExit() {
        CustomNetworkManager manager = CustomNetworkManager.getSingleton();

        this.manager.closeCurrent();

        if(NetworkServer.active) {
            GameSaver.saveGame();

            manager.StopHost();
            Player.localPlayer.uiManager.closeCurrent();
            ScreenManager.singleton.showScreen(ScreenManager.singleton.screenMainMenu);
        }
        else {
            manager.StopClient();
            ScreenManager.singleton.showScreen(ScreenManager.singleton.screenMainMenu);
        }
    }
}
