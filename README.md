# Turnbased Multiplayer

## Turn-based Protocol Design

### Game State Object

* stage (String): what is the current state of the game. In some game, there might be a SETUP stage where players have to pick sides and roles.
* numRounds (Integer): which round it is in the game. A round consists of multiple phases.
* phase (String): what is the name of the current phase.
* participants (Participant): The players of the game.
* assets (Any): The state of the board or decks of cards

### Participants (Class)

* properties
* cards
* decks

### Message Types 

* onStageChanged
* onTurnChanged
* onPhaseChanged
* commitMoves (Moves)