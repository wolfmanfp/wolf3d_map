using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Globalization;

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

        private string dosbox_procname;
        private uint dosbox_baseaddr;

        private MemoryEdit.Memory mem;

        private Brush[] colors;
        private List<GameConfig> game_list = new List<GameConfig>();
        GameConfig game_sel;
        private Process game;

        public Form1()
        {
            InitializeComponent();
            try
            {
                using (StreamReader sr = new StreamReader(cfg_file))
                {
                    colors = Array.ConvertAll(sr.ReadLine().Split(','), x => ProcessColor(x));
                    dosbox_procname = sr.ReadLine();
                    dosbox_baseaddr = uint.Parse(sr.ReadLine(), NumberStyles.HexNumber);
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
                        ProcessData(tmp.secret_door_tiles, sr.ReadLine().Split(','));
                        game_list.Add(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading settings:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            mem = new MemoryEdit.Memory();
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
            //Get addresses
            uint addr_base = (uint)mem.Read(dosbox_baseaddr);
            //Wolf3d
            /*uint addr_map = addr_base + 0x280E0;
            uint addr_pos_x = addr_base + 0x46824;
            uint addr_pos_y = addr_base + 0x46826;
            //Tristania 3d
            uint addr_map = addr_base + 0x2B140;
            uint addr_pos_x = addr_base + 0x4AB21;
            uint addr_pos_y = addr_base + 0x4AB22;*/
            //
            uint addr_map = addr_base + game_sel.addr_map;
            uint addr_pos_x = addr_base + game_sel.addr_pos_x;
            uint addr_pos_y = addr_base + game_sel.addr_pos_y;
            //Get map data
            byte[] tmp_data = mem.ReadBytes(addr_map, MAP_SIZE_DATA * 4);
            //Get player pos
            byte pos_x = mem.ReadByte(addr_pos_x);
            byte pos_y = mem.ReadByte(addr_pos_y);
            //Process map data
            ushort[] map_data = new ushort[MAP_SIZE_DATA * 2];
            for (int i = 0; i < MAP_SIZE_DATA * 2; i++)
            {
                map_data[i] = BitConverter.ToUInt16(tmp_data, i * 2);
            }
            //Draw map
            Graphics gfx = e.Graphics;
            gfx.FillRectangle(colors[0], START_X, START_Y, 512, 512);
            for (int i = 0; i < MAP_SIZE_DATA; i++)
            {
                ushort tmp = map_data[i];
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
                ushort tmp = map_data[MAP_SIZE_DATA + i];
                bool obj = game_sel.block_tiles.Contains(tmp);
                bool secret_door = game_sel.secret_door_tiles.Contains(tmp);
                if (obj)
                    DrawBlock(gfx, colors[3], i);
                if (secret_door)
                    DrawBlock(gfx, colors[2], i);
            }
            //Draw player
            gfx.FillRectangle(colors[4], START_X + pos_x * BLOCK_SIZE, START_Y + pos_y * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
            base.OnPaint(e);
        }

        private void DrawBlock(Graphics gfx, Brush col, int i)
        {
            gfx.FillRectangle(col, START_X + (i % MAP_SIZE) * BLOCK_SIZE, START_Y + (i / MAP_SIZE) * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE);
        }

        private void cb_game_SelectedIndexChanged(object sender, EventArgs e)
        {
            game_sel = game_list[cb_game.SelectedIndex];
        }

        private void tmr_refresh_Tick(object sender, EventArgs e)
        {
            if (game == null || game.HasExited)
            {
                ScanForGame();
                return;
            }
            Invalidate();
        }

        private void ScanForGame()
        {
            Process[] procs = Process.GetProcessesByName(dosbox_procname);
            if (procs.Length > 0)
            {
                game = procs[0];
                mem.Attach((uint)game.Id, MemoryEdit.Memory.ProcessAccessFlags.All);
            }
        }
    }
}
