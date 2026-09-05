using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public ActionResult<Game> CreateGame(
        [FromBody] CreateGameRequest request)
    {
        var game = _gameService.CreateGame(request.Mode);

        return Ok(game);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<Game> GetGame(Guid id)
    {
        var game = _gameService.GetGame(id);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    [HttpPost("{id:guid}/moves")]
    public ActionResult<Game> MakeMove(
        Guid id,
        [FromBody] MakeMoveRequest request)
    {
        try
        {
            var game = _gameService.MakeMove(
                id,
                request.Player,
                request.Row,
                request.Column);

            return Ok(game);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/undo")]
    public ActionResult<Game> Undo(Guid id)
    {
        try
        {
            return Ok(_gameService.Undo(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reset")]
    public ActionResult<Game> Reset(Guid id)
    {
        try
        {
            return Ok(_gameService.ResetGame(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public record CreateGameRequest(GameMode Mode);

public record MakeMoveRequest(
    Player Player,
    int Row,
    int Column);