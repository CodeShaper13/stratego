using UnityEngine;

public class UIManager : MonoBehaviour { 

    public UIAttack attack;
    public UIGameSettings gameSettings;
    public UIElimination lose;
    public UISurrender surrenderConfirm;
    public UIPause escape;

    private UIBase currentUi;

    /// <summary>
    /// Closes the current UI and calls the UiBase#onHide() method for cleanup.
    /// </summary>
    public void closeCurrent() {
        if(this.currentUi != null) {
            this.currentUi.gameObject.SetActive(false);
            this.currentUi.onHide();
            this.currentUi = null;
        }
    }

    /// <summary>
    /// Opens the UI with the passed UiId and returns it.
    /// </summary>
    public UIBase showUi(int uiId, params string[] args) {
        this.closeCurrent();

        switch(uiId) {
            case PopupIds.ATTACK:
                return this.open(this.attack);
            case PopupIds.GAME_SETTINGS:
                return this.open(this.gameSettings);
            case PopupIds.ELIMINATION:
                this.open(this.lose);
                this.lose.setReasonText(args[0]);
                this.lose.setSmallText(args[1]);
                return this.lose;
            case PopupIds.SURRENDER_CONFIRM:
                return this.open(this.surrenderConfirm);
            case PopupIds.ESCAPE:
                return this.open(this.escape);
        }

        return null; // Should never happen.
    }

    private UIBase open(UIBase ui) {
        this.currentUi = ui;
        ui.gameObject.SetActive(true);
        ui.onShow();
        return ui;
    }

    /// <summary>
    /// May be null.
    /// </summary>
    public UIBase getCurrentUi() {
        return this.currentUi;
    }
}
