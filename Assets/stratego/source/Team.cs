public class Team {

    public static readonly Team RED = new Team("Red", 0);
    public static readonly Team BLUE = new Team("Blue", 1);
    public static readonly Team YELLOW = new Team("Yellow", 2);
    public static readonly Team GREEN = new Team("Green", 3);
    public static readonly Team PURPLE = new Team("Purple", 4);
    public static readonly Team ORANGE = new Team("Orange", 5);

    public static readonly Team[] ALL_TEAMS = new Team[] { RED, BLUE, YELLOW, GREEN, PURPLE, ORANGE, };

    private readonly string name;
    private readonly int id;

    public Team(string name, int id) {
        this.name = name;
        this.id = id;
    }

    public int getId() {
        return this.id;
    }

    public string getName() {
        return this.name;
    }

    public static Team teamFromId(int id) {
        if(id < 0 || id >= Team.ALL_TEAMS.Length) {
            return null;
        } else {
            return Team.ALL_TEAMS[id];
        }
    }
}
