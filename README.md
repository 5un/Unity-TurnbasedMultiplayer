# Turnbased Multiplayer

## Turn-based Protocol Design

### Game State Object

* stage (String): what is the current state of the game. In some game, there might be a SETUP stage where players have to pick sides and roles.
* numRounds (Integer): which round it is in the game. A round consists of multiple phases.
* phase (String): what is the name of the current phase.
* whoseTurn (Participant Id): whose turn it is in the phase, if applicable.
* participants (Participant): The players of the game.
* capabilityCards (List of Cards): 
* goals (List of String):
* board (List of Cells):

### Participants (Class)

Represents a player in the game

* nakamaUserId (String): User Id in Nakama's session manager
* country (String): The country or factor the user is playing for
* cardsOnHand (List of Cards)
* cardsOnTable (List of Cards)
* goals (List of String): hardcoded names of goals
* signalingTokens (Integer): the numbers of signaling tokens left to use
* money (Float)
* preciousMetal (Float)

### Cell (Class)

Represents a cell, hex, or territory on the game board.

* location (???): we will need to come up with a location notation system and distance function 
* adjacents (List of Cells): cells that are adjacent to the cell.
* controlledBy (Participant Id): The one who controls the cell.
* enabled: Is the cell enabled to be played or it is tainted by radiation
* infrastructure (Infrastructure): The infrastructure that was built on the cell. Can be null.
* population (Integer): number of populations on the cell

### Card (Class)

Represents a card. We could also subclass it into different types.

* name (Float)
* type (Float)
* description (String)
* influenceValue (Float)
* damageValue (Float)
* defenseValue (Float)
* maximumRange (Integer)

### Infrastructure (Class)

Represents a *built* infrastructed on a cell. 

* name (String)
* type (String): millitary | cilivian
* hp (Float)

### Moves (Class)

* name (String)
* type (String)

#### Types of Moves possible

* selectCountry (country)
* proposeTrade (items, toWhom)
* acceptTrade (fromWhom)
* declineTrade (fromWhom)
* placeSignalingToken (cell, whatCapCard) 
* playCard (card, assignToCell, targetCell): used in resolution phase to play cards
* buildInfrastructure (what, where): Is build infra the same as playing cards
* upgradeInfrastructure (infrastructure, toWhat)
* movePopulation (fromCell, toCell)

### Nakama Message Opcodes 

* CHAT
* HOST_GAME_STATE
* GUEST_MOVES
* EVENTS