using UnityEngine;

public abstract class ScreenBase : MonoBehaviour {

    [HideInInspector]
    public ScreenManager screenManager;

    private void Awake() {
        this.onAwake();
    }

    private void Update() {
        this.onUpdate();

        // Go back to the main menu if escape is pressed.
        if(Input.GetKeyDown(KeyCode.Escape)) {
            this.onEscape();
        }
    }

    public virtual void onAwake() { }

    public virtual void onUpdate() { }

    public virtual void onUiOpen() { }

    public virtual void onUiClose() { }

    public virtual void onEscape() { }

    public virtual bool showPhotographBackground() {
        return false;
    }

    public CustomNetworkManager getNetworkManager() {
        return CustomNetworkManager.getSingleton();
    }

    public void callback_changeScreen(ScreenBase newUi) {
        this.screenManager.showScreen(newUi);
    }

    protected void playBtnSound() {
        Startup.playUiClick();
    }
}