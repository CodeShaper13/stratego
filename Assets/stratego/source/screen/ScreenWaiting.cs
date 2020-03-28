using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScreenWaiting : ScreenBase {

    [SerializeField]
    private Text textTip;
    [SerializeField]
    private Text playersRemainingText;

    public override void onAwake() {
        this.callback_nextTip();
    }

    public override void onEscape() {
        CustomNetworkManager manager = CustomNetworkManager.getSingleton();
        if(NetworkServer.active) {
            manager.StopHost();
        } else {
            manager.StopClient();
        }
    }

    public void callback_nextTip() {
        this.textTip.text = Tips.instance.getNextTip();
    }

    public void updatePlayersRemainingText(MessageShowNeededPlayers msg) {
        int playersNeeded = msg.players;
        string s = "Waiting for " + playersNeeded + " more Player" + (playersNeeded > 1 ? "s" : string.Empty);
        this.playersRemainingText.text = s;
    }
}
