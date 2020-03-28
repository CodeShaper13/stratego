using UnityEngine.UI;

public class ScreenPickUsername : ScreenBase {

    public InputField field;
    public Button okButton;

    public override void onUiOpen() {
        base.onUiOpen();

        this.field.text = Username.getUsername();
    }

    public override void onUpdate() {
        this.okButton.interactable = !string.IsNullOrWhiteSpace(this.field.text);
    }

    public override bool showPhotographBackground() {
        return true;
    }

    public void callback_usernameOk() {
        string username = this.field.text;
        Username.setUsername(username);

        this.screenManager.showScreen(this.screenManager.screenMainMenu);
    }
}
