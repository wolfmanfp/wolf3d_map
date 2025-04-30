namespace wolf3d_map
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmr_refresh = new System.Windows.Forms.Timer(this.components);
            this.cb_game = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tmr_refresh
            // 
            this.tmr_refresh.Interval = 200;
            this.tmr_refresh.Tick += new System.EventHandler(this.tmr_refresh_Tick);
            // 
            // cb_game
            // 
            this.cb_game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_game.FormattingEnabled = true;
            this.cb_game.Location = new System.Drawing.Point(43, 2);
            this.cb_game.Name = "cb_game";
            this.cb_game.Size = new System.Drawing.Size(152, 21);
            this.cb_game.TabIndex = 0;
            this.cb_game.SelectedIndexChanged += new System.EventHandler(this.cb_game_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 537);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_game);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wolfenstein 3D Map for DOSBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_game;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmr_refresh;
    }
}

