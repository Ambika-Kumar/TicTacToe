using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public class GameService
{
    private readonly GameRules _gameRules;
    private readonly Dictionary<Guid, Game> _games = new();
    private readonly Scoreboard _scoreboard = new();

    private static readonly (int Row, int Column)[] Corners =
    {
        (0, 0), (0, 2), (2, 0), (2, 2)
    };

    public GameService(GameRules gameRules) => _gameRules = gameRules;

    public Game CreateGame(GameMode mode)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(), Mode = mode, CurrentPlayer = Player.X,
            Status = GameStatus.InProgress
        };
        _games[game.Id] = game;
        return game;
    }

    public Game? GetGame(Guid gameId) => _games.TryGetValue(gameId, out var game) ? game : null;

    public Scoreboard GetScoreboard() => _scoreboard;

    public Game MakeMove(Guid gameId, Player player, int row, int column)
    {
        var game = GetRequiredGame(gameId);
        ValidateMove(game, player, row, column);
        ApplyMove(game, player, row, column);

        if (game.Status != GameStatus.InProgress)
            return game;

        if (game.Mode == GameMode.Computer)
        {
            game.CurrentPlayer = Player.O;
            var computerMove = SelectComputerMove(game);
            if (computerMove is not null)
            {
                ApplyMove(game, Player.O, computerMove.Value.Row, computerMove.Value.Column);
                if (game.Status == GameStatus.InProgress)
                    game.CurrentPlayer = Player.X;
            }
            return game;
        }

        game.CurrentPlayer = player == Player.X ? Player.O : Player.X;
        return game;
    }

    public Game Undo(Guid gameId)
    {
        var game = GetRequiredGame(gameId);
        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Undo is disabled after the game is completed.");
        if (game.MoveHistory.Count == 0)
            throw new InvalidOperationException("There are no moves to undo.");

        var movesToRemove = game.Mode == GameMode.Computer ? Math.Min(2, game.MoveHistory.Count) : 1;
        for (var i = 0; i < movesToRemove; i++)
        {
            var lastMove = game.MoveHistory[^1];
            game.Board[lastMove.Row, lastMove.Column] = null;
            game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
        }

        game.CurrentPlayer = game.Mode == GameMode.Computer ? Player.X :
            game.MoveHistory.Count == 0 ? Player.X :
            game.MoveHistory[^1].Player == Player.X ? Player.O : Player.X;
        game.Status = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells.Clear();
        return game;
    }

    public Game ResetGame(Guid gameId)
    {
        var game = GetRequiredGame(gameId);
        game.Board = new Player?[3, 3];
        game.CurrentPlayer = Player.X;
        game.Status = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells.Clear();
        game.MoveHistory.Clear();
        game.ScoreRecorded = false;
        return game;
    }

    private void ApplyMove(Game game, Player player, int row, int column)
    {
        game.Board[row, column] = player;
        game.MoveHistory.Add(new GameMove
        {
            MoveNumber = game.MoveHistory.Count + 1,
            Player = player, Row = row, Column = column
        });

        var winningCells = _gameRules.GetWinningCells(game.Board, player);
        if (winningCells.Count > 0)
        {
            game.Status = GameStatus.Won;
            game.Winner = player;
            game.WinningCells = winningCells
                .Select(c => $"Row {c.Row + 1}, Column {c.Column + 1}")
                .ToList();
            UpdateScoreboard(game);
            return;
        }

        if (_gameRules.IsBoardFull(game.Board))
        {
            game.Status = GameStatus.Draw;
            UpdateScoreboard(game);
        }
    }

    private (int Row, int Column)? SelectComputerMove(Game game)
    {
        // 1. O can win.
        var winning = FindWinningMove(game.Board, Player.O);
        if (winning is not null) return winning;

        // 2. X can win next: block X.
        var block = FindWinningMove(game.Board, Player.X);
        if (block is not null) return block;

        // 3. Center.
        if (game.Board[1, 1] is null) return (1, 1);

        // 4. Corner.
        foreach (var corner in Corners)
            if (game.Board[corner.Row, corner.Column] is null) return corner;

        // 5. Any available cell.
        for (var row = 0; row < 3; row++)
            for (var column = 0; column < 3; column++)
                if (game.Board[row, column] is null) return (row, column);

        return null;
    }

    private (int Row, int Column)? FindWinningMove(Player?[,] board, Player player)
    {
        for (var row = 0; row < 3; row++)
        {
            for (var column = 0; column < 3; column++)
            {
                if (board[row, column] is not null) continue;
                board[row, column] = player;
                var wins = _gameRules.GetWinningCells(board, player).Count > 0;
                board[row, column] = null;
                if (wins) return (row, column);
            }
        }
        return null;
    }

    private void ValidateMove(Game game, Player player, int row, int column)
    {
        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("The game has already been completed.");
        if (row < 0 || row > 2 || column < 0 || column > 2)
            throw new ArgumentException("Row and column must be between 0 and 2.");
        if (game.Mode == GameMode.Computer && player != Player.X)
            throw new InvalidOperationException("In Computer mode, the human player is X.");
        if (game.CurrentPlayer != player)
            throw new InvalidOperationException("It is not this player's turn.");
        if (game.Board[row, column] != null)
            throw new InvalidOperationException("The selected cell is already occupied.");
    }

    private Game GetRequiredGame(Guid gameId)
    {
        if (!_games.TryGetValue(gameId, out var game))
            throw new KeyNotFoundException("Game not found.");
        return game;
    }

    private void UpdateScoreboard(Game game)
    {
        if (game.ScoreRecorded) return;
        if (game.Status == GameStatus.Won && game.Winner == Player.X) _scoreboard.XWins++;
        else if (game.Status == GameStatus.Won && game.Winner == Player.O) _scoreboard.OWins++;
        else if (game.Status == GameStatus.Draw) _scoreboard.Draws++;
        game.ScoreRecorded = true;
    }
}
