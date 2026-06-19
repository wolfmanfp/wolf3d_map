using System.Collections.Generic;

namespace wolf3d_map
{
    class GameConfig
    {
        public string name;
        public uint addr_map;
        public uint addr_pos_x;
        public uint addr_pos_y;
        public List<ushort> empty_tiles = new List<ushort>();
        public List<ushort> door_tiles = new List<ushort>();
        public List<ushort> block_tiles = new List<ushort>();
        public List<ushort> secret_door_tiles = new List<ushort>();
    }
}
