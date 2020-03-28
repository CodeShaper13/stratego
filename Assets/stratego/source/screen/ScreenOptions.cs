using UnityEngine;
using UnityEngine.UI;

public class ScreenOptions : ScreenBase {

    [SerializeField]
    private Toggle fullscreen;
    [SerializeField]
    private Slider volume;

    public override void onUiOpen() {
        base.onUiOpen();        

        this.volume.value = AudioListener.volume;
        this.fullscreen.isOn = Screen.fullScreen;
    }

    public override bool showPhotographBackground() {
        return true;
    }

    public void callback_toggleFullscreen() {
        Screen.fullScreen = !this.fullscreen.isOn;
        Startup.playUiClick();
        PlayerPrefs.SetInt("fullscreen", Screen.fullScreen ? 1 : 0);
    }

    public void callback_volumeSlider() {
        AudioListener.volume = this.volume.value;
        Startup.playUiClick();
        PlayerPrefs.SetFloat("volume", AudioListener.volume);
    }

    public override void onEscape() {
        if(Player.localPlayer == null) {
            this.screenManager.showScreen(this.screenManager.screenMainMenu);
        } else {
            this.screenManager.showScreen(null);
            Player.localPlayer.uiManager.showUi(PopupIds.ESCAPE);
        }
    }
}
