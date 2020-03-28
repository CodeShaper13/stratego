using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupPanel : MonoBehaviour {

    [SerializeField]
    private Canvas canvas = null;
    [SerializeField]
    private GameObject prefab = null;
    [SerializeField]
    private Text remainingPiecesText = null;
    [SerializeField]
    private Toggle readyToggle = null;
    [SerializeField]
    private Player player = null;

    /// <summary> The currently selected piece button.  May be null. </summary>
    private SetupButton selectedPiece;
    /// <summary> A list of references to all the piece buttons. </summary>
    private List<SetupButton> allButtons;
    /// <summary> The total number of pieces the player has at the start of the game. </summary>
    private int totalPieceCount;
    private readonly KeyCode[] SHORTCUT_KEYS = new KeyCode[] {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.S,
        KeyCode.B,
        KeyCode.F
    };

    private void Update() {
        if(!this.GetComponent<Canvas>().enabled) {
            if(this.player.getTeam() != null && this.player.gameOptions != null) {
                // All of the required data is here, now we can show the menu!
                this.GetComponent<Canvas>().enabled = true;
                this.createPieceButtons();
            } else {
                return;
            }
        }

        // Update the piece buttons.
        foreach(SetupButton btn in this.allButtons) {
            btn.updateText();
        }

        Piece[] allPieces = GameObject.FindObjectsOfType<Piece>();

        // Find out how many pieces have been placed.
        int i = 0;
        foreach(Piece piece in allPieces) {
            if(piece.getTeam() == this.player.getTeam()) {
                i++;
            }
        }

        // Figure out how many pieces still need to be places.
        int remainingPieces = (this.totalPieceCount - i);

        // Update the "Pieces Remaining" text.
        this.remainingPiecesText.text = "Pieces Remaining: " + remainingPieces.ToString();

        // Enable the ready toggle if all pices are placed.
        if(remainingPieces <= 0) {
            this.readyToggle.interactable = true;
        } else {
            this.readyToggle.interactable = false;
            this.readyToggle.isOn = false;
        }

        // Shortcut keys to selected a piece type.
        for(int j = 0; j < this.SHORTCUT_KEYS.Length; j++) {
            if(Input.GetKeyDown(this.SHORTCUT_KEYS[j])) {
                this.setSelectedPiece(this.allButtons[j]);
            }
        }

        // Shortcut key to clear all pieces.
        if(Input.GetKeyDown(KeyCode.C)) {
            foreach(Piece piece in allPieces) {
                if(piece.getTeam() == this.player.getTeam()) {
                    this.removePiece(piece);
                }
            }
        }

        // Update outline effects
        this.func();
    }

    /// <summary>
    /// Creates all of the UI buttons on the side of the screen.
    /// </summary>
    public void createPieceButtons() {
        this.allButtons = new List<SetupButton>();

        // Create the piece buttons.
        GameOptions options = this.player.gameOptions;
        Vector2 btnPos = new Vector2(0, 0);
        this.createBtn(player, ref btnPos, EnumPieceType.ONE, options.oneCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.TWO, options.twoCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.THREE, options.threeCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.FOUR, options.fourCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.FIVE, options.fiveCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.SIX, options.sixCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.SEVEN, options.sevenCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.EIGHT, options.eightCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.NINE, options.nineCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.SPY, options.spyCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.BOMB, options.bombCount.get());
        this.createBtn(player, ref btnPos, EnumPieceType.FLAG, 1);
    }

    /// <summary>
    /// Creates one of the piece buttons on the side of the screen.
    /// </summary>
    private void createBtn(Player player, ref Vector2 pos, EnumPieceType pieceType, int maxPieceCount) {
        const float SPACING = 50;

        this.totalPieceCount += maxPieceCount;

        GameObject obj = GameObject.Instantiate(
            this.prefab,
            this.canvas.transform);
        obj.transform.localPosition = new Vector3(40 + (pos.x * SPACING), 40 + (pos.y * SPACING));
        SetupButton btn = obj.GetComponent<SetupButton>();
        btn.setValues(player, this, pieceType, maxPieceCount);

        pos.y++;
        if(pos.y >= 6) {
            pos.y = 0;
            pos.x = 1;
        }

        this.allButtons.Add(btn);
    }

    public void removePiece(Piece hitPiece) {
        // TODO piece dupe bug
        //hitPiece.gameObject.SetActive(false); // Hide it, just in case the server is slow in receiving the message.
        this.player.sendMessageToServer(new MessageRemovePiece(hitPiece.getCell().cellIndex));
    }

    public SetupButton getSelectedPiece() {
        return this.selectedPiece;
    }

    public void setSelectedPiece(SetupButton btn) {
        // Reset the scale of the previously selected piece button.
        if(this.selectedPiece != null) {
            this.selectedPiece.setScale(false);
        }

        this.selectedPiece = btn;

        if(this.selectedPiece != null) {
            this.selectedPiece.setScale(true);
        }
    }

    public void func() {
        int type = this.selectedPiece == null ? 0 : (int)this.selectedPiece.getPieceType();
        foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
            if(piece.getTeam() == Player.localPlayer.getTeam()) {
                piece.setOutlineVisible(piece.pieceType == type);
            }
        }
    }

    /// <summary>
    /// Called whenever the ready toggle button is clicked. 
    /// </summary>
    public void callback_toggleClick() {
        Startup.playUiClick();
        this.player.sendMessageToServer(new MessageIsReady(this.readyToggle.isOn));
    }
}
