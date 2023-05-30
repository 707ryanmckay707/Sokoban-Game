using MazeGame.Enums;
using MazeGame.Structs;
using System.IO;
using System.Collections.Generic;

namespace MazeGame
{
    class Level
    {
		struct Entity
        {
			Obj obj;
			OrdPair currPos;
			OrdPair attPos;
			OrdPair? resPos;
			int strength;
        }

		List<Entity> _entities;

		readonly Player _player;
		readonly List<OrdPair> _updatedCells;
		
		Obj[] _levelClean;
		Obj[] _levelActive;
		
		OrdPair _levelDim = new OrdPair( 0, 0 );

		const int _EXPLOSION_RADIUS = 1;

		public Level(Player player, List<OrdPair> updatedCells)
		{
			_player = player;
			_updatedCells = updatedCells;
		}

		
		public void LoadLevelFile(in string fileName)
		{
			_player.BackupInventory();
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);

			string lineRead = streamReader.ReadLine();
			string[] substrings = lineRead.Split(' ');
			_levelDim.x = int.Parse(substrings[0]);
			_levelDim.y = int.Parse(substrings[1]);

			_levelClean = new Obj[_levelDim.x * _levelDim.y];
			_levelActive = new Obj[_levelDim.x * _levelDim.y];

			for (int hCount = 0; hCount < _levelDim.y; hCount++)
			{
				lineRead = streamReader.ReadLine();
				substrings = lineRead.Split(' ');
				for (int wCount = 0; wCount < _levelDim.x; wCount++)
				{
					string inputTag = substrings[wCount];
					switch (inputTag)
                    {
						case "___":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.SPACE;
							break;
						case "*,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.WALL;
							break;
						case "@,,":
							{
								_levelClean[hCount * _levelDim.x + wCount] = Obj.PLAYER;
								OrdPair pCurrPos;
								pCurrPos.y = hCount;
								pCurrPos.x = wCount;
								_player.SetCurrPos(pCurrPos);
							}
							break;
						case "G,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.GOAL;
							break;
						case "#,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.ROCK;
							break;
						case "O,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.HOLE;
							break;
						case "o,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.BOMB_PICKUP;
							break;
						case "-,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.PANEL_VERT;
							break;
						case "|,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.PANEL_HORIZ;
							break;
						case "$,,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.MONEY;
							break;
						case "K1,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.KEY1;
							break;
						case "D1,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.DOOR1;
							break;
						case "K2,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.KEY2;
							break;
						case "D2,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.DOOR2;
							break;
						case "K3,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.KEY3;
							break;
						case "D3,":
							_levelClean[hCount * _levelDim.x + wCount] = Obj.DOOR3;
							break;
					}
					_levelActive[hCount * _levelDim.x + wCount] = _levelClean[hCount * _levelDim.x + wCount];
				}
			}
			streamReader.Close();
		}
		

		public void RestartLevel()
		{
			_player.RestoreInventory();
			for (int hCount = 0; hCount < _levelDim.y; hCount++)
			{
				for (int wCount = 0; wCount < _levelDim.x; wCount++)
				{
					_levelActive[(hCount * _levelDim.x) + wCount] = _levelClean[(hCount * _levelDim.x) + wCount];
					if (_levelClean[(hCount * _levelDim.x) + wCount] == Obj.PLAYER)
					{
						OrdPair pCurrPos;
						pCurrPos.y = hCount;
						pCurrPos.x = wCount;
						_player.SetCurrPos(pCurrPos);
					}
				}
			}
		}

		public GameState UpdateLevel()
		{
			GameState gameState = GameState.IN_PROGRESS;

			switch (_player.Action)
			{
				case Action.PLACE_LIT_BOMB_UP:
				case Action.PLACE_LIT_BOMB_LEFT:
				case Action.PLACE_LIT_BOMB_DOWN:
				case Action.PLACE_LIT_BOMB_RIGHT:
					{
						BombExplosion(_player.GetLitBombPos());
					}
					break;
				default:
					{
						switch (_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x])
						{
							case Obj.SPACE:
								MovePlayer();
								break;
							case Obj.MONEY:
								{
									_player.AddMoney();
									_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x] = Obj.SPACE;
									MovePlayer();
									break;
								}
							case Obj.ROCK:
								{
									bool movedRock = PushRock(_player.GetCurrPos(), _player.GetAttPos());
									if (movedRock)
										MovePlayer();
									else
										_player.SetAction(Action.WAIT);
									break;
								}
							case Obj.BOMB_PICKUP:
								{
									_player.AddBomb();
									_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x] = Obj.SPACE;
									MovePlayer();
									break;
								}
							case Obj.KEY1:
							case Obj.KEY2:
							case Obj.KEY3:
								{
									bool keyGrabbed = (_player.AddKey(_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x]));
									if (keyGrabbed)
									{
										_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x] = Obj.SPACE;
										MovePlayer();
									}
									else
									{
										//cout << "Not enough space!\n";
										_player.SetAction(Action.WAIT);
									}
									break;
								}
							case Obj.DOOR1:
							case Obj.DOOR2:
							case Obj.DOOR3:
								{
									bool doorUnlocked = (_player.UseKey(_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x]));
									if (doorUnlocked)
									{
										_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x] = Obj.SPACE;
										MovePlayer();
									}
									else
									{
										//cout << "You don't have the proper key to open the door.\n";
										_player.SetAction(Action.WAIT);
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

		private void MovePlayer()
		{
			_updatedCells.Add(new OrdPair(_player.GetCurrPos().x, _player.GetCurrPos().y));
			_updatedCells.Add(new OrdPair(_player.GetAttPos().x, _player.GetAttPos().y));
			_levelActive[_player.GetAttPos().y * _levelDim.x + _player.GetAttPos().x] = _levelActive[_player.GetCurrPos().y * _levelDim.x + _player.GetCurrPos().x];
			_levelActive[_player.GetCurrPos().y * _levelDim.x + _player.GetCurrPos().x] = Obj.SPACE;
			_player.CompleteMove();
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

		private bool PushRock(in OrdPair pCurrPos, in OrdPair pAttPos)
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

			switch (_levelActive[rAttPos.y * _levelDim.x + rAttPos.x])
			{
				case Obj.SPACE:
				case Obj.MONEY:
				case Obj.BOMB_PICKUP:
				case Obj.KEY1:
				case Obj.KEY2:
				case Obj.KEY3:
					{
						_updatedCells.Add(new OrdPair(rAttPos.x, rAttPos.y));
						_levelActive[rAttPos.y * _levelDim.x + rAttPos.x] = Obj.ROCK;
						_levelActive[rCurrPos.y * _levelDim.x + rCurrPos.x] = Obj.SPACE;
						movedRock = true;
						break;
					}
				case Obj.HOLE:
					{
						_updatedCells.Add(new OrdPair(rAttPos.x, rAttPos.y));
						_levelActive[rAttPos.y * _levelDim.x + rAttPos.x] = Obj.SPACE;
						_levelActive[rCurrPos.y * _levelDim.x + rCurrPos.x] = Obj.SPACE;
						movedRock = true;
						break;
					}
			}

			return movedRock;
		}

		private void BombExplosion(in OrdPair bPos)
		{
			for (int yIndex = (bPos.y - 1); (yIndex - bPos.y) <= _EXPLOSION_RADIUS; ++yIndex)
			{
				for (int xIndex = (bPos.x - 1); (xIndex - bPos.x) <= _EXPLOSION_RADIUS; ++xIndex)
				{
					switch (_levelActive[yIndex * _levelDim.x + xIndex])
					{
						case Obj.PANEL_HORIZ:
						case Obj.PANEL_VERT:
						case Obj.MONEY:
						case Obj.KEY1:
						case Obj.KEY2:
						case Obj.KEY3:
							_updatedCells.Add(new OrdPair(xIndex, yIndex));
							_levelActive[yIndex * _levelDim.x + xIndex] = Obj.SPACE;
							break;
					}
				}
			}
		}

		public OrdPair GetLevelDim()
        {
			return _levelDim;
        }

		public Obj GetObjAtPos(in int xCoord, in int yCoord)
        {
			return _levelActive[_levelDim.x * yCoord + xCoord];
        }
	}
}
