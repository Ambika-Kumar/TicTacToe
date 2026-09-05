using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    private static GameService CreateService() => new(new GameRules());

    [Fact]
    public void CreateGameStartsWithX()
    {
        var game = CreateService().CreateGame(GameMode.TwoPlayer);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Empty(game.MoveHistory);
    }

    [Fact]
    public void ValidMoveSwitchesTurn()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        game = service.MakeMove(game.Id, Player.X, 0, 0);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Equal(Player.X, game.Board[0, 0]);
    }

    [Fact]
    public void WrongPlayerIsRejected()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        Assert.Throws<InvalidOperationException>(() => service.MakeMove(game.Id, Player.O, 0, 0));
    }

    [Fact]
    public void OccupiedCellIsRejected()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        Assert.Throws<InvalidOperationException>(() => service.MakeMove(game.Id, Player.O, 0, 0));
    }

    [Fact]
    public void InvalidCoordinatesAreRejected()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        Assert.Throws<ArgumentException>(() => service.MakeMove(game.Id, Player.X, 3, 0));
    }

    [Fact]
    public void DetectsWinAndRecordsWinningCells()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 1, 0);
        service.MakeMove(game.Id, Player.X, 0, 1);
        service.MakeMove(game.Id, Player.O, 1, 1);
        game = service.MakeMove(game.Id, Player.X, 0, 2);
        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(3, game.WinningCells.Count);
        Assert.True(game.ScoreRecorded);
        Assert.Equal(1, service.GetScoreboard().XWins);
    }

    [Fact]
    public void MoveAfterCompletionIsRejected()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 1, 0);
        service.MakeMove(game.Id, Player.X, 0, 1);
        service.MakeMove(game.Id, Player.O, 1, 1);
        service.MakeMove(game.Id, Player.X, 0, 2);
        Assert.Throws<InvalidOperationException>(() => service.MakeMove(game.Id, Player.O, 2, 2));
    }

    [Fact]
    public void UndoTwoPlayerRestoresTurn()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 1, 1);
        game = service.Undo(game.Id);
        Assert.Null(game.Board[1, 1]);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.MoveHistory);
    }

    [Fact]
    public void ComputerAutomaticallyPlaysOAndUndoRemovesPair()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.Computer);
        game = service.MakeMove(game.Id, Player.X, 0, 0);
        Assert.Equal(2, game.MoveHistory.Count);
        Assert.Equal(Player.O, game.Board[1, 1]);
        Assert.Equal(Player.X, game.CurrentPlayer);
        game = service.Undo(game.Id);
        Assert.Empty(game.MoveHistory);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    [Fact]
    public void ComputerBlocksImmediateXWin()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.Computer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        game = service.MakeMove(game.Id, Player.X, 0, 1);
        Assert.Equal(Player.O, game.Board[0, 2]);
    }

    [Fact]
    public void ResetClearsGameButKeepsScoreboard()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 1, 0);
        service.MakeMove(game.Id, Player.X, 0, 1);
        service.MakeMove(game.Id, Player.O, 1, 1);
        service.MakeMove(game.Id, Player.X, 0, 2);
        game = service.ResetGame(game.Id);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Empty(game.MoveHistory);
        Assert.Equal(1, service.GetScoreboard().XWins);
    }
    [Fact]
    public void ComputerTakesWinningMoveWhenAvailable()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.Computer);

        // X at (0,0) -> O center.
        service.MakeMove(game.Id, Player.X, 0, 0);
        // X at (1,0) -> O chooses first corner (0,2).
        service.MakeMove(game.Id, Player.X, 1, 0);
        // X at (2,1) -> O should win at (2,0).
        game = service.MakeMove(game.Id, Player.X, 2, 1);

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.O, game.Winner);
        Assert.Equal(Player.O, game.Board[2, 0]);
    }

    [Fact]
    public void DetectsDrawAndUpdatesScoreboard()
    {
        var service = CreateService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 0, 1);
        service.MakeMove(game.Id, Player.X, 0, 2);
        service.MakeMove(game.Id, Player.O, 1, 1);
        service.MakeMove(game.Id, Player.X, 1, 0);
        service.MakeMove(game.Id, Player.O, 1, 2);
        service.MakeMove(game.Id, Player.X, 2, 1);
        service.MakeMove(game.Id, Player.O, 2, 0);
        game = service.MakeMove(game.Id, Player.X, 2, 2);

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
        Assert.True(game.ScoreRecorded);
        Assert.Equal(1, service.GetScoreboard().Draws);
    }

}

