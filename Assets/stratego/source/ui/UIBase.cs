using UnityEngine;

public class UIBase : MonoBehaviour {

    protected UIManager manager;

    private void Awake() {
        this.manager = this.GetComponentInParent<UIManager>();

        this.onAwake();
    }

    private void Update() {
        this.onUpdate();
    }

    public virtual void onAwake() { }

    public virtual void onUpdate() { }

    /// <summary>
    /// Reveals the UI so you can see it.
    /// </summary>
    public virtual void onShow() { }

    /// <summary>
    /// Hides the UI so it can't be seen or interacted with.
    /// </summary>
    public virtual void onHide() { }

    /// <summary>
    /// Called when the escape key is pressed or the back button is pressed (if it exists).
    /// </summary>
    public virtual void onEscapeOrBack() {
        this.manager.closeCurrent();
    }
}