using UnityEngine;

public class Attack {

    private Board board;
    private Piece attacker;
    private Piece defender;
    private Cell defenderCell;
    /// <summary> The player who triggered the attack. </summary>
    private ConnectedPlayer initiator;

    private float timer;

    private EnumAttackOutcome outcome;

    public Attack(Board board, Piece attacker, Piece defender, ConnectedPlayer initiator) {
        this.board = board;
        this.attacker = attacker;
        this.defender = defender;
        this.defenderCell = this.defender.getCell();
        this.initiator = initiator;

        // Figure out who won.
        this.outcome = this.attacker.getAttackOutcome(this.defender);

        this.initiator.sendMessage(new MessageSetPieceVisible(this.defender, true));

        // Show the attack ui to all players
        Team winner;
        if(this.outcome == EnumAttackOutcome.WIN) {
            winner = this.attacker.getTeam();
        } else if(this.outcome == EnumAttackOutcome.LOSE) {
            winner = this.defender.getTeam();
        } else {
            winner = null;
        }

        // Move the attacker at the defender
        this.attacker.setDestination((this.attacker.transform.position + this.defender.transform.position) / 2);

        CustomNetworkManager.getSingleton().connectedPlayers.sendMessage(new MessageShowAttackUI(this.attacker, this.defender, winner));
    }

    /// <summary>
    /// Updates the state of the attack.  If true is returned, the attack is over
    /// and it should be the next Player's turn.
    /// </summary>
    public bool update() {
        this.timer += Time.deltaTime;

        if(this.timer > UIAttack.VISIBLE_TIME) {


            // Find out the outcome of the attack.
            if(outcome == EnumAttackOutcome.TIE) {
                this.board.removePiece(this.attacker);
                this.board.removePiece(this.defender);
            }
            else if(outcome == EnumAttackOutcome.LOSE) {
                this.board.removePiece(this.attacker);
            }
            else if(outcome == EnumAttackOutcome.WIN) {
                this.board.removePiece(this.defender);
                // Move the attacker to it's new cell.
                
                this.attacker.setDestination(this.defenderCell.getPos());

                if(this.defender.pieceType == PieceType.FLAG) {
                    this.board.eliminatePlayer(CustomNetworkManager.getSingleton().connectedPlayers.getFromPiece(this.defender), EliminationReson.LOSE_FLAG);
                }
            }

            return true;
        }
        return false;
    }
}
