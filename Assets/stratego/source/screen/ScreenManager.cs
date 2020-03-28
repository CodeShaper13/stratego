using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour {

    public static ScreenManager singleton;

    public ScreenBase screenPickUsernme;
    public ScreenBase screenMainMenu;
    public ScreenBase screenNewGame;
    public ScreenBase screenJoinGame;
    public ScreenBase screenOptions;
    public ScreenWaiting screenWaiting;
    public ScreenInfo screenInfo;
    public ScreenBase screenSaveWarning;

    public Image background;

    private ScreenBase currentlyVisibleScreen;

    public void Awake() {
        ScreenManager.singleton = this;

        // Hide all of the UIs, as some might be open because of dev.
        this.func(this.screenPickUsernme);
        this.func(this.screenMainMenu);
        this.func(this.screenNewGame);
        this.func(this.screenJoinGame);
        this.func(this.screenOptions);
        this.func(this.screenWaiting);
        this.func(this.screenInfo);
        this.func(this.screenSaveWarning);

        this.background.enabled = false;
    }

    public ScreenBase getCurrentlyOpenScreen() {
        return this.currentlyVisibleScreen;
    }

    /// <summary>
    /// Shows the passed screen.  Pass null to hide the current screen.
    /// </summary>
    public void showScreen(ScreenBase newScreen) {
        if(this.currentlyVisibleScreen != null) {
            this.currentlyVisibleScreen.gameObject.SetActive(false);
            this.currentlyVisibleScreen.onUiClose();
            this.currentlyVisibleScreen = null;
        }

        if(newScreen != null) {
            this.currentlyVisibleScreen = newScreen;
            this.currentlyVisibleScreen.gameObject.SetActive(true);
            this.currentlyVisibleScreen.onUiOpen();
            this.background.enabled = this.currentlyVisibleScreen.showPhotographBackground();
        } else {
            this.background.enabled = false;
        }
    }

    private void func(ScreenBase screen) {
        screen.gameObject.SetActive(false);
        screen.screenManager = this;
    }
}
