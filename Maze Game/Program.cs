using System;
using System.Collections.Generic;
using System.IO;
using MazeGame.Enums;
using MazeGame.Structs;

//add out where appropriate

namespace MazeGame
{
    class Program
	{
		static void Main(string[] args)
        {
			// Main output for this program is with the console
			// And this is without the console's cursor being used by the user
			// Instead the console cursor gets repositioned to various places 
			// for drawing selectively to the console
			Console.CursorVisible = false; 

			int numOfLevels = 0;
			int currLevelNum = 0;
			string[] levelList = LoadLevelList("./Level List.txt", ref numOfLevels);

			Player player = new Player();

			// Holds all of the cells that get changed from a single turn
			// Is used to determine which cells in the game should get redrawn
			// Is reset to empty after every turn,
			// and is then refilled based on what happens in that turn
			List<OrdPair> updatedCells = new List<OrdPair>(); 
			
			// currLevel, manages updating updatedCells
			// Program.cs, in the loop below, manages updating the player's action
			// CurrLevel manages what happens to that player as result of the player's action
			// (It does this using the player's methods)
			Level currLevel = new Level(player, updatedCells);

			// Master Game Loop (Loops through all of the levels in the game)
			while (currLevelNum < numOfLevels)
			{
				GameState gameState = GameState.IN_PROGRESS;
				currLevel.LoadLevelFile(levelList[currLevelNum]);
				Console.Clear();
				DrawInventoryInitial(player.GetInventory());
				DrawLevelInitial(currLevel);

				// Individual Level Loop
				// Each loop represents a single turn in the level
				// This loop completes when the particular level is completed
				while (gameState != GameState.LEVEL_COMPLETE)
				{
					player.SetAction(Enums.Action.SELECTING); // Setup the player for selecting an action
					player.SetAction(GetActionInput(player.GetInventory().bombs)); // Get and set the player's desired action

					if (player.action == Enums.Action.RESTART)
                    {
						currLevel.RestartLevel();
						Console.Clear();
						DrawInventoryInitial(player.GetInventory());
						DrawLevelInitial(currLevel);
					}
					else
                    {
						gameState = currLevel.UpdateLevel(); // Update the level and all associated entities based on the player's action
						if(updatedCells.Count != 0)
                        {
							DrawLevelUpdated(currLevel, updatedCells);

							// NOTE: This is put here on purpose as currently
							// the inventory can't be updated unless SOMETHING moves
							// But hypothetically, the games design could change
							// and these inventory draws would need to be moved 
							// out of the if
							DrawInvMoneyUpdated(player.GetInventory());
							DrawInvBombsUpdated(player.GetInventory());
							DrawInvKeysUpdated(player.GetInventory());
                        }
					}

					updatedCells.Clear();
				}

				++currLevelNum;
			}
		}






		static string[] LoadLevelList(in string levelListFileName, ref int numberOfLevels)
		{
			FileStream fileStream = new FileStream(levelListFileName, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);
	
			numberOfLevels = int.Parse(streamReader.ReadLine());
			string[] levelList = new string[numberOfLevels];
			for (int index = 0; index<numberOfLevels; ++index)
				levelList[index] = streamReader.ReadLine() + ".txt";
	
			return levelList;
		}

		static Enums.Action GetActionInput(int numOfBombs)
		{
			Enums.Action action = Enums.Action.SELECTING;
			bool actionReady = false;

			do
			{
				// Passing true to the ReadKey function
				// Stops it from printing the pressed key to the console
				ConsoleKey keyPressed = Console.ReadKey(true).Key;
				switch (keyPressed)
				{
					case ConsoleKey.W:
						action = Enums.Action.MOVE_UP;
						break;
					case ConsoleKey.A:
						action = Enums.Action.MOVE_LEFT;
						break;
					case ConsoleKey.S:
						action = Enums.Action.MOVE_DOWN;
						break;
					case ConsoleKey.D:
						action = Enums.Action.MOVE_RIGHT;
						break;
					case ConsoleKey.B:
						{
							if (numOfBombs > 0)
							{
								bool bombActionSelected = false;
								while (bombActionSelected == false)
								{
									keyPressed = Console.ReadKey(true).Key;
									switch (keyPressed)
									{
										case ConsoleKey.W:
											{
												action = Enums.Action.PLACE_LIT_BOMB_UP;
												bombActionSelected = true;
											}
											break;
										case ConsoleKey.A:
											{
												action = Enums.Action.PLACE_LIT_BOMB_LEFT;
												bombActionSelected = true;
											}
											break;
										case ConsoleKey.S:
											{
												action = Enums.Action.PLACE_LIT_BOMB_DOWN;
												bombActionSelected = true;
											}
											break;
										case ConsoleKey.D:
											{
												action = Enums.Action.PLACE_LIT_BOMB_RIGHT;
												bombActionSelected = true;
											}
											break;
										case ConsoleKey.Escape:
											bombActionSelected = true; //no action is selected, thus the primary input loop will continue
											break;
									}
								}
							}
						}
						break;
					case ConsoleKey.R:
						action = Enums.Action.RESTART;
						break;
				} //end of primary switch

				if (action != Enums.Action.SELECTING)
					actionReady = true;
			} while (actionReady == false);

			return action;
		}







