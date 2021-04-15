namespace OlanderIA
{
    partial class UltimateTicTacToe
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UltimateTicTacToe));
            this.MLBoard = new System.Windows.Forms.Button();
            this.LRBoard = new System.Windows.Forms.Button();
            this.ULBoard = new System.Windows.Forms.Button();
            this.MMBoard = new System.Windows.Forms.Button();
            this.LMBoard = new System.Windows.Forms.Button();
            this.UMBoard = new System.Windows.Forms.Button();
            this.MRBoard = new System.Windows.Forms.Button();
            this.URBoard = new System.Windows.Forms.Button();
            this.LLBoard = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Setup = new System.Windows.Forms.Button();
            this.CombinationLevel = new System.Windows.Forms.TextBox();
            this.UltLogBox = new System.Windows.Forms.TextBox();
            this.BoardBox = new System.Windows.Forms.PictureBox();
            this.TempStart = new System.Windows.Forms.Button();
            this.ModeDisplayer = new System.Windows.Forms.Label();
            this.SelectFlag = new System.Windows.Forms.Label();
            this.Winner = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.BoardBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MLBoard
            // 
            this.MLBoard.Location = new System.Drawing.Point(759, 304);
            this.MLBoard.Name = "MLBoard";
            this.MLBoard.Size = new System.Drawing.Size(50, 50);
            this.MLBoard.TabIndex = 121;
            this.MLBoard.UseVisualStyleBackColor = true;
            this.MLBoard.Click += new System.EventHandler(this.MLBoard_Click);
            // 
            // LRBoard
            // 
            this.LRBoard.Location = new System.Drawing.Point(871, 360);
            this.LRBoard.Name = "LRBoard";
            this.LRBoard.Size = new System.Drawing.Size(50, 50);
            this.LRBoard.TabIndex = 120;
            this.LRBoard.UseVisualStyleBackColor = true;
            this.LRBoard.Click += new System.EventHandler(this.LRBoard_Click);
            // 
            // ULBoard
            // 
            this.ULBoard.Location = new System.Drawing.Point(759, 248);
            this.ULBoard.Name = "ULBoard";
            this.ULBoard.Size = new System.Drawing.Size(50, 50);
            this.ULBoard.TabIndex = 119;
            this.ULBoard.UseVisualStyleBackColor = true;
            this.ULBoard.Click += new System.EventHandler(this.ULBoard_Click);
            // 
            // MMBoard
            // 
            this.MMBoard.Location = new System.Drawing.Point(815, 304);
            this.MMBoard.Name = "MMBoard";
            this.MMBoard.Size = new System.Drawing.Size(50, 50);
            this.MMBoard.TabIndex = 118;
            this.MMBoard.UseVisualStyleBackColor = true;
            this.MMBoard.Click += new System.EventHandler(this.MMBoard_Click);
            // 
            // LMBoard
            // 
            this.LMBoard.Location = new System.Drawing.Point(815, 360);
            this.LMBoard.Name = "LMBoard";
            this.LMBoard.Size = new System.Drawing.Size(50, 50);
            this.LMBoard.TabIndex = 117;
            this.LMBoard.UseVisualStyleBackColor = true;
            this.LMBoard.Click += new System.EventHandler(this.LMBoard_Click);
            // 
            // UMBoard
            // 
            this.UMBoard.Location = new System.Drawing.Point(815, 248);
            this.UMBoard.Name = "UMBoard";
            this.UMBoard.Size = new System.Drawing.Size(50, 50);
            this.UMBoard.TabIndex = 116;
            this.UMBoard.UseVisualStyleBackColor = true;
            this.UMBoard.Click += new System.EventHandler(this.UMBoard_Click);
            // 
            // MRBoard
            // 
            this.MRBoard.Location = new System.Drawing.Point(871, 304);
            this.MRBoard.Name = "MRBoard";
            this.MRBoard.Size = new System.Drawing.Size(50, 50);
            this.MRBoard.TabIndex = 115;
            this.MRBoard.UseVisualStyleBackColor = true;
            this.MRBoard.Click += new System.EventHandler(this.MRBoard_Click);
            // 
            // URBoard
            // 
            this.URBoard.Location = new System.Drawing.Point(871, 248);
            this.URBoard.Name = "URBoard";
            this.URBoard.Size = new System.Drawing.Size(50, 50);
            this.URBoard.TabIndex = 114;
            this.URBoard.UseVisualStyleBackColor = true;
            this.URBoard.Click += new System.EventHandler(this.URBoard_Click);
            // 
            // LLBoard
            // 
            this.LLBoard.Location = new System.Drawing.Point(759, 360);
            this.LLBoard.Name = "LLBoard";
            this.LLBoard.Size = new System.Drawing.Size(50, 50);
            this.LLBoard.TabIndex = 113;
            this.LLBoard.UseVisualStyleBackColor = true;
            this.LLBoard.Click += new System.EventHandler(this.LLBoard_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(756, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 17);
            this.label1.TabIndex = 122;
            this.label1.Text = "Grid Selector";
            // 
            // Setup
            // 
            this.Setup.Location = new System.Drawing.Point(759, 12);
            this.Setup.Name = "Setup";
            this.Setup.Size = new System.Drawing.Size(106, 50);
            this.Setup.TabIndex = 123;
            this.Setup.Text = "Setup";
            this.Setup.UseVisualStyleBackColor = true;
            this.Setup.Click += new System.EventHandler(this.Setup_Click);
            // 
            // CombinationLevel
            // 
            this.CombinationLevel.Location = new System.Drawing.Point(759, 68);
            this.CombinationLevel.Name = "CombinationLevel";
            this.CombinationLevel.Size = new System.Drawing.Size(100, 22);
            this.CombinationLevel.TabIndex = 124;
            // 
            // UltLogBox
            // 
            this.UltLogBox.Location = new System.Drawing.Point(703, 446);
            this.UltLogBox.Multiline = true;
            this.UltLogBox.Name = "UltLogBox";
            this.UltLogBox.Size = new System.Drawing.Size(238, 162);
            this.UltLogBox.TabIndex = 125;
            // 
            // BoardBox
            // 
            this.BoardBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("BoardBox.InitialImage")));
            this.BoardBox.Location = new System.Drawing.Point(12, 12);
            this.BoardBox.Name = "BoardBox";
            this.BoardBox.Size = new System.Drawing.Size(50, 50);
            this.BoardBox.TabIndex = 126;
            this.BoardBox.TabStop = false;
            this.BoardBox.Paint += new System.Windows.Forms.PaintEventHandler(this.BoardBox_Paint);
            this.BoardBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BoardBox_MouseClick);
            // 
            // TempStart
            // 
            this.TempStart.Location = new System.Drawing.Point(759, 96);
            this.TempStart.Name = "TempStart";
            this.TempStart.Size = new System.Drawing.Size(106, 50);
            this.TempStart.TabIndex = 127;
            this.TempStart.Text = "Start";
            this.TempStart.UseVisualStyleBackColor = true;
            this.TempStart.Click += new System.EventHandler(this.TempStart_Click);
            // 
            // ModeDisplayer
            // 
            this.ModeDisplayer.AutoSize = true;
            this.ModeDisplayer.Location = new System.Drawing.Point(756, 160);
            this.ModeDisplayer.Name = "ModeDisplayer";
            this.ModeDisplayer.Size = new System.Drawing.Size(124, 17);
            this.ModeDisplayer.TabIndex = 128;
            this.ModeDisplayer.Text = "Human vs. Human";
            // 
            // SelectFlag
            // 
            this.SelectFlag.AutoSize = true;
            this.SelectFlag.Location = new System.Drawing.Point(756, 177);
            this.SelectFlag.Name = "SelectFlag";
            this.SelectFlag.Size = new System.Drawing.Size(0, 17);
            this.SelectFlag.TabIndex = 129;
            // 
            // Winner
            // 
            this.Winner.AutoSize = true;
            this.Winner.Location = new System.Drawing.Point(756, 194);
            this.Winner.Name = "Winner";
            this.Winner.Size = new System.Drawing.Size(0, 17);
            this.Winner.TabIndex = 130;
            // 
            // UltimateTicTacToe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 620);
            this.Controls.Add(this.Winner);
            this.Controls.Add(this.SelectFlag);
            this.Controls.Add(this.ModeDisplayer);
            this.Controls.Add(this.TempStart);
            this.Controls.Add(this.BoardBox);
            this.Controls.Add(this.UltLogBox);
            this.Controls.Add(this.CombinationLevel);
            this.Controls.Add(this.Setup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MLBoard);
            this.Controls.Add(this.LRBoard);
            this.Controls.Add(this.ULBoard);
            this.Controls.Add(this.MMBoard);
            this.Controls.Add(this.LMBoard);
            this.Controls.Add(this.UMBoard);
            this.Controls.Add(this.MRBoard);
            this.Controls.Add(this.URBoard);
            this.Controls.Add(this.LLBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UltimateTicTacToe";
            this.Text = "UltimateTicTacToe";
            this.Load += new System.EventHandler(this.UltimateTicTacToe_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UltimateTicTacToe_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.BoardBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button MLBoard;
        private System.Windows.Forms.Button LRBoard;
        private System.Windows.Forms.Button ULBoard;
        private System.Windows.Forms.Button MMBoard;
        private System.Windows.Forms.Button LMBoard;
        private System.Windows.Forms.Button UMBoard;
        private System.Windows.Forms.Button URBoard;
        private System.Windows.Forms.Button LLBoard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Setup;
        private System.Windows.Forms.TextBox CombinationLevel;
        private System.Windows.Forms.TextBox UltLogBox;
        private System.Windows.Forms.PictureBox BoardBox;
        private System.Windows.Forms.Button MRBoard;
        private System.Windows.Forms.Button TempStart;
        private System.Windows.Forms.Label ModeDisplayer;
        private System.Windows.Forms.Label SelectFlag;
        private System.Windows.Forms.Label Winner;
    }
}