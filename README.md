# Sokoban-Game
A terminal based sokoban styled game, created in C# with the .NETCore framework.
The game currently contains 4 levels that can be played through. The first three focus on rock puzzles, and then the fourth is a more advanced level, which uses keys and multiple locked doors.
Each level is stored in a file, and then an additional file contains the level order for the game.
The level files are loaded in, and their symbols converted to the appropriate game "objects" (an enum value), and then, these objects are each tied with an ascii character which serves as their sprite.
To improve performance, only the portions of the level which have changed get redrawn.


### Controls
- W - Move Up
- A - Move Left
- S - Move Down
- D - Move Right
- R - Restart the Level

### Legend
- @ - The player
- \# - A rock that can be pushed
- O - A hole which blocks you, but which can be crossed after filling it in with a rock
- $ - Collectible points
- G - The goal, reaching it takes you to the next level

The letters d, f and g are keys which can open doors. Each key, a letter case letter, corresponds to a door, the corresponding capital letter.
- Key d opens door D
- Key f opens door F
- Key g opens door G
