# Tic Tac Toe

A browser-based Tic Tac Toe application built with Angular and a .NET Web API.

The application supports both Two Player Mode and Play Against Computer Mode. The backend manages game state, validates moves, maintains move history, and manages the scoreboard.

## Technology Stack

- Angular
- TypeScript
- .NET Web API
- C#
- REST API
- In-memory storage
- GitHub

## Features

- Standard 3 x 3 Tic Tac Toe board
- Two Player Mode
- Play Against Computer Mode
- Automatic turn handling
- Row, column, and diagonal win detection
- Draw detection
- Winning-cell highlighting
- Move history
- Undo Last Move
- Session scoreboard
- Reset Game
- Reset Scoreboard
- Invalid move validation
- Backend-driven game state
- Automated backend tests

## Game Modes

### Two Player Mode

Both X and O are controlled by players.

X starts the game and the players alternate turns after every valid move.

An occupied cell cannot be selected again.

### Play Against Computer Mode

In Computer Mode:

- The human player is X.
- The computer is O.
- The computer automatically makes its move after X.

The computer follows this priority:

1. If O can win, take the winning move.
2. If X can win on the next move, block X.
3. Take the center if available.
4. Take a corner if available.
5. Take any available cell.

The computer does not make another move after the game has been completed.

## Move Validation

Move validation is handled by the backend.

The backend rejects:

- Moves outside the board
- Moves on occupied cells
- Moves after the game has finished
- Moves made by the wrong player

For example, if a player selects an occupied cell, the application displays:

`Invalid move: Cell is already occupied.`

An invalid move does not change the current player's turn.

## Move History

The application displays the move history for the current game.

Each move contains:

- Move number
- Player
- Row
- Column

Example:

    Move 1 - X - Row 1, Column 1
    Move 2 - O - Row 2, Column 2

The history is updated after every valid move.

## Undo

Undo restores the game to the previous valid state.

### Two Player Mode

Undo removes the most recent move.

Example:

    X plays
    O plays
    User clicks Undo

O's move is removed and it becomes O's turn again.

### Computer Mode

Undo removes the computer's last move and the human player's previous move together.

Example:

    X plays
    O computer plays
    User clicks Undo

Both the O move and the previous X move are removed, and it becomes X's turn again.

### Completed Games

Option A was selected from the exercise requirements.

Undo is disabled after a game has been won or drawn. This keeps the completed result and scoreboard unchanged.

## Scoreboard

The application maintains a session-level scoreboard containing:

- X Wins
- O Wins
- Draws

The scoreboard is maintained by the backend.

When a game is completed, the appropriate score is updated.

Reset Game starts a new game without changing the scoreboard.

Reset Scoreboard clears all scoreboard values.

Since the application uses in-memory storage, the scoreboard is reset when the backend is restarted.

## Backend State

The backend is the source of truth for the current game.

It manages:

- Game ID
- Board state
- Current player
- Game mode
- Game status
- Winner
- Winning cells
- Move history
- Scoreboard

The frontend uses the state returned by the backend to display the current game.

# API Documentation / Endpoint Summary

