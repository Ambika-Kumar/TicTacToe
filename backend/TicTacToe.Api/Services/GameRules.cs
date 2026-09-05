using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public class GameRules
{
    public List<(int Row, int Column)> GetWinningCells(
        Player?[,] board,
        Player player)
    {
        var winningLines = new List<List<(int Row, int Column)>>
        {
            new() { (0, 0), (0, 1), (0, 2) },
            new() { (1, 0), (1, 1), (1, 2) },
            new() { (2, 0), (2, 1), (2, 2) },

            new() { (0, 0), (1, 0), (2, 0) },
            new() { (0, 1), (1, 1), (2, 1) },
            new() { (0, 2), (1, 2), (2, 2) },

            new() { (0, 0), (1, 1), (2, 2) },
            new() { (0, 2), (1, 1), (2, 0) }
        };

        foreach (var line in winningLines)
        {
            if (line.All(cell =>
                board[cell.Row, cell.Column] == player))
            {
                return line;
            }
        }

        return new List<(int Row, int Column)>();
    }

    public bool IsBoardFull(Player?[,] board)
    {
        for (var row = 0; row < 3; row++)
        {
            for (var column = 0; column < 3; column++)
            {
                if (board[row, column] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}