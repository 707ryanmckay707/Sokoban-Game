using MazeGame.Enums;

namespace MazeGame.Structs
{
    struct Inventory
    {
        public int money;
        public int bombs;
        public Obj[] keys;
        public Inventory(int money, int bombs, Obj[] keys)
        {
            this.money = money;
            this.bombs = bombs;
            this.keys = keys;
        }
    }
}
