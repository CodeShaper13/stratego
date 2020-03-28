using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public static Player localPlayer;

    [SerializeField]
    private AnnouncementText announcementText;
    [SerializeField]
    private Text cornerText;
    [SerializeField]
    private Text spectatingText;
    [SerializeField]
    private GameObject clientCanvases;
    [SerializeField]
    private SetupPanel setupPanel;
    /// <summary> Inspector set reference to the canvas that contains the message stating that players are missing. </summary>
    [SerializeField]
    private Canvas missingPlayerCanvas;
    [SerializeField]
    private Button buttonSurrenderFlag;
    [SerializeField]
    private Button buttonGear;
    
    public UIManager uiManager;
    private ScreenManager screenManager;

    private bool isMyTurn;
    private bool movedThisTurn;
    private bool missingPlayers;
    private bool spectating;
    private Team team;

    public Board board;
    public GameOptions gameOptions;

    /// <summary> The currently selected piece, may be null. </summary>
    private Piece selectedPiece;

    public override void OnStartClient() {
        this.board = GameObject.FindObjectOfType<Board>();
    }

    public override void OnStartLocalPlayer() {
        Player.localPlayer = this;

        this.clientCanvases.SetActive(true);

        // Make the camera look at the board in a pretty way
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(0.55f, 6.23f, -6f);
        cam.transform.rotation = Quaternion.Euler(54, -11, -2.2f);

        this.uiManager = GameObject.FindObjectOfType<UIManager>();
        this.screenManager = ScreenManager.singleton;

        if(this.board.gameState == GameStates.WAITING) {
            this.screenManager.showScreen(this.screenManager.screenWaiting);
        } else {
            this.screenManager.showScreen(null);
        }
    }

    private void Update() {
        if(this.isLocalPlayer) {
            this.setupPanel.gameObject.SetActive(this.board.gameState == GameStates.SETUP && !this.spectating);

            // Open the Escape screen if the "Esc" key is pressed.
            if(Input.GetKeyDown(KeyCode.Escape)) {
                if(this.screenManager.getCurrentlyOpenScreen() == null) {
                    this.uiManager.showUi(PopupIds.ESCAPE);
                }
            }

            if(!this.missingPlayers) {
                if(!this.isSpectating()) {
                    bool leftBtn = Input.GetMouseButtonDown(0);
                    bool rightBtn = Input.GetMouseButtonDown(1);

                    if(this.board.gameState == GameStates.SETUP) {
                        this.setupUpdate(leftBtn, rightBtn);
                    }
                    else if(this.board.gameState == GameStates.PLAYING) {
                        this.playingUpdate(leftBtn, rightBtn);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update logic for durring the Setup phase.
    /// </summary>
    private void setupUpdate(bool leftBtn, bool rightBtn) {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity)) {
            Cell hitCell = hit.transform.GetComponent<Cell>();
            Piece hitPiece = hit.transform.GetComponent<Piece>();
            if(leftBtn) {
                // Remove a piece if the Player clicked one.
                if(hitPiece != null && hitPiece.teamId == this.team.getId()) {
                    this.setupPanel.removePiece(hitPiece);
                }

                // Add a piece if the Player clicked an empty square.
                if(hitCell != null) {
                    if(hitCell.ownedByLocalPlayer && hitCell.getCurrentPiece() == null) {
                        SetupButton setupBtn = this.setupPanel.getSelectedPiece();
                        if(setupBtn != null && setupBtn.canPlaceMore()) {
                            setupBtn.updateText();
                            this.sendMessageToServer(new MessageAddPiece(hitCell, setupBtn.getPieceType(), this.team));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update logic for durring the Playing phase.
    /// </summary>
    private void playingUpdate(bool leftBtn, bool rightBtn) {
        if(this.isMyTurn && !this.movedThisTurn) {
            // Handle input, it is this players turn.
            if(leftBtn) {
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity)) {
                    Piece hitPiece = hit.transform.GetComponent<Piece>();
                    Cell hitCell = hit.transform.GetComponent<Cell>();

                    if(hitPiece != null) {
                        if(hitPiece.teamId == this.team.getId() && hitPiece.aloudToMove()) {
                            // Clicked a piece on our side
                            if(this.selectedPiece == hitPiece) {
                                // Deselect piece, we clicked the selected one..
                                this.deselSelected();
                            }
                            else {
                                // Select the clicked piece.
                                this.deselSelected();
                                this.selectedPiece = hitPiece;
                                this.selectedPiece.setOutlineVisible(true);
                            }
                        }
                        else {
                            // Clicked an enemy piece.
                            if(this.selectedPiece != null) {
                                if(this.selectedPiece.getCell().isAdjacent(hitPiece.getCell())) {
                                    this.moveSelectedPiece(hitPiece.getCell());
                                }
                            }

                        }
                    }

                    if(hitCell != null && !hitCell.isWater && this.selectedPiece != null) {
                        if(hitCell.isAdjacent(this.selectedPiece.getCell())) {
                            if(hitCell.getCurrentPiece() == null) {
                                // move piece to it.
                                this.moveSelectedPiece(hitCell);
                            }
                            else if(hitCell.getCurrentPiece().teamId != this.team.getId()) {
                                // attack it.
                                this.moveSelectedPiece(hitCell);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Moves the selected piece to the passed cell.
    /// </summary>
    private void moveSelectedPiece(Cell targetCell) {
        this.sendMessageToServer(new MessageMovePiece(this.selectedPiece.getCell(), targetCell));
        this.movedThisTurn = true;
        this.deselSelected();
    }

    /// <summary>
    /// Deselects the selected piece.
    /// </summary>
    private void deselSelected() {
        if(this.selectedPiece != null) {
            this.selectedPiece.setOutlineVisible(false);
            this.selectedPiece = null;
        }
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    public void sendMessageToServer(AbstractMessageClient message) {
        base.connectionToServer.Send(message.getID(), message);
    }

    /// <summary>
    /// Sets the Player as spectating.  This will also reveal all pieces.
    /// </summary>
    public void setSpectating() {
        if(!this.spectating) {
            this.allertMissingPlayers(false);

            this.spectating = true;

            this.func();

            // Reveal the values of all of the pieces.
            foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
                piece.showValue();
            }

            // Hide the surrender flag.
            this.buttonSurrenderFlag.gameObject.SetActive(false);

            this.spectatingText.text = "Spectating";
            this.cornerText.text = string.Empty;
        } else {
            Debug.LogWarning("Player#setSpectating() should only be called once!");
        }
    }

    public bool isSpectating() {
        return this.spectating;
    }

    public void setTheirTurn(bool theirTurn, string whosTurnText) {
        this.isMyTurn = theirTurn;
        this.cornerText.text = whosTurnText;

        if(!theirTurn) {
            // Rest when it's someone elses's turn.
            this.movedThisTurn = false;
        }
    }

    /// <summary>
    /// Returns true if it is this players turn.
    /// </summary>
    public bool isTheirTurn() {
        return this.isMyTurn;
    }

    /// <summary>
    /// Sets this Player's team, adjusts their camera, colors the cells of their base and closes the waiting screen.
    /// </summary>
    public void setTeam(Team team, Vector3 cameraPosition, Quaternion cameraRotation) {
        if(this.team == null) {
            this.team = team;

            // Adjust The Camera to look at this players spot on the board.
            Camera c = Camera.main;
            c.transform.localRotation = cameraRotation;
            c.transform.position = Vector3.zero;
            c.transform.Translate(cameraPosition);
            c.transform.Rotate(new Vector3(51, 0, 0));

            // Color their spaces on the board.
            foreach(Cell cell in this.board.sliceArray[team.getId()].getCells()) {
                cell.setOwnedByLocal(team);
            }

            // This results in the waiting screen being hidden.
            this.screenManager.showScreen(null);

            // Show the flag and gear buttons.
            this.buttonSurrenderFlag.gameObject.SetActive(true);
            this.buttonGear.gameObject.SetActive(true);
        }
        else {
            Debug.LogWarning("Player#setTeam() should only be called once!");
        }
    }

    /// <summary>
    /// Returns the team this player controls.  Null is returned if this player has only
    /// every spectated or if the team hasn't been set yet.
    /// </summary>
    public Team getTeam() {
        return this.team;
    }

    /// <summary>
    /// Alerts the Player that a different Player has left the game via a Popup that can't be closed.
    /// </summary>
    public void allertMissingPlayers(bool missing) {
        if(!this.isSpectating()) {
            this.missingPlayers = missing;
            this.missingPlayerCanvas.gameObject.SetActive(this.missingPlayers);
        }
    }

    /// <summary>
    /// Starts the game and end's the setup phase.
    /// </summary>
    public void startGame() {
        this.announcementText.setText("The Game Has Begun", 1f);

        this.func();
    }

    private void func() {
        // Remove the highlight effect.
        this.setupPanel.setSelectedPiece(null);
        this.setupPanel.func();
    }

    public void callback_gear() {
        this.uiManager.showUi(PopupIds.GAME_SETTINGS);
    }

    public void callback_flag() {
        this.uiManager.showUi(PopupIds.SURRENDER_CONFIRM);
    }
}