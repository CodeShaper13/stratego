using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetupButton : MonoBehaviour, IPointerClickHandler {

    private SetupUI setupUI;
    private int count;
    private int pieceType;
    private Text text;
    private string pieceName;

    public void set(SetupUI setupUI, int type, int count) {
        this.setupUI = setupUI;
        this.pieceType = type;
        this.count = count;
        this.text = this.GetComponentInChildren<Text>();

        string s;
        if((int)type < 1) {
            this.pieceName = Piece.getPrefabFromType(type).GetComponent<Piece>().getPieceLetter();
        } else {
            this.pieceName = ((int)type).ToString();
        }

        this.updateText();
    }

    public int getPieceType() {
        return this.pieceType;
    }

    public bool reduceCount() {
        if(this.count <= 0) {
            return false;
        }

        this.count--;
        this.updateText();

        return true;
    }

    public int getCount() {
        return this.count;
    }

    public void increaseCount() {
        this.count++;
        this.updateText();
    }

    public void updateText() {
        this.text.text = "<b>" + this.pieceName + "</b> x " + this.count;
        this.text.color = this.count > 0 ? Color.black : Color.red;
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
