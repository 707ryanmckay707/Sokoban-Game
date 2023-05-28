using MazeGame.Enums;
using MazeGame.Structs;

namespace MazeGame
{
	class Player
	{
		private Inventory inventory;
		private Inventory backupInventory;

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

		public void BackupInventory()
        {
			backupInventory = new Inventory
			{
				money = inventory.money,
				bombs = inventory.bombs,
				keys = (Obj[])inventory.keys.Clone()
			};
		}

		public void RestoreInventory()
        {
			inventory = backupInventory;
		}

		public bool AddKey(in Obj keyToAdd)
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


		public bool UseKey(in Obj doorToUnlock)
		{
			foreach (Obj key in inventory.keys)
            {
				System.Diagnostics.Debug.Write(key + " ");
            }
			System.Diagnostics.Debug.Write("\n");

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
				case Obj.DOOR3:
					keyToUse = Obj.KEY3;
					break;
				default:
					System.Diagnostics.Debug.WriteLine("ERROR: Door Obj found with no matching Key Obj");
					System.Diagnostics.Debug.WriteLine("       Something is missing from the Obj Enum");
					return false;

					//throw new System.Exception();
					//better ex for reference: throw new ArgumentException("Index is out of range", nameof(index), ex);

			}

			for (int arrayPos = 0; (arrayPos < Constants.MAX_NUM_KEYS) && !hasKey; ++arrayPos)
			{

				if (inventory.keys[arrayPos] == keyToUse)
				{
					inventory.keys[arrayPos] = Obj.EMPTY_SLOT;
					hasKey = true;
				}
			}
			return hasKey;
		}

		public void CompleteMove()
		{
			SetCurrPos(GetAttPos());
			action = Action.SELECTING;
		}

		public bool SetAction(Action inAction)
		{
			bool actionSet = false;
			switch (inAction)
			{
				case Action.PLACE_LIT_BOMB_UP:
				case Action.PLACE_LIT_BOMB_LEFT:
				case Action.PLACE_LIT_BOMB_DOWN:
				case Action.PLACE_LIT_BOMB_RIGHT:
				{
					if (PrepareLitBomb(inAction))
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

		public OrdPair GetAttPos()
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

		bool HasBomb()
		{
			if (inventory.bombs > 0)
				return true;
			else
				return false;
		}

		private bool PrepareLitBomb(Action inAction)
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

		public void AddBomb()
		{ 
			inventory.bombs++; 
		}
		
		public void AddMoney()
		{ 
			inventory.money++; 
		}


		//Getters and Setters for private structs

		public Inventory GetInventory()
		{
			return inventory;
		}

		/*
		public void setInventory(in Inventory inventory)
		{
			this.inventory = inventory;
		}
		*/

		public OrdPair GetCurrPos()
		{
			return currPos;
		}
		public void SetCurrPos(in OrdPair currPos)
		{
			this.currPos = currPos;
		}

		public OrdPair GetLitBombPos()
        {
			return litBombPos;
        }
	}
}
