using UnityEngine.Networking;

public class NetHandlerClient : NetHandlerBase {

    private Player player {
        get {
            return Player.localPlayer;
        }
    }

    protected override void registerHandlers() {
        this.registerMsg<MessageChangeTurn>();
        this.registerMsg<MessageGameBegin>();
        this.registerMsg<MessageSetPieceVisible>();
        this.registerMsg<MessageSetTeam>();
        this.registerMsg<MessageShowAttackUI>();
        this.registerMsg<MessageShowNeededPlayers>();
        this.registerMsg<MessageShowEliminationMsg>();
        this.registerMsg<MessageStartSpectating>();
        this.registerMsg<MessageSendGameOptions>();
        this.registerMsg<MessageChangePieceLocation>();
        this.registerMsg<MessageMissingPlayers>();
    }

    public void toggleMissingPlayerScreen(MessageMissingPlayers msg) {
        this.player.allertMissingPlayers(msg.missingPlayer);
    }

    public void startSpectating(MessageStartSpectating messageStartSpectating) {
        this.player.setSpectating();
    }

    public void tellTurn(MessageChangeTurn msg) {
        string whosTurnText;
        if(msg.teamIdOfTurn == this.player.getTeam().getId()) {
            whosTurnText = "Your Turn";
        } else {
            whosTurnText = Team.teamFromId(msg.teamIdOfTurn).getName() + "'s Turn";
        }

        this.player.setTheirTurn(msg.teamIdOfTurn == this.player.getTeam().getId(), whosTurnText);
    }

    public void setWaitingScreenText(MessageShowNeededPlayers msg) {
        ScreenManager.singleton.screenWaiting.updatePlayersRemainingText(msg);
    }

    public void revealPieceValue(MessageSetPieceVisible msg) {
        Piece piece = msg.piece.GetComponent<Piece>();
        if(msg.visibleValue) {
            piece.showValue();
        } else {
            piece.hideValue();
        }
    }

    public void showAttackUI(MessageShowAttackUI msg) {
        this.player.uiManager.showUi(PopupIds.ATTACK);
        this.player.uiManager.attack.setData(
            msg.attackerPiece.GetComponent<Piece>(),
            msg.defenderPiece.GetComponent<Piece>(),
            Team.teamFromId(msg.winnerId));
    }

    public void showUI(MessageShowEliminationMsg msg) {
        this.player.uiManager.showUi(PopupIds.ELIMINATION, msg.s1, msg.s2);
    }

    public void onGameBegin(MessageGameBegin msg) {
        this.player.startGame();
    }

    public void setTeam(MessageSetTeam msg) {
        this.player.setTeam(Team.teamFromId(msg.teamId), msg.cameraPosition, msg.cameraRotation);
    }

    public void setGameOptions(MessageSendGameOptions msg) {
        this.player.gameOptions = msg.getOptions();
    }

    // Msg is never sent by the server yet!
    public void changePieceLocation(MessageChangePieceLocation msg) {
        msg.piece.GetComponent<Piece>().setDestination(msg.moveTo);
    }

    private void registerMsg<T>() where T : AbstractMessageServer, new() {
        NetworkClient client = NetworkManager.singleton.client;
        client.RegisterHandler(
            new T().getID(), delegate (NetworkMessage netMsg) {
                T msg = netMsg.ReadMessage<T>();
                msg.processMessage(this);
            });
    }
}
