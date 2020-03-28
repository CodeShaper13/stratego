using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSettings : UIBase {

    [SerializeField]
    private Text optionListText;

    public override void onShow() {
        GameOptions options = Player.localPlayer.gameOptions;
        StringBuilder sb = new StringBuilder(options.all.Count);
        foreach(GameOptions.IOption option in options.all) {
            sb.Append(option.prettyPrint() + "\n");
        }
        this.optionListText.text = sb.ToString();
    }

    public void callback_backButton() {
        this.manager.closeCurrent();
    }
}
