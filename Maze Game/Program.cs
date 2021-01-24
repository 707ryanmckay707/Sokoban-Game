using System;
using System.IO;
using MazeGame.Enums;
using MazeGame.Structs;

namespace MazeGame
{
    class Program
    {
        static void Main(string[] args)
        {
			insertSpace(400); //Scroll screen to help shaking

			int numOfLevels;
			int currLevelNum = 0;
			string[] levelList = loadLevelList("Level List.txt", ref numOfLevels);

			Player player;
			Level currLevel = new Level(player);

			//Master Game Loop
			while (currLevelNum < numOfLevels)
			{
				GameState gameState = GameState.IN_PROGRESS;
				currLevel.loadLevelFile(levelList[currLevelNum]);

				//Level Loop
				while (gameState != GameState.LEVEL_COMPLETE)
				{
					insertSpace();
					drawInventory(player.getInventory());
					drawLevel(currLevel);

					this_thread.sleep_for(150ms);
					player.setAction(Action.SELECTING);
					player.setAction(getActionInput(player.getInventory().bombs));

					if (player.getAction() == Action.RESTART)
						currLevel.restartLevel();
					else
						gameState = currLevel.updateLevel();
				}

				++currLevelNum;
			}

			if (levelList != 0)
			{
				delete[] levelList;
				levelList = 0;
			}

			return 0;
		}






		static string[] loadLevelList(in string levelListFileName, ref int numberOfLevels)
		{
			FileStream fileStream = new FileStream(levelListFileName, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);
	
			numberOfLevels = int.Parse(streamReader.ReadLine());
			string[] levelList = new string[numberOfLevels];
			for (int index = 0; index<numberOfLevels; ++index)
				levelList[index] = streamReader.ReadLine() + ".txt";
	
			return levelList;
		}

		static Enums.Action getActionInput(int numOfBombs)
		{
			Enums.Action action = Enums.Action.SELECTING;
			bool actionReady = false;

			do
			{
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







		static void drawInventory(in Inventory pInventory)
		{
			Console.Write("Inventory\n");
			Console.Write("---------\n");
			Console.Write($"${pInventory.money}\n");
			if (pInventory.bombs == 1)
				cout << " 1 Bomb\n";
			else
				cout << ' ' << pInventory.bombs << " Bombs\n";

			cout << "Keys: ";
			for (int count = 0; count < MAX_NUM_KEYS; count++)
			{
				switch (pInventory.keys[count])
				{
					case Obj.KEY1:
						cout << 'd';
						break;
					case Obj.KEY2:
						cout << 'f';
						break;
					case Obj.EMPTY_SLOT:
						cout << '_';
						break;
				}
				cout << ' ';
			}
			cout << "\n\n";
		}







		static void drawLevel(const Level& level)
		{
			for (int hCount = 0; hCount < level.getLevelDim().y; hCount++)
			{
				for (int wCount = 0; wCount < level.getLevelDim().x; wCount++)
				{
					switch (level.getObjAtPos(wCount, hCount))
					{
						case Obj.SPACE:
							cout << ' ';
							break;
						case Obj.WALL:
							cout << '*';
							break;
						case Obj.PLAYER:
							cout << '@';
							break;
						case Obj.GOAL:
							cout << 'G';
							break;
						case Obj.ROCK:
							cout << '#';
							break;
						case Obj.HOLE:
							cout << 'O';
							break;
						case Obj.PANEL_VERT:
							cout << '-';
							break;
						case Obj.PANEL_HORIZ:
							cout << '|';
							break;
						case Obj.BOMB_PICKUP:
							cout << 'o';
							break;
						case Obj.MONEY:
							cout << '$';
							break;
						case Obj.KEY1:
							cout << 'd';
							break;
						case Obj.DOOR1:
							cout << 'D';
							break;
						case Obj.KEY2:
							cout << 'f';
							break;
						case Obj.DOOR2:
							cout << 'F';
							break;
					}
					cout << ' ';
				}
				cout << '\n';
			}
		}

		static void insertSpace(int spaceAmt)
		{
			for (int i = 0; i < spaceAmt; i++)
			{
				cout << '\n';
			}
		}
    }
}
