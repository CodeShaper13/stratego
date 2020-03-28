using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetupButton : MonoBehaviour, IPointerClickHandler {

    private Player player;
    private SetupPanel setupUI;
    private int maxCount;
    private EnumPieceType pieceType;

    [SerializeField]
    private Text valueText;
    [SerializeField]
    private Image valueIcon;
    [SerializeField]
    private Text textCount;
    [SerializeField]
    private Image imageBackground;

    public void setValues(Player player, SetupPanel setupUI, EnumPieceType type, int maxCount) {
        this.player = player;
        this.setupUI = setupUI;
        this.pieceType = type;
        this.maxCount = maxCount;

        if(pieceType > 0) {
            this.valueText.text = ((int)pieceType).ToString();
        } else {
            this.valueIcon.enabled = true;
            this.valueIcon.sprite = References.spriteFromPiece((int)pieceType);
        }
    }

    public EnumPieceType getPieceType() {
        return this.pieceType;
    }

    public bool canPlaceMore() {
        return this.getPlacedCount() < this.maxCount;
    }

    /// <summary>
    /// Returns the number of pieces that have been placed.
    /// </summary>
    public int getPlacedCount() {
        int i = 0;
        foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
            if(piece.getTeam() == this.player.getTeam() && piece.pieceType == (int)this.pieceType) {
                i++;
            }
        }
        return i;
    }

    public void updateText() {
        int remainingPieces = this.maxCount - this.getPlacedCount();
        this.textCount.text = remainingPieces.ToString();

        bool noPiecesLeft = remainingPieces <= 0;

        // Set button color
        Team team = this.player.getTeam();
        if(team != null) { // network is slow
            this.imageBackground.color = noPiecesLeft ? Color.gray : References.getMaterialFromTeam(team).color;
        }
    }

    /// <summary>
    /// Sets the scale of the button.  Used when buttons are clicked to make the selected one bigger.
    /// </summary>
    public void setScale(bool isBig) {
        const float f = 1.2f;
        this.transform.localScale = isBig ? new Vector3(f, f, f) : Vector3.one;

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
