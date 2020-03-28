using UnityEngine;
using UnityEngine.UI;

public class UIAttack : UIBase {

    public const float VISIBLE_TIME = 1.5f;

    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private Text textPiecesInvolved;
    [SerializeField]
    private Text textOutcome;

    /// <summary> A timer for how long this UI has been visible. </summary>
    private float timer;

    private Piece attacker;
    private Piece defender;

    public override void onUpdate() {
        this.timer += Time.deltaTime;

        float a = this.curve.Evaluate((this.timer - 0.7f) * VISIBLE_TIME);
        Color c = this.textOutcome.color;
        this.textOutcome.color = new Color(c.r, c.g, c.b);
    }

    public override void onShow() {
        this.timer = 0f;
    }

    public override void onHide() {
        this.hidePieceValue(this.attacker);
        this.hidePieceValue(this.defender);
    }

    /// <summary>
    /// Hides the Piece value if it doesn't belong to the local Player.
    /// </summary>
    private void hidePieceValue(Piece piece) {
        if(piece != null) {
            piece.setOutlineVisible(false);

            bool enemyPiece = piece.getTeam() != Player.localPlayer.getTeam();
            if(enemyPiece) {
                piece.hideValue();
            }
        }
    }

    /// <summary>
    /// Sets the data that this UI needs.
    /// </summary>
    public void setData(Piece attacker, Piece defender, Team winnerTeam) {
        this.attacker = attacker;
        this.defender = defender;

        // Move the window so it doesn't cover the attack
        Vector3 v = Camera.main.WorldToScreenPoint((this.attacker.transform.position + this.defender.transform.position) / 2);
        float x;
        if(v.x >= 0.5f) {
            x = 200;
        } else {
            x = -200;
        }
        this.transform.GetChild(0).localPosition = new Vector3(x, 0, 0);

        // Outline both pieces involved in the attack
        this.attacker.setOutlineVisible(true);
        this.defender.setOutlineVisible(true);

        Team myTeam = Player.localPlayer.getTeam();
        bool isAttacker = this.attacker.getTeam() == myTeam;
        bool isDefender = this.defender.getTeam() == myTeam;
        bool isInvolved = isAttacker || isDefender;

        string letterA = ".";
        string letterD = ".";
        string outcomeMsg;
        if(isAttacker) {
            letterA = this.attacker.getPieceLetter();
            letterD = this.defender.getPieceLetter();
        }
        if(isDefender) {
            letterA = (winnerTeam == myTeam) ? this.attacker.getPieceLetter() : "?";
            letterD = this.defender.getPieceLetter();
        }
        if(!isInvolved) {
            letterA = "?";
            letterD = "?";
            outcomeMsg = winnerTeam.getName() + " wins";
        }

        if(winnerTeam == null) {
            outcomeMsg = "Tie...";
        } else {
            if(isInvolved) {
                if(winnerTeam == myTeam) {
                    outcomeMsg = "Victory!";
                } else {
                    outcomeMsg = "Defeated";
                }
            } else {
                outcomeMsg = winnerTeam.getName() + " wins";
            }
        }

        string s1 = TextColorer.getColoredText(attacker.getTeam(), letterA);
        string s2 = TextColorer.getColoredText(defender.getTeam(), letterD);
        this.textPiecesInvolved.text = s1 + " attacks " + s2;
        this.textOutcome.text = outcomeMsg;
    }

    /// <summary>
    /// Callback for the "OK" button.
    /// </summary>
    public void callback_okButton() {
        this.manager.closeCurrent();
    }
}
