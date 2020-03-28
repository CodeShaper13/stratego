using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Starts up the game.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Startup : MonoBehaviour {

    public static bool DEBUG = false;

    private static AudioSource auido;

    private void Awake() {
        Startup.auido = this.GetComponent<AudioSource>();

        Tips.initTips();

        NetworkTransport.Init();
    }

    private void Start() {
        if(Username.retrieveUsername()) {
            ScreenManager.singleton.showScreen(ScreenManager.singleton.screenMainMenu);
        } else {
            ScreenManager.singleton.showScreen(ScreenManager.singleton.screenPickUsernme);
        }
    }

    /// <summary>
    /// Retursn the AudioSource object to use when playing UI clicks.
    /// </summary>
    /// <returns></returns>
    public static void playUiClick() {
        Startup.auido.Play();
    }

    /*
    private void Update() {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.D)) {
            Startup.DEBUG = !Startup.DEBUG;
        }
    }
    */
}