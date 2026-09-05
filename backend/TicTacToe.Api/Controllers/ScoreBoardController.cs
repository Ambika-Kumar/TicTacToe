using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly GameService _gameService;

    public ScoreboardController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public ActionResult<Scoreboard> GetScoreboard()
    {
        return Ok(_gameService.GetScoreboard());
    }

    [HttpPost("reset")]
    public ActionResult<Scoreboard> ResetScoreboard()
    {
        var scoreboard = _gameService.GetScoreboard();

        scoreboard.XWins = 0;
        scoreboard.OWins = 0;
        scoreboard.Draws = 0;

        return Ok(scoreboard);
    }
}