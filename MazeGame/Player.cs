using MazeGame.Enums;
using MazeGame.Structs;

namespace MazeGame
{
	class Player
	{
		private Inventory _inventory;
		private Inventory _backupInventory;

		private OrdPair _currPos;
		private OrdPair _litBombPos;

		public Action Action { get; private set; }

		public Player()
		{
			_inventory = new Inventory
			{ 
				money = 0, 
				bombs = 0, 
				keys = new Obj[Constants.MAX_NUM_KEYS] { Obj.EMPTY_SLOT, Obj.EMPTY_SLOT, Obj.EMPTY_SLOT, Obj.EMPTY_SLOT, Obj.EMPTY_SLOT } 
			};
			Action = Action.SELECTING;
		}

		public void BackupInventory()
        {
			_backupInventory = new Inventory
			{
				money = _inventory.money,
				bombs = _inventory.bombs,
				keys = (Obj[])_inventory.keys.Clone()
			};
		}

		public void RestoreInventory()
        {
			_inventory = _backupInventory;
		}

		public bool AddKey(in Obj keyToAdd)
		{
			bool hasRoom = false;
			for (int arrayPos = 0; ((arrayPos < Constants.MAX_NUM_KEYS) && (!hasRoom)); ++arrayPos)
			{
				if (_inventory.keys[arrayPos] == Obj.EMPTY_SLOT)
				{
					_inventory.keys[arrayPos] = keyToAdd;
					hasRoom = true;
				}
			}
			return hasRoom;
		}


		public bool UseKey(in Obj doorToUnlock)
		{
			foreach (Obj key in _inventory.keys)
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

				if (_inventory.keys[arrayPos] == keyToUse)
				{
					_inventory.keys[arrayPos] = Obj.EMPTY_SLOT;
					hasKey = true;
				}
			}
			return hasKey;
		}

		public void CompleteMove()
		{
			SetCurrPos(GetAttPos());
			Action = Action.SELECTING;
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
						Action = inAction;
					}
				}
					break;
				default:
				{
					Action = inAction;
					actionSet = true;
				}
					break;
			}
			return actionSet;
		}

		public OrdPair GetAttPos()
		{
			OrdPair attPos;
			switch (Action)
			{
				case Action.MOVE_UP:
				{
					attPos.y = _currPos.y - 1;
					attPos.x = _currPos.x;
					break;
				}
				case Action.MOVE_LEFT:
				{
					attPos.x = _currPos.x - 1;
					attPos.y = _currPos.y;
					break;
				}
				case Action.MOVE_DOWN:
				{
					attPos.y = _currPos.y + 1;
					attPos.x = _currPos.x;
					break;
				}
				case Action.MOVE_RIGHT:
				{
					attPos.x = _currPos.x + 1;
					attPos.y = _currPos.y;
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
			if (_inventory.bombs > 0)
				return true;
			else
				return false;
		}

		private bool PrepareLitBomb(Action inAction)
		{
			bool hasABomb = false;
			if (_inventory.bombs > 0)
			{
				--_inventory.bombs;
				hasABomb = true;

				switch (inAction)
				{
					case Action.PLACE_LIT_BOMB_UP:
					{
						_litBombPos.y = _currPos.y - 1;
						_litBombPos.x = _currPos.x;
						break;
					}
					case Action.PLACE_LIT_BOMB_LEFT:
					{
						_litBombPos.x = _currPos.x - 1;
						_litBombPos.y = _currPos.y;
						break;
					}
					case Action.PLACE_LIT_BOMB_DOWN:
					{
						_litBombPos.y = _currPos.y + 1;
						_litBombPos.x = _currPos.x;
						break;
					}
					case Action.PLACE_LIT_BOMB_RIGHT:
					{
						_litBombPos.x = _currPos.x + 1;
						_litBombPos.y = _currPos.y;
						break;
					}
				}
			}
			return hasABomb;
		}

		public void AddBomb()
		{ 
			_inventory.bombs++; 
		}
		
		public void AddMoney()
		{ 
			_inventory.money++; 
		}


		//Getters and Setters for private structs

		public Inventory GetInventory()
		{
			return _inventory;
		}

		/*
		public void setInventory(in Inventory inventory)
		{
			this.inventory = inventory;
		}
		*/

		public OrdPair GetCurrPos()
		{
			return _currPos;
		}
		public void SetCurrPos(in OrdPair currPos)
		{
			this._currPos = currPos;
		}

		public OrdPair GetLitBombPos()
        {
			return _litBombPos;
        }
	}
}
