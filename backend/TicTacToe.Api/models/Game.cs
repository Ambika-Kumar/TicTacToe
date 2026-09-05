namespace TicTacToe.Api.Models;

public class Game
{
    public Guid Id { get; set; }
    //public Player?[,] Board { get; set; } = new Player?[3, 3];
    [System.Text.Json.Serialization.JsonIgnore] public Player?[,] Board { get; set; } = new Player?[3, 3];
    public Player?[][] BoardView
{
    get
    {
        var result = new Player?[3][];

        for (int row = 0; row < 3; row++)
        {
            result[row] = new Player?[3];

            for (int col = 0; col < 3; col++)
            {
                result[row][col] = Board[row, col];
            }
        }

        return result;
    }
}

    public Player CurrentPlayer { get; set; } = Player.X;

    public GameMode Mode { get; set; }

    public GameStatus Status { get; set; } = GameStatus.InProgress;

    public Player? Winner { get; set; }

    public List<string> WinningCells { get; set; } = new();

    public List<GameMove> MoveHistory { get; set; } = new();

    public bool ScoreRecorded { get; set; }
}