The backend exposes REST APIs for the main game operations.

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/games` | Create a new game |
| GET | `/api/games/{id}` | Get the current game |
| POST | `/api/games/{id}/moves` | Submit a move |
| POST | `/api/games/{id}/undo` | Undo the last move |
| POST | `/api/games/{id}/reset` | Reset the current game |
| GET | `/api/scoreboard` | Get the scoreboard |
| POST | `/api/scoreboard/reset` | Reset the scoreboard |

Swagger/OpenAPI is available when the backend is running.

# Project Structure

    TicTacToe/
    |
    +-- backend/
    |   +-- TicTacToe.Api/
    |   +-- TicTacToe.Tests/
    |   +-- TicTacToe.slnx
    |
    +-- frontend/
    |   +-- tictactoe-web/
    |
    +-- .gitignore
    +-- README.md
    +-- package-lock.json

# Setup and Run Instructions

## Prerequisites

The following are required to run the application locally:

- .NET SDK
- Node.js
- Angular CLI

## Run the Backend

From the project root:

    cd backend
    dotnet run --project TicTacToe.Api

The .NET API will start locally.

Swagger/OpenAPI can be used to review and test the available API endpoints.

## Run the Frontend

Open a second terminal:

    cd frontend\tictactoe-web

Install the dependencies:

    npm install

Start the Angular application:

    ng serve

Open the local Angular URL shown in the terminal.

The Angular application communicates with the .NET backend through REST APIs.

# Test Instructions

From the backend directory:

    cd backend
    dotnet test

The current test run completes successfully:

    Test summary:
    Total: 17
    Failed: 0
    Succeeded: 17
    Skipped: 0

    Build succeeded

The tests cover the core game rules and state transitions, including:

- Valid moves
- Invalid moves
- Turn switching
- Row wins
- Column wins
- Diagonal wins
- Draw
- Reset Game
- Undo in Two Player Mode
- Undo in Computer Mode
- Scoreboard updates
- Computer move selection
- Moves after game completion

# Design Decisions

## Backend as the Source of Truth

The game rules and game state are managed by the backend rather than being independently maintained by the frontend.

This keeps move validation, game status, move history, and scoreboard behavior consistent.

## In-Memory Storage

In-memory storage is used because it is sufficient for this exercise and keeps the application simple to run.

The trade-off is that game state and the scoreboard are lost when the backend is restarted.

## Computer Move Logic

The computer logic follows the priority specified in the exercise rather than using a more complex AI algorithm.

This keeps the implementation straightforward while satisfying the required behavior.

## Undo After Completion

Option A was selected from the exercise requirements.

Undo is disabled after a game has been completed, so a completed scoreboard result does not need to be reversed.

# Clarifications and Assumptions

- The application is intended to run locally for the assessment.
- In-memory storage is sufficient for this exercise.
- The scoreboard represents the current backend session.
- Restarting the backend starts a new session.
- X is always the human player in Computer Mode.
- O is always controlled by the computer in Computer Mode.
- Undo is disabled after a game is completed.
- Authentication is not required for this exercise.
- A persistent database is not required.

# Known Limitations

- Game state is stored in memory.
- The scoreboard is lost when the backend restarts.
- There is no authentication or user management.
- The application is primarily configured for local development.
- There is no persistent database.

These limitations are acceptable for the scope of this exercise.

# AI-Assisted Development / Prompt Summary

AI tools were used as a development assistant during implementation, review, testing, troubleshooting, and documentation.

The following are examples of the prompts used during development.

## Requirement Analysis

- "Break down the Tic Tac Toe requirements into frontend, backend, API, and testing tasks."
- "Review the requirements and identify all functional scenarios that need to be implemented and tested."

## Backend Development

- "Help design the .NET REST API for a Tic Tac Toe application."
- "Review the game logic and make sure the backend validates moves, turns, wins, draws, and completed games correctly."
- "Check the implementation for invalid moves, including selecting an already occupied cell."

## Computer Mode

- "Implement the computer move logic using this priority: win if possible, block the opponent, take center, take a corner, then take any available cell."
- "Review the computer mode and make sure the computer does not make a move after the game is completed."

## Frontend

- "Review the Angular Tic Tac Toe UI and make sure the board, current status, move history, scoreboard, buttons, and error messages are displayed correctly."
- "Fix the Angular UI so that an invalid move displays the backend error message to the user."

## Undo

- "Review the Undo requirements for both Two Player Mode and Computer Mode."
- "In Computer Mode, Undo should remove the computer's last move and the human player's previous move together. Verify that the game returns to X's turn."

## Testing

- "Create a test plan covering valid moves, invalid moves, wins, draws, undo, reset, scoreboard, and computer mode."
- "Review the existing .NET tests and identify any missing scenarios."
- "Help troubleshoot failing tests and verify that the final test suite covers the required behavior."

## Troubleshooting

- "The Angular application is running locally but the expected error message is not displayed. Help identify whether the issue is in the frontend, API response handling, or template."
- "Review the project structure and help identify where the Angular component and template code should be updated."

## Documentation

- "Review the README against the assignment requirements and identify any missing documentation."
- "Create a clear README containing project overview, technology stack, features, setup instructions, test instructions, API endpoint summary, AI workflow, assumptions, limitations, and future improvements."

## Final Review

- "Review the complete Tic Tac Toe implementation against the original requirements and identify any gaps before submission."

## AI Workflow

The general workflow followed during development was:

1. Understand and break down the requirements.
2. Design the frontend and backend structure.
3. Implement the .NET API and game rules.
4. Implement the Angular UI.
5. Add validation and error handling.
6. Implement Computer Mode.
7. Implement Undo and scoreboard behavior.
8. Create and review automated tests.
9. Run the application locally and manually verify the main scenarios.
10. Review the implementation against the requirements.
11. Update the README and prepare the GitHub submission.

AI suggestions were reviewed against the actual requirements and tested in the application.

The final implementation was manually verified for the main game scenarios and the backend test suite was run successfully.

# Future Improvements

If the application were developed further, possible improvements would include:

- Persistent game and scoreboard storage
- Player names and user accounts
- Online multiplayer
- Multiple computer difficulty levels
- A stronger computer strategy such as minimax
- Additional frontend automated tests
- CI/CD integration
- Production deployment
- More comprehensive application logging

# Review Checklist

## Basic Game

- Start a new game.
- Make X and O moves.
- Verify that turns alternate correctly.

## Invalid Move

- Select an already occupied cell.
- Verify the error message.
- Verify that the current turn does not change.

## Win

- Test a row win.
- Test a column win.
- Test a diagonal win.
- Verify that winning cells are highlighted.
- Verify that additional moves are prevented.
- Verify that the scoreboard is updated.

## Draw

- Fill the board without a winner.
- Verify the draw message.
- Verify the scoreboard.
- Verify that additional moves are prevented.

## Undo

- Test Undo in Two Player Mode.
- Test Undo in Computer Mode.
- Verify that the board, turn, and move history are restored correctly.

## Reset

- Use Reset Game.
- Verify that the board and move history are cleared.
- Verify that X starts again.
- Verify that the scoreboard remains unchanged.

## Computer Mode

- Switch to Play Against Computer.
- Verify that the computer plays automatically.
- Verify that the computer makes only valid moves.
- Verify the required move-selection priority.

# Submission Requirements

This repository contains:

- Angular frontend source code
- .NET backend source code
- README.md
- Setup and run instructions
- Test instructions
- Prompt summary / AI workflow notes
- API documentation / endpoint summary
- Known assumptions and limitations
