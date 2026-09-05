namespace TicTacToe.Api.Models;

public class GameMove
{
    public int MoveNumber { get; set; }

    public Player Player { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }
}