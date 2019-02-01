using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetupButton : MonoBehaviour, IPointerClickHandler {

    private Player player;
    private SetupUI setupUI;
    private int maxCount;
    private int pieceType;
    private Text text;
    private string pieceName;

    public void set(Player player, SetupUI setupUI, int type, int maxCount) {
        this.player = player;
        this.setupUI = setupUI;
        this.pieceType = type;
        this.maxCount = maxCount;
        this.text = this.GetComponentInChildren<Text>();

        if(type < 1) {
            this.pieceName = Piece.getPrefabFromType(type).GetComponent<Piece>().getPieceLetter();
        } else {
            this.pieceName = (type).ToString();
        }

        this.updateText();
    }

    public int getPieceType() {
        return this.pieceType;
    }

    public bool canPlaceMore() {
        return this.getPlacedCount() < this.maxCount;
    }

    public int getPlacedCount() {
        int i = 0;
        foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
            if(piece.playerControllerId == this.player.controllingTeamID && piece.pieceType == this.pieceType) {
                i++;
            }
        }
        return i;
    }

    public void updateText() {
        int placedCount = this.getPlacedCount();
        this.text.text = "<b>" + this.pieceName + "</b> x " + placedCount;
        this.text.color = placedCount > 0 ? Color.black : Color.red;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(eventData.button == PointerEventData.InputButton.Left) {
            if(this.setupUI.getSelectedPiece() == this) {
                this.setupUI.setSelectedPiece(null);
            } else {
                this.setupUI.setSelectedPiece(this);
            }
        }
    }
}
