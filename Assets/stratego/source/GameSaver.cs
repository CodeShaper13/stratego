using fNbt;
using System.IO;

public class GameSaver {

    private const string SAVE_FILE_NAME = "currentGame.nbt";

    /// <summary>
    /// Returns true if the save file exists.
    /// </summary>
    public static bool doesSaveExists() {
        return File.Exists(SAVE_FILE_NAME);
    }

    /// <summary>
    /// Saves the current game to disk.
    /// </summary>
    public static void saveGame() {
        CustomNetworkManager manager = CustomNetworkManager.getSingleton();

        NbtCompound rootTag = new NbtCompound("data");

        rootTag.Add(manager.getBoard().writeToNbt(new NbtCompound("board")));
        rootTag.Add(manager.getGameOptions().writeToNbt(new NbtCompound("options")));

        NbtFile nbtFile = new NbtFile(rootTag);
        nbtFile.SaveToFile(GameSaver.SAVE_FILE_NAME, NbtCompression.None);
    }

    /// <summary>
    /// Loads the saved game and starts up the server.  False is returned if the game can't be found.
    /// </summary>
    public static bool loadGame(CustomNetworkManager manager) {
        if(GameSaver.doesSaveExists()) {
            NbtFile nbtFile = new NbtFile();
            nbtFile.LoadFromFile(GameSaver.SAVE_FILE_NAME);
            NbtCompound rootTag = nbtFile.RootTag;

            GameOptions options = new GameOptions(rootTag.getCompound("options"));
            NbtCompound boardNbt = rootTag.getCompound("board");

            manager.startGameServer(options, boardNbt);

            return true;
        } else {
            return false;
        }
    }
}