		static void DrawInventoryInitial(in Inventory pInventory)
		{
			Console.Write("Inventory\n");
			Console.Write("---------\n");
			Console.Write($"${pInventory.money}\n");
			if (pInventory.bombs == 1)
				Console.Write(" 1 Bomb\n");
			else
				Console.Write($" {pInventory.bombs} Bombs\n");

			Console.Write("Keys: ");
			for (int count = 0; count < Constants.MAX_NUM_KEYS; count++)
			{
				switch (pInventory.keys[count])
				{
					case Obj.KEY1:
						Console.Write('d');
						break;
					case Obj.KEY2:
						Console.Write('f');
						break;
					case Obj.KEY3:
						Console.Write('h');
						break;
					case Obj.EMPTY_SLOT:
						Console.Write('_');
						break;
				}
				Console.Write(' ');
			}
			Console.Write("\n\n");
		}

		/*
		static void drawInventoryUpdated(in bool[] updatedInvItems, in Inventory pInventory)
        {
			if (updatedInvItems[0])
				drawInvMoney(pInventory);
			if (updatedInvItems[1])
				drawInvBombs(pInventory);
			if (updatedInvItems[2])
				drawInvKeys(pInventory);
		}
		*/

		static void DrawInvMoneyUpdated(in Inventory pInventory)
        {
			Console.SetCursorPosition(1, 2);
			Console.Write(pInventory.money);
		}

		static void DrawInvBombsUpdated(in Inventory pInventory)
        {
			if (pInventory.bombs == 1)
            {
				Console.SetCursorPosition(1, 3);
				Console.Write('1');					// Print 1 for the number of bombs
				Console.SetCursorPosition(7, 3);
				Console.Write(' ');					// Remove the s from bombs
			}
			else
            {
				Console.SetCursorPosition(1, 3);
				Console.Write(pInventory.bombs);    // Print the number of bombs
				Console.SetCursorPosition(7, 3);
				Console.Write('s');					// Put the s on bombs (incase it was removed)
			}
		}

		// Possible TODO: Remake this function for the purposes of only redrawing the keys we have to
		static void DrawInvKeysUpdated(in Inventory pInventory)
        {
			Console.SetCursorPosition(6, 4);
			for (int count = 0; count < Constants.MAX_NUM_KEYS; count++)
			{
				switch (pInventory.keys[count])
				{
					case Obj.KEY1:
						Console.Write('d');
						break;
					case Obj.KEY2:
						Console.Write('f');
						break;
					case Obj.KEY3:
						Console.Write('h');
						break;
					case Obj.EMPTY_SLOT:
						Console.Write('_');
						break;
				}
				Console.Write(' ');
			}
		}


		static void DrawLevelInitial(in Level level)
		{
			for (int hCount = 0; hCount < level.GetLevelDim().y; hCount++)
			{
				for (int wCount = 0; wCount < level.GetLevelDim().x; wCount++)
				{
					DrawObjAtCurrCursorPos(level.GetObjAtPos(wCount, hCount));
					Console.Write(' ');
				}
				Console.Write('\n');
			}
		}

		static void DrawLevelUpdated(in Level currLevel, in List<OrdPair> updatedCells)
        {
			foreach (OrdPair cell in updatedCells)
            {
				Console.SetCursorPosition(cell.x * 2, cell.y + Constants.LEVEL_START_Y_POS);
				DrawObjAtCurrCursorPos(currLevel.GetObjAtPos(cell.x, cell.y));
            }
        }

		static void DrawObjAtCurrCursorPos(Obj objToDraw)
        {
			switch (objToDraw)
			{
				case Obj.SPACE:
					Console.Write(' ');
					break;
				case Obj.WALL:
					Console.Write('*');
					break;
				case Obj.PLAYER:
					Console.Write('@');
					break;
				case Obj.GOAL:
					Console.Write('G');
					break;
				case Obj.ROCK:
					Console.Write('#');
					break;
				case Obj.HOLE:
					Console.Write('O');
					break;
				case Obj.PANEL_VERT:
					Console.Write('-');
					break;
				case Obj.PANEL_HORIZ:
					Console.Write('|');
					break;
				case Obj.BOMB_PICKUP:
					Console.Write('o');
					break;
				case Obj.BOMB_LIT:
					Console.Write('!');
					break;
				case Obj.BOMB_EXPLOSION:
					Console.Write("X");
					break;
				case Obj.MONEY:
					Console.Write('$');
					break;
				case Obj.KEY1:
					Console.Write('d');
					break;
				case Obj.DOOR1:
					Console.Write('D');
					break;
				case Obj.KEY2:
					Console.Write('f');
					break;
				case Obj.DOOR2:
					Console.Write('F');
					break;
				case Obj.KEY3:
					Console.Write('h');
					break;
				case Obj.DOOR3:
					Console.Write('H');
					break;
			}
		}
    }
}
