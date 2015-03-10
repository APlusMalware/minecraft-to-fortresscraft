namespace MC_to_FCE
{
    partial class MCToFCEForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.MCToFCENamePathInput = new System.Windows.Forms.TextBox();
            this.MCToFCENamePathButton = new System.Windows.Forms.Button();
            this.MinecraftWorldPathInput = new System.Windows.Forms.TextBox();
            this.FortressCraftWorldPathInput = new System.Windows.Forms.TextBox();
            this.MinecraftWorldPathButton = new System.Windows.Forms.Button();
            this.FortressCraftWorldPathButton = new System.Windows.Forms.Button();
            this.StartConvertButton = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.MCToFCENamePathDialog = new System.Windows.Forms.OpenFileDialog();
            this.FortressCraftTerrainDataPathInput = new System.Windows.Forms.TextBox();
            this.FortressCraftTerrainDataPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.FortressCraftWorldPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.MinecraftWorldPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.FortressCraftTerrainDataPathButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cube Mapping Path";
            // 
            // MCToFCENamePathInput
            // 
            this.MCToFCENamePathInput.Location = new System.Drawing.Point(120, 67);
            this.MCToFCENamePathInput.Name = "MCToFCENamePathInput";
            this.MCToFCENamePathInput.Size = new System.Drawing.Size(486, 20);
            this.MCToFCENamePathInput.TabIndex = 1;
            this.MCToFCENamePathInput.Text = "R:\\Documents\\Visual Studio 2013\\Projects\\MC to FCE\\MC to FCE\\MCNamesToFCENames.xm" +
    "l";
            // 
            // MCToFCENamePathButton
            // 
            this.MCToFCENamePathButton.Location = new System.Drawing.Point(612, 65);
            this.MCToFCENamePathButton.Name = "MCToFCENamePathButton";
            this.MCToFCENamePathButton.Size = new System.Drawing.Size(91, 23);
            this.MCToFCENamePathButton.TabIndex = 2;
            this.MCToFCENamePathButton.Text = "Select File";
            this.MCToFCENamePathButton.UseVisualStyleBackColor = true;
            this.MCToFCENamePathButton.Click += new System.EventHandler(this.MCToFCENamePathButton_click);
            // 
            // MinecraftWorldPathInput
            // 
            this.MinecraftWorldPathInput.Location = new System.Drawing.Point(120, 40);
            this.MinecraftWorldPathInput.Name = "MinecraftWorldPathInput";
            this.MinecraftWorldPathInput.Size = new System.Drawing.Size(486, 20);
            this.MinecraftWorldPathInput.TabIndex = 3;
            this.MinecraftWorldPathInput.Text = "C:\\Users\\Ultimate\\AppData\\Roaming\\.minecraft\\saves\\New World 2";
            // 
            // FortressCraftWorldPathInput
            // 
            this.FortressCraftWorldPathInput.Location = new System.Drawing.Point(120, 13);
            this.FortressCraftWorldPathInput.Name = "FortressCraftWorldPathInput";
            this.FortressCraftWorldPathInput.Size = new System.Drawing.Size(486, 20);
            this.FortressCraftWorldPathInput.TabIndex = 4;
            this.FortressCraftWorldPathInput.Text = "C:\\Users\\Ultimate\\AppData\\LocalLow\\ProjectorGames\\FortressCraft\\Worlds\\MC Convert" +
    "ed 2\\";
            // 
            // MinecraftWorldPathButton
            // 
            this.MinecraftWorldPathButton.Location = new System.Drawing.Point(612, 38);
            this.MinecraftWorldPathButton.Name = "MinecraftWorldPathButton";
            this.MinecraftWorldPathButton.Size = new System.Drawing.Size(91, 23);
            this.MinecraftWorldPathButton.TabIndex = 5;
            this.MinecraftWorldPathButton.Text = "Select Folder";
            this.MinecraftWorldPathButton.UseVisualStyleBackColor = true;
            this.MinecraftWorldPathButton.Click += new System.EventHandler(this.MinecraftWorldPathButton_click);
            // 
            // FortressCraftWorldPathButton
            // 
            this.FortressCraftWorldPathButton.Location = new System.Drawing.Point(612, 11);
            this.FortressCraftWorldPathButton.Name = "FortressCraftWorldPathButton";
            this.FortressCraftWorldPathButton.Size = new System.Drawing.Size(91, 23);
            this.FortressCraftWorldPathButton.TabIndex = 6;
            this.FortressCraftWorldPathButton.Text = "Select Folder";
            this.FortressCraftWorldPathButton.UseVisualStyleBackColor = true;
            this.FortressCraftWorldPathButton.Click += new System.EventHandler(this.FortressCraftWorldPathButton_click);
            // 
            // StartConvertButton
            // 
            this.StartConvertButton.Location = new System.Drawing.Point(612, 222);
            this.StartConvertButton.Name = "StartConvertButton";
            this.StartConvertButton.Size = new System.Drawing.Size(91, 23);
            this.StartConvertButton.TabIndex = 7;
            this.StartConvertButton.Text = "Start Convert";
            this.StartConvertButton.UseVisualStyleBackColor = true;
            this.StartConvertButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(612, 120);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(91, 20);
            this.textBox4.TabIndex = 8;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(612, 156);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(91, 20);
            this.textBox5.TabIndex = 9;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(612, 180);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(91, 20);
            this.textBox6.TabIndex = 10;
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(12, 156);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(335, 126);
            this.logBox.TabIndex = 11;
            // 
            // MCToFCENamePathDialog
            // 
            this.MCToFCENamePathDialog.FileName = "MCNamesToFCENames.xml";
            this.MCToFCENamePathDialog.Filter = "XML files|*.xml|All files|*.*";
            // 
            // FortressCraftTerrainDataPathInput
            // 
            this.FortressCraftTerrainDataPathInput.Location = new System.Drawing.Point(120, 93);
            this.FortressCraftTerrainDataPathInput.Name = "FortressCraftTerrainDataPathInput";
            this.FortressCraftTerrainDataPathInput.Size = new System.Drawing.Size(486, 20);
            this.FortressCraftTerrainDataPathInput.TabIndex = 12;
            this.FortressCraftTerrainDataPathInput.Text = "C:\\Users\\Ultimate\\AppData\\LocalLow\\ProjectorGames\\FortressCraft\\Worlds\\MC Convert" +
    "ed 2\\";
            // 
            // FortressCraftTerrainDataPathDialog
            // 
            this.FortressCraftTerrainDataPathDialog.FileName = "TerrainData.xml";
            this.FortressCraftTerrainDataPathDialog.Filter = "XML files|*.xml|All files|*.*";
            // 
            // FortressCraftWorldPathDialog
            // 
            this.FortressCraftWorldPathDialog.DefaultExt = "dat";
            this.FortressCraftWorldPathDialog.Filter = "World files|world.dat";
            // 
            // MinecraftWorldPathDialog
            // 
            this.MinecraftWorldPathDialog.DefaultExt = "dat";
            this.MinecraftWorldPathDialog.Filter = "Level files|level.dat|All files|*.*";
            // 
            // FortressCraftTerrainDataPathButton
            // 
            this.FortressCraftTerrainDataPathButton.Location = new System.Drawing.Point(612, 90);
            this.FortressCraftTerrainDataPathButton.Name = "FortressCraftTerrainDataPathButton";
            this.FortressCraftTerrainDataPathButton.Size = new System.Drawing.Size(91, 23);
            this.FortressCraftTerrainDataPathButton.TabIndex = 13;
            this.FortressCraftTerrainDataPathButton.Text = "Select File";
            this.FortressCraftTerrainDataPathButton.UseVisualStyleBackColor = true;
            this.FortressCraftTerrainDataPathButton.Click += new System.EventHandler(this.FortressCraftTerrainDataPathButton_Click);
            // 
            // MCToFCEForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 294);
            this.Controls.Add(this.FortressCraftTerrainDataPathButton);
            this.Controls.Add(this.FortressCraftTerrainDataPathInput);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.StartConvertButton);
            this.Controls.Add(this.FortressCraftWorldPathButton);
            this.Controls.Add(this.MinecraftWorldPathButton);
            this.Controls.Add(this.FortressCraftWorldPathInput);
            this.Controls.Add(this.MinecraftWorldPathInput);
            this.Controls.Add(this.MCToFCENamePathButton);
            this.Controls.Add(this.MCToFCENamePathInput);
            this.Controls.Add(this.label1);
            this.Name = "MCToFCEForm";
            this.Text = "Minecraft to FortressCraft Evolved World Converter";
            this.Load += new System.EventHandler(this.MCToFCEForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MCToFCENamePathInput;
        private System.Windows.Forms.Button MCToFCENamePathButton;
        private System.Windows.Forms.TextBox MinecraftWorldPathInput;
        private System.Windows.Forms.TextBox FortressCraftWorldPathInput;
        private System.Windows.Forms.Button MinecraftWorldPathButton;
        private System.Windows.Forms.Button FortressCraftWorldPathButton;
        private System.Windows.Forms.Button StartConvertButton;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.OpenFileDialog MCToFCENamePathDialog;
        private System.Windows.Forms.TextBox FortressCraftTerrainDataPathInput;
        private System.Windows.Forms.OpenFileDialog FortressCraftTerrainDataPathDialog;
        private System.Windows.Forms.OpenFileDialog FortressCraftWorldPathDialog;
        private System.Windows.Forms.OpenFileDialog MinecraftWorldPathDialog;
        private System.Windows.Forms.Button FortressCraftTerrainDataPathButton;
    }
}

