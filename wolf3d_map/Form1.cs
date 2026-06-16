using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wolf3d_map
{
    public partial class Form1 : Form
    {
        private const string cfg_file = "wolf3d_map.ini";

        private const int MAP_SIZE = 64;
        private const int MAP_SIZE_DATA = MAP_SIZE * MAP_SIZE;
        private const int BLOCK_SIZE = 8;
        private const int START_X = 0;
        private const int START_Y = 25;

        private string dosbox_host;
        private int dosbox_port;

        private DosBoxApi api;
        private MapData mapData;

        private Brush[] colors;
        private List<GameConfig> game_list = new List<GameConfig>();
        GameConfig game_sel;

        public Form1()
        {
            InitializeComponent();
            try
            {
                using (StreamReader sr = new StreamReader(cfg_file))
                {
                    colors = Array.ConvertAll(sr.ReadLine().Split(','), x => ProcessColor(x));
                    dosbox_host = sr.ReadLine();
                    dosbox_port = int.Parse(sr.ReadLine());
                    
                    while (sr.Peek() > -1)
                    {
                        GameConfig tmp = new GameConfig();
                        tmp.name = sr.ReadLine();
                        tmp.addr_map = uint.Parse(sr.ReadLine(), NumberStyles.HexNumber);
                        tmp.addr_pos_x = uint.Parse(sr.ReadLine(), NumberStyles.HexNumber);
                        tmp.addr_pos_y = uint.Parse(sr.ReadLine(), NumberStyles.HexNumber);
                        ProcessData(tmp.empty_tiles, sr.ReadLine().Split(','));
                        ProcessData(tmp.door_tiles, sr.ReadLine().Split(','));
                        ProcessData(tmp.block_tiles, sr.ReadLine().Split(','));
                        game_list.Add(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading settings:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            api = new DosBoxApi(dosbox_host, dosbox_port);
            mapData = new MapData
            {
                map_data = new ushort[MAP_SIZE_DATA * 2],
                pos_x = 0,
                pos_y = 0
            };
            cb_game.Items.AddRange(game_list.Select(x => x.name).ToArray());
            cb_game.SelectedIndex = 0;
            tmr_refresh.Enabled = true;
        }

        private Brush ProcessColor(string color)
        {
            return new SolidBrush(Color.FromArgb(int.Parse("FF" + color, NumberStyles.HexNumber)));
        }

        private void ProcessData(List<ushort> list, string[] input)
        {
            foreach (string data in input)
            {
                string[] tmp = data.Split('-');
                if (tmp.Length == 2)
                {
                    ushort min = ushort.Parse(tmp[0]);
                    ushort max = ushort.Parse(tmp[1]);
                    for (ushort i = min; i <= max; i++) list.Add(i);
                }
                list.Add(ushort.Parse(tmp[0]));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawMap(e.Graphics);
            base.OnPaint(e);
        }

        private async Task ReadFromMemory()
        {
            //Get map data
            byte[] tmp_data = await api.ReadMemory(game_sel.addr_map, MAP_SIZE_DATA * 4);
            //Get player pos
            byte pos_x = await api.ReadByte(game_sel.addr_pos_x);
            byte pos_y = await api.ReadByte(game_sel.addr_pos_y);
            //Process map data
            ushort[] map_data = new ushort[MAP_SIZE_DATA * 2];
            for (int i = 0; i < MAP_SIZE_DATA * 2; i++)
            {
                map_data[i] = BitConverter.ToUInt16(tmp_data, i * 2);
            }
            mapData = new MapData
            {
                map_data = map_data,
                pos_x = pos_x,
                pos_y = pos_y
            };
        }

        private void DrawMap(Graphics gfx)
        {
            //Draw map
            gfx.FillRectangle(colors[0], START_X, START_Y, 512, 512);
            for (int i = 0; i < MAP_SIZE_DATA; i++)
            {
                ushort tmp = mapData.map_data[i];
                bool empty = game_sel.empty_tiles.Contains(tmp);
                bool door = game_sel.door_tiles.Contains(tmp);
                if (!empty && !door)
                    DrawBlock(gfx, colors[1], i);
                else if (door)
                    DrawBlock(gfx, colors[2], i);
            }
            //Draw objects
            for (int i = 0; i < MAP_SIZE_DATA; i++)
            {
                ushort tmp = mapData.map_data[MAP_SIZE_DATA + i];
                bool obj = game_sel.block_tiles.Contains(tmp);
                if (obj)
                    DrawBlock(gfx, colors[3], i);
            }
            //Draw player
            gfx.FillRectangle(colors[4], START_X + mapData.pos_x * BLOCK_SIZE, START_Y + mapData.pos_y * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
        }

        private void DrawBlock(Graphics gfx, Brush col, int i)
        {
            gfx.FillRectangle(col, START_X + (i % MAP_SIZE) * BLOCK_SIZE, START_Y + (i / MAP_SIZE) * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
        }

        private void cb_game_SelectedIndexChanged(object sender, EventArgs e)
        {
            game_sel = game_list[cb_game.SelectedIndex];
        }

        private async void tmr_refresh_Tick(object sender, EventArgs e)
        {
            await Task.Run(ReadFromMemory);
            Invalidate();
        }

    }
}
