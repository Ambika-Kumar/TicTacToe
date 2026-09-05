using Xunit;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using TicTacToe.Api;

namespace TicTacToe.Tests;

public class GameRulesTests
{
    private readonly GameRules _rules = new();

    [Fact]
    public void DetectsRowWin()
    {
        var board = new Player?[3, 3];
        board[0, 0] = board[0, 1] = board[0, 2] = Player.X;
        Assert.Equal(3, _rules.GetWinningCells(board, Player.X).Count);
    }

    [Fact]
    public void DetectsColumnWin()
    {
        var board = new Player?[3, 3];
        board[0, 1] = board[1, 1] = board[2, 1] = Player.O;
        Assert.Equal(3, _rules.GetWinningCells(board, Player.O).Count);
    }

    [Fact]
    public void DetectsBothDiagonals()
    {
        var board = new Player?[3, 3];
        board[0, 0] = board[1, 1] = board[2, 2] = Player.X;
        Assert.Equal(3, _rules.GetWinningCells(board, Player.X).Count);
        board = new Player?[3, 3];
        board[0, 2] = board[1, 1] = board[2, 0] = Player.O;
        Assert.Equal(3, _rules.GetWinningCells(board, Player.O).Count);
    }

    [Fact]
    public void DetectsFullBoard()
    {
        var board = new Player?[3, 3];
        for (var r = 0; r < 3; r++)
            for (var c = 0; c < 3; c++)
                board[r, c] = (r + c) % 2 == 0 ? Player.X : Player.O;
        Assert.True(_rules.IsBoardFull(board));
    }
}
