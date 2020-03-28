using fNbt;
using UnityEngine;
using UnityEngine.Networking;

public class Board : NetworkBehaviour {

    /// <summary> The state of the game. 0 = waiting for players, 1 = setup, 2 = playing, 3 = end. </summary>
    [SyncVar]
    public int gameState;

    /// <summary> An array of all of the cells that make up the board. </summary>
    private Cell[] allCells;
    public Slice[] sliceArray;

    [ServerSideOnly]
    private AvailibleTeams availibleTeams;
    /// <summary> The current attack that is happening. Null is there is no attack going on. </summary>
    [ServerSideOnly]
    public Attack attack;
    [ServerSideOnly]
    private int turnIndex;
    [ServerSideOnly]
    private NetHandlerServer handler;
    [ServerSideOnly]
    public ConnectedPlayerList allPlayers;

    public void Awake() {
        // Create an array of all the Cells.
        this.allCells = GameObject.FindObjectsOfType<Cell>();
    }

    public override void OnStartServer() {
        base.OnStartServer();

        this.handler = new NetHandlerServer(this);
    }

    /// <summary>
    /// Called after the board object is created for new games.
    /// </summary>
    /// <param name="options"></param>
    public void initNewGame(GameOptions options) {
        this.availibleTeams = new AvailibleTeams(options.maxPlayers.get(), this.sliceArray);
        this.gameState = GameStates.WAITING;
    }

    private void Update() {
        if(this.isServer) {
            #region DEBUG
            if(Input.GetKeyDown(KeyCode.F2)) {
                if(this.gameState == GameStates.WAITING) {
                    this.startSetupPhase();
                }
            }
            if(Input.GetKeyDown(KeyCode.F3)) {
                if(this.gameState != GameStates.SETUP) {
                    this.startSetupPhase();
                }
                this.startPlayingPhase();
            }
            if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) {
                GameSaver.saveGame();
                print("DEBUG: Saving Game...");
            }
            #endregion DEBUG

            if(!CustomNetworkManager.getSingleton().missingPlayers) {
                if(this.gameState == GameStates.WAITING) {
                    // Do nothing, we're just waiting for all the players to join.
                }
                else if(this.gameState == GameStates.SETUP) {
                    // Check if all the Players are ready.
                    bool allReady = true;
                    foreach(ConnectedPlayer player in this.allPlayers) {
                        allReady &= player.isReady();
                    }
                    if(allReady) {
                        this.startPlayingPhase();
                    }
                }
                else if(this.gameState == GameStates.PLAYING) {
                    if(this.attack != null) {
                        bool isAttackOver = this.attack.update();
                        if(isAttackOver) {
                            this.attack = null;
                        }
                    }
                }
                else if(this.gameState == GameStates.END) {

                }
            }
        }
    }

    /// <summary>
    /// Eliminates the passed player from the game.  If there is only one player left, this will
    /// trigger a win for the remaining player.
    /// </summary>
    /// <param name="playerToEliminate"></param>
    /// <param name="reason">A string stating what caused the player to lose.</param>
    [ServerSideOnly]
    public void eliminatePlayer(ConnectedPlayer playerToEliminate, int reason) {
        Team eliminatedTeam = playerToEliminate.getTeam();
        string teamName = eliminatedTeam.getName();

        string s = string.Empty;
        if(reason == EliminationReson.SURRENDER) {
            s = " Has Surrendered";
        } else if(reason == EliminationReson.NO_MOVE) {
            s = " Has No More Moves";
        } else if(reason == EliminationReson.LOSE_FLAG) {
            s = " Lost Their Flag";
        }
        string elimReasonMsg = TextColorer.getColoredText(eliminatedTeam, teamName) + s;

        // Find out if someone has won the game.
        int remainingPlayers = 0;
        ConnectedPlayer playerWhoWon = null;
        foreach(ConnectedPlayer p in this.allPlayers) {
            if(!p.isSpectating() && !p.isEqualTo(playerToEliminate)) { // connections can now be null
                remainingPlayers++;
                playerWhoWon = p;
            }
        }

//        print("Remaining P: " + remainingPlayers);

        if(playerWhoWon == null) {
            print("THERE WAS ONLY 1 PLAYER IN THIS GAME AND THEY LOST, IS THIS IN DEV MODE?");
        } else {
            if(remainingPlayers <= 1) {
                // Someone won the game!

                foreach(ConnectedPlayer p in this.allPlayers) {
                    // Send a message to everyone, telling them about the winner.
                    p.sendMessage(new MessageShowEliminationMsg(TextColorer.getColoredText(playerWhoWon.getTeam(), playerWhoWon.getTeam().getName()) + " Has Won", elimReasonMsg));

                    // Put everyone in spectator mode
                    p.setSpectating();
                }
            }
            else {
                // No one won yet, send a message to all of the other players, telling them someone was eliminated.
                foreach(ConnectedPlayer p in this.allPlayers) {
                    if(!playerToEliminate.isEqualTo(p)) {
                        p.sendMessage(new MessageShowEliminationMsg(elimReasonMsg, "They Are Now Spectating")); //Msg doesnt show up!!!
                    }
                }

                // Send message to the player who lost, telling them why they lost, and put them in spectator mode.
                playerToEliminate.sendMessage(new MessageShowEliminationMsg(elimReasonMsg, "You Are Now Spectating"));
                playerToEliminate.setSpectating();

                // Remove all of this Player's pieces except for their bombs.
                foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
                    if(piece.getTeam() == playerToEliminate.getTeam()) {
                        if(piece.pieceType != PieceType.BOMB) {
                            this.removePiece(piece);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes the passed Piece from the game.
    /// </summary>
    [ServerSideOnly]
    public void removePiece(Piece piece) {
        GameObject.Destroy(piece.gameObject);
    }

    /// <summary>
    /// When called, this will shift the game from the waiting phase to the setup phase.
    /// </summary>
    [ServerSideOnly]
    public void startSetupPhase() {
        this.gameState = GameStates.SETUP;

        foreach(ConnectedPlayer cp in this.allPlayers) {
            // Give every Player a random team.
            cp.setTeam(this.availibleTeams.getTeam());

            // Send a msg to the Player, telling them their team.
            this.sendTeamInfo(cp);
        }
    }

    /// <summary>
    /// Starts the game, moving it from the setup phase to the playing phase.
    /// </summary>
    [ServerSideOnly]
    private void startPlayingPhase() {
        this.gameState = GameStates.PLAYING;

        this.allPlayers.sendMessage(new MessageGameBegin());

        this.turnIndex = Random.Range(0, this.allPlayers.Count) - 1;
        this.nextTurn();
    }

    /// <summary>
    /// Cycles to the next Player's turn.  A message is sent to all Players telling them of this change.
    /// </summary>
    [ServerSideOnly]
    public void nextTurn() {
        do {
            this.turnIndex += 1;
            if(this.turnIndex >= this.allPlayers.Count) {
                this.turnIndex = 0;
            }
        } while(this.getCurrentPlayer().isSpectating());

        ConnectedPlayer playerWithTurn = this.getCurrentPlayer();

        // Make sure the Player has a valid move.  If not, they are eliminated.
        bool hasValidMove = false;
        foreach(Cell cell in this.allCells) {
            Piece p = cell.getCurrentPiece();
            if(p != null && p.getTeam() == playerWithTurn.getTeam()) {
                // The player owns this piece.
                if(p.getPossibleMoves().Count > 0) {
                    hasValidMove = true;
                    break;
                }
            }
        }

        // If the Player has no valid move, eliminate them.
        if(!hasValidMove) {
            this.eliminatePlayer(playerWithTurn, EliminationReson.NO_MOVE);
        }

        // Send a message to all the players telling them who's turn it is.
        this.allPlayers.sendMessage(new MessageChangeTurn(playerWithTurn.getTeam()));
    }

    [ServerSideOnly]
    public void addPiece(int pieceType, int cellIndex, int teamId) {
        Piece piece = this.instantiatePiecePrefab();

        piece.teamId = teamId;
        piece.setPositionInstantly(this.getCellFromIndex(cellIndex));
        piece.pieceType = pieceType;

        NetworkServer.Spawn(piece.gameObject);
    }

    [ServerSideOnly]
    public void addPiece(NbtCompound tag) {
        Piece piece = this.instantiatePiecePrefab();

        piece.readFromNbt(tag);

        NetworkServer.Spawn(piece.gameObject);
    }

    [ServerSideOnly]
    private Piece instantiatePiecePrefab() {
        return GameObject.Instantiate(References.list.gamePiece).GetComponent<Piece>();
    }

    /// <summary>
    /// Returns the player who's turn it currently is.
    /// </summary>
    [ServerSideOnly]
    public ConnectedPlayer getCurrentPlayer() {
        return this.allPlayers[this.turnIndex];
    }

    /// <summary>
    /// Returns the cell from the passed cell index/identifier.
    /// </summary>
    public Cell getCellFromIndex(int index) {
        foreach(Cell cell in this.allCells) {
            if(cell.cellIndex == index) {
                return cell;
            }
        }
        throw new System.Exception("Tried to find a cell with an index that was out of bounds!  " + index);
    }

    public NbtCompound writeToNbt(NbtCompound tag) {
        tag.setTag("currentTurn", this.turnIndex);
        tag.setTag("gameState", this.gameState);

        // Write the Availible Teams to NBT.
        NbtCompound tag1 = new NbtCompound("availibleTeams");
        this.availibleTeams.writeToNbt(tag1);
        tag.Add(tag1);

        // Write all of the Players to NBT.
        NbtList tagPlayers = new NbtList("players");
        foreach(ConnectedPlayer player in this.allPlayers) {
            tagPlayers.Add(player.writeToNbt());
        }
        tag.Add(tagPlayers);

        // Write all of the Pieces to NBT.
        NbtList tagPieces = new NbtList("pieces");
        foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
            tagPieces.Add(piece.writeToNbt());
        }
        tag.Add(tagPieces);

        // TODO write attack

        return tag;
    }

    public void readFromNbt(NbtCompound tag) {
        this.turnIndex = tag.getInt("currentTurn");
        this.gameState = tag.getInt("gameState");

        this.availibleTeams = new AvailibleTeams(tag.getCompound("availibleTeams"));

        // Read Players.
        foreach(NbtCompound pTag in tag.getList("players")) {
            this.allPlayers.Add(new ConnectedPlayer(pTag));
        }

        // Read Pieces.
        foreach(NbtCompound pieceTag in tag.getList("pieces")) {
            this.addPiece(pieceTag);
        }

        // TODO read attack

        // Tell the new players who's turn it is
        //TODO this.nextTurn(
    }

    public void sendTeamInfo(ConnectedPlayer player) {
        int teamId = player.getTeam().getId();
        CameraTransfrom camTrans = this.sliceArray[teamId].getOrgin();

        int pCount = CustomNetworkManager.getSingleton().getGameOptions().maxPlayers.get();
        Vector3 pos;
        if(pCount == 2) {
            pos = new Vector3(0f, 6.57f, -6.7f);
        } else {
            pos = new Vector3(0f, 6.57f, -6.5f);
        }
        player.sendMessage(new MessageSetTeam(teamId, pos, camTrans.transform.rotation));
    }
}