using MazeGame.Enums;
using MazeGame.Structs;

namespace MazeGame
{
	class Player
	{
		private Inventory inventory;
		private OrdPair currPos;
		private OrdPair litBombPos;

		public Action action { get; private set; }

		public Player()
		{
			inventory = new Inventory
			{ 
				money = 0, 
				bombs = 0, 
				keys = new Obj[Constants.MAX_NUM_KEYS] { Obj.EMPTY_SLOT, Obj.EMPTY_SLOT, Obj.EMPTY_SLOT, Obj.EMPTY_SLOT, Obj.EMPTY_SLOT } 
			};
			action = Action.SELECTING;
		}



		public bool addKey(in Obj keyToAdd)
		{
			bool hasRoom = false;
			for (int arrayPos = 0; ((arrayPos < Constants.MAX_NUM_KEYS) && (!hasRoom)); ++arrayPos)
			{
				if (inventory.keys[arrayPos] == Obj.EMPTY_SLOT)
				{
					inventory.keys[arrayPos] = keyToAdd;
					hasRoom = true;
				}
			}
			return hasRoom;
		}


		public bool useKey(in Obj doorToUnlock)
		{
			bool hasKey = false;
			Obj keyToUse;
			switch (doorToUnlock)
			{
				case Obj.DOOR1:
					keyToUse = Obj.KEY1;
					break;
				case Obj.DOOR2:
					keyToUse = Obj.KEY2;
					break;
				default:
					//better ex: throw new ArgumentException("Index is out of range", nameof(index), ex);
					throw new System.Exception();
			}

			for (int arrayPos = 0; ((arrayPos < Constants.MAX_NUM_KEYS) && !hasKey); ++arrayPos)
			{

				if (inventory.keys[arrayPos] == keyToUse)
				{
					inventory.keys[arrayPos] = Obj.EMPTY_SLOT;
					hasKey = true;
				}
			}
			return hasKey;
		}

		public void completeMove()
		{
			setCurrPos(getAttPos());
			action = Action.SELECTING;
		}

		public bool setAction(Action inAction)
		{
			bool actionSet = false;
			switch (inAction)
			{
				case Action.PLACE_LIT_BOMB_UP:
				case Action.PLACE_LIT_BOMB_LEFT:
				case Action.PLACE_LIT_BOMB_DOWN:
				case Action.PLACE_LIT_BOMB_RIGHT:
				{
					if (prepareLitBomb(inAction))
					{
						actionSet = true;
						action = inAction;
					}
				}
					break;
				default:
				{
					action = inAction;
					actionSet = true;
				}
					break;
			}
			return actionSet;
		}

		public OrdPair getAttPos()
		{
			OrdPair attPos;
			switch (action)
			{
				case Action.MOVE_UP:
				{
					attPos.y = currPos.y - 1;
					attPos.x = currPos.x;
					break;
				}
				case Action.MOVE_LEFT:
				{
					attPos.x = currPos.x - 1;
					attPos.y = currPos.y;
					break;
				}
				case Action.MOVE_DOWN:
				{
					attPos.y = currPos.y + 1;
					attPos.x = currPos.x;
					break;
				}
				case Action.MOVE_RIGHT:
				{
					attPos.x = currPos.x + 1;
					attPos.y = currPos.y;
					break;
				}
				default:
					//better ex: throw new ArgumentException("Index is out of range", nameof(index), ex);
					throw new System.Exception();
			}
			return attPos;
		}

		bool hasBomb()
		{
			if (inventory.bombs > 0)
				return true;
			else
				return false;
		}

		private bool prepareLitBomb(Action inAction)
		{
			bool hasABomb = false;
			if (inventory.bombs > 0)
			{
				--inventory.bombs;
				hasABomb = true;

				switch (inAction)
				{
					case Action.PLACE_LIT_BOMB_UP:
					{
						litBombPos.y = currPos.y - 1;
						litBombPos.x = currPos.x;
						break;
					}
					case Action.PLACE_LIT_BOMB_LEFT:
					{
						litBombPos.x = currPos.x - 1;
						litBombPos.y = currPos.y;
						break;
					}
					case Action.PLACE_LIT_BOMB_DOWN:
					{
						litBombPos.y = currPos.y + 1;
						litBombPos.x = currPos.x;
						break;
					}
					case Action.PLACE_LIT_BOMB_RIGHT:
					{
						litBombPos.x = currPos.x + 1;
						litBombPos.y = currPos.y;
						break;
					}
				}
			}
			return hasABomb;
		}

		public void addBomb()
		{ 
			inventory.bombs++; 
		}
		
		public void addMoney()
		{ 
			inventory.money++; 
		}


		//Getters and Setters for private structs

		public Inventory getInventory()
		{
			return inventory;
		}
		public void setInventory(in Inventory inventory)
		{
			this.inventory = inventory;
		}

		public OrdPair getCurrPos()
		{
			return currPos;
		}
		public void setCurrPos(in OrdPair currPos)
		{
			this.currPos = currPos;
		}

		public OrdPair getLitBombPos()
        {
			return litBombPos;
        }
	}
}
