# Turnbased Multiplayer

## Turn-based Protocol Design

### Game State Object

* state (String): what is the current state of the game
* numRounds (Integer): what is the turn number of the current turn
* phase (String): what is the name of the current phase
* participants (Participant): 
* assets (Any): Different deck of cards and etc

### Participants (Class)

* properties
* cards
* decks

### Message Types 

* onStageChanged
* onTurnChanged
* onPhaseChanged
* commitMoves (Moves)