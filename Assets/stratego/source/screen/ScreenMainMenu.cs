using UnityEngine;
using UnityEngine.UI;

public class ScreenMainMenu : ScreenBase {

    [SerializeField]
    private Text usernameText;
    [SerializeField]
    private Button buttonResumeGame;

    public override void onUiOpen() {
        base.onUiOpen();

        this.buttonResumeGame.interactable = GameSaver.doesSaveExists();

        this.usernameText.text = Username.getUsername();
    }

    public override bool showPhotographBackground() {
        return true;
    }

    public void callback_createGame() {
        if(GameSaver.doesSaveExists()) {
            this.screenManager.showScreen(this.screenManager.screenSaveWarning);
        } else {
            this.screenManager.showScreen(this.screenManager.screenNewGame);
        }
    }

    public void callback_resumeGame() {
        if(GameSaver.doesSaveExists()) { // Double check
            this.screenManager.showScreen(null);
            GameSaver.loadGame(this.getNetworkManager());
        }
        else {
            this.buttonResumeGame.enabled = false;
        }
    }

    public void callback_joinGame() {
        this.screenManager.showScreen(this.screenManager.screenJoinGame);
    }

    public void callback_exitGame() {
        Application.Quit();
    }
}
