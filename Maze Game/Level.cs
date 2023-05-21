using MazeGame.Enums;
using MazeGame.Structs;
using System.IO;
using System.Collections.Generic;

namespace MazeGame
{
    class Level
    {
		Player player;
		List<OrdPair> updatedCells;
		bool[] updatedInvItems;
		
		Inventory backupInventory;
		Obj[] levelClean;
		Obj[] levelActive;
		
		OrdPair levelDim = new OrdPair( 0, 0 );

		const int EXPLOSION_RADIUS = 1;

		public Level(Player player, List<OrdPair> updatedCells, bool[] updatedInvItems)
		{
			this.player = player;
			this.updatedCells = updatedCells;
			this.updatedInvItems = updatedInvItems;
		}

		
		public void loadLevelFile(in string fileName)
		{
			backupInventory = player.getInventory();
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);

			string lineRead = streamReader.ReadLine();
			string[] substrings = lineRead.Split(' ');
			levelDim.x = int.Parse(substrings[0]);
			levelDim.y = int.Parse(substrings[1]);

			levelClean = new Obj[levelDim.x * levelDim.y];
			levelActive = new Obj[levelDim.x * levelDim.y];

			for (int hCount = 0; hCount < levelDim.y; hCount++)
			{
				lineRead = streamReader.ReadLine();
				substrings = lineRead.Split(' ');
				for (int wCount = 0; wCount < levelDim.x; wCount++)
				{
					string inputTag = substrings[wCount];
					switch (inputTag)
                    {
						case "___":
							levelClean[hCount * levelDim.x + wCount] = Obj.SPACE;
							break;
						case "*,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.WALL;
							break;
						case "@,,":
							{
								levelClean[hCount * levelDim.x + wCount] = Obj.PLAYER;
								OrdPair pCurrPos;
								pCurrPos.y = hCount;
								pCurrPos.x = wCount;
								player.setCurrPos(pCurrPos);
							}
							break;
						case "G,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.GOAL;
							break;
						case "#,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.ROCK;
							break;
						case "O,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.HOLE;
							break;
						case "o,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.BOMB_PICKUP;
							break;
						case "-,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.PANEL_VERT;
							break;
						case "|,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.PANEL_HORIZ;
							break;
						case "$,,":
							levelClean[hCount * levelDim.x + wCount] = Obj.MONEY;
							break;
						case "K1,":
							levelClean[hCount * levelDim.x + wCount] = Obj.KEY1;
							break;
						case "D1,":
							levelClean[hCount * levelDim.x + wCount] = Obj.DOOR1;
							break;
						case "K2,":
							levelClean[hCount * levelDim.x + wCount] = Obj.KEY1;
							break;
						case "D2,":
							levelClean[hCount * levelDim.x + wCount] = Obj.DOOR1;
							break;
					}
					levelActive[hCount * levelDim.x + wCount] = levelClean[hCount * levelDim.x + wCount];
				}
			}
			streamReader.Close();
		}
		

		public void restartLevel()
		{
			player.setInventory(backupInventory);
			for (int hCount = 0; hCount < levelDim.y; hCount++)
			{
				for (int wCount = 0; wCount < levelDim.x; wCount++)
				{
					levelActive[(hCount * levelDim.x) + wCount] = levelClean[(hCount * levelDim.x) + wCount];
					if (levelClean[(hCount * levelDim.x) + wCount] == Obj.PLAYER)
					{
						OrdPair pCurrPos;
						pCurrPos.y = hCount;
						pCurrPos.x = wCount;
						player.setCurrPos(pCurrPos);
					}
				}
			}
		}

		public GameState updateLevel()
		{
			GameState gameState = GameState.IN_PROGRESS;

			switch (player.action)
			{
				case Action.PLACE_LIT_BOMB_UP:
				case Action.PLACE_LIT_BOMB_LEFT:
				case Action.PLACE_LIT_BOMB_DOWN:
				case Action.PLACE_LIT_BOMB_RIGHT:
					{
						bombExplosion(player.getLitBombPos());
					}
					break;
				default:
					{
						switch (levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x])
						{
							case Obj.SPACE:
								moveEntity();
								break;
							case Obj.MONEY:
								{
									player.addMoney();
									levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x] = Obj.SPACE;
									moveEntity();
									//updatedInvItems[]
									break;
								}
							case Obj.ROCK:
								{
									bool movedRock = pushRock(player.getCurrPos(), player.getAttPos());
									if (movedRock)
										moveEntity();
									else
										player.setAction(Action.WAIT);
									break;
								}
							case Obj.BOMB_PICKUP:
								{
									player.addBomb();
									levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x] = Obj.SPACE;
									moveEntity();
									break;
								}
							case Obj.KEY1:
							case Obj.KEY2:
								{
									bool keyGrabbed = (player.addKey(levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x]));
									if (keyGrabbed)
									{
										levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x] = Obj.SPACE;
										moveEntity();
									}
									else
									{
										//cout << "Not enough space!\n";
										player.setAction(Action.WAIT);
									}
									break;
								}
							case Obj.DOOR1:
							case Obj.DOOR2:
								{
									bool doorUnlocked = (player.useKey(levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x]));
									if (doorUnlocked)
									{
										levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x] = Obj.SPACE;
										moveEntity();
									}
									else
									{
										//cout << "You don't have the proper key to open the door.\n";
										player.setAction(Action.WAIT);
									}

									break;
								}
							case Obj.GOAL:
								gameState = GameState.LEVEL_COMPLETE;
								break;
						}
					}
					break;
			}
			return gameState;
		}

		private void moveEntity()
		{
			updatedCells.Add(new OrdPair(player.getCurrPos().x, player.getCurrPos().y));
			updatedCells.Add(new OrdPair(player.getAttPos().x, player.getAttPos().y));
			levelActive[player.getAttPos().y * levelDim.x + player.getAttPos().x] = levelActive[player.getCurrPos().y * levelDim.x + player.getCurrPos().x];
			levelActive[player.getCurrPos().y * levelDim.x + player.getCurrPos().x] = Obj.SPACE;
			player.completeMove();
		}

		/*
		private void moveObject(in OrdPair oCurrPos, in OrdPair oAttPos)
		{
			//maybe add if
			updatedCells.Add(new OrdPair(oCurrPos.x, oCurrPos.y));
			updatedCells.Add(new OrdPair(oAttPos.x, oAttPos.y));
			Obj tempObj = levelActive[oCurrPos.y * levelDim.x + oCurrPos.x];
			levelActive[oCurrPos.y * levelDim.x + oCurrPos.x] = levelActive[oAttPos.y * levelDim.x + oAttPos.x];
			levelActive[oAttPos.y * levelDim.x + oAttPos.x] = tempObj;
		}
		*/

		private bool pushRock(in OrdPair pCurrPos, in OrdPair pAttPos)
		{
			OrdPair rCurrPos = new OrdPair(pAttPos.x, pAttPos.y);
			OrdPair rAttPos;
			if (pAttPos.y - pCurrPos.y <= -1) //If player is pushing up, set rock to move up
			{
				rAttPos.y = rCurrPos.y - 1;
				rAttPos.x = rCurrPos.x;
			}
			else if (pAttPos.x - pCurrPos.x <= -1) //If player is pushing left, set rock to move left
			{
				rAttPos.x = rCurrPos.x - 1;
				rAttPos.y = rCurrPos.y;
			}
			else if (pAttPos.y - pCurrPos.y >= 1) //If player is pushing down, set rock to move down
			{
				rAttPos.y = rCurrPos.y + 1;
				rAttPos.x = rCurrPos.x;
			}
			else //if (pAttPos.x - pCurrPos.x >= 1) //If player is pushing right, set rock to move right
			{
				rAttPos.x = rCurrPos.x + 1;
				rAttPos.y = rCurrPos.y;
			}

			bool movedRock = false;

			switch (levelActive[rAttPos.y * levelDim.x + rAttPos.x])
			{
				case Obj.SPACE:
				case Obj.MONEY:
				case Obj.BOMB_PICKUP:
				case Obj.KEY1:
				case Obj.KEY2:
					{
						updatedCells.Add(new OrdPair(rAttPos.x, rAttPos.y));
						levelActive[rAttPos.y * levelDim.x + rAttPos.x] = Obj.ROCK;
						levelActive[rCurrPos.y * levelDim.x + rCurrPos.x] = Obj.SPACE;
						movedRock = true;
						break;
					}
				case Obj.HOLE:
					{
						updatedCells.Add(new OrdPair(rAttPos.x, rAttPos.y));
						levelActive[rAttPos.y * levelDim.x + rAttPos.x] = Obj.SPACE;
						levelActive[rCurrPos.y * levelDim.x + rCurrPos.x] = Obj.SPACE;
						movedRock = true;
						break;
					}
			}

			return movedRock;
		}

		private void bombExplosion(in OrdPair bPos)
		{
			for (int yIndex = (bPos.y - 1); (yIndex - bPos.y) <= EXPLOSION_RADIUS; ++yIndex)
			{
				for (int xIndex = (bPos.x - 1); (xIndex - bPos.x) <= EXPLOSION_RADIUS; ++xIndex)
				{
					switch (levelActive[yIndex * levelDim.x + xIndex])
					{
						case Obj.PANEL_HORIZ:
						case Obj.PANEL_VERT:
							updatedCells.Add(new OrdPair(xIndex, yIndex));
							levelActive[yIndex * levelDim.x + xIndex] = Obj.SPACE;
							break;
					}
				}
			}
		}

		public OrdPair getLevelDim()
        {
			return levelDim;
        }

		public Obj getObjAtPos(in int xCoord, in int yCoord)
        {
			return levelActive[levelDim.x * yCoord + xCoord];
        }
	}
}
