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
            this.MCToFCENamePathLabel = new System.Windows.Forms.Label();
            this.MCToFCENamePathInput = new System.Windows.Forms.TextBox();
            this.MCToFCENamePathButton = new System.Windows.Forms.Button();
            this.MinecraftWorldPathInput = new System.Windows.Forms.TextBox();
            this.FortressCraftWorldPathInput = new System.Windows.Forms.TextBox();
            this.MinecraftWorldPathButton = new System.Windows.Forms.Button();
            this.FortressCraftWorldPathButton = new System.Windows.Forms.Button();
            this.StartConvertButton = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.TextBox();
            this.MCToFCENamePathDialog = new System.Windows.Forms.OpenFileDialog();
            this.FortressCraftTerrainDataPathInput = new System.Windows.Forms.TextBox();
            this.FortressCraftTerrainDataPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.FortressCraftWorldPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.MinecraftWorldPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.FortressCraftTerrainDataPathButton = new System.Windows.Forms.Button();
            this.FortressCraftTerrainDataLabel = new System.Windows.Forms.Label();
            this.FortressCraftWorldPathLabel = new System.Windows.Forms.Label();
            this.MinecraftWorldPathLabel = new System.Windows.Forms.Label();
            this.LogBoxLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MCToFCENamePathLabel
            // 
            this.MCToFCENamePathLabel.AutoSize = true;
            this.MCToFCENamePathLabel.Location = new System.Drawing.Point(9, 87);
            this.MCToFCENamePathLabel.Name = "MCToFCENamePathLabel";
            this.MCToFCENamePathLabel.Size = new System.Drawing.Size(101, 13);
            this.MCToFCENamePathLabel.TabIndex = 12;
            this.MCToFCENamePathLabel.Text = "Cube Mapping Path";
            // 
            // MCToFCENamePathInput
            // 
            this.MCToFCENamePathInput.Location = new System.Drawing.Point(12, 103);
            this.MCToFCENamePathInput.Name = "MCToFCENamePathInput";
            this.MCToFCENamePathInput.Size = new System.Drawing.Size(503, 20);
            this.MCToFCENamePathInput.TabIndex = 4;
            this.MCToFCENamePathInput.Text = "MCNamesToFCENames.xml";
            // 
            // MCToFCENamePathButton
            // 
            this.MCToFCENamePathButton.Location = new System.Drawing.Point(521, 101);
            this.MCToFCENamePathButton.Name = "MCToFCENamePathButton";
            this.MCToFCENamePathButton.Size = new System.Drawing.Size(91, 23);
            this.MCToFCENamePathButton.TabIndex = 5;
            this.MCToFCENamePathButton.Text = "Select File";
            this.MCToFCENamePathButton.UseVisualStyleBackColor = true;
            this.MCToFCENamePathButton.Click += new System.EventHandler(this.MCToFCENamePathButton_click);
            // 
            // MinecraftWorldPathInput
            // 
            this.MinecraftWorldPathInput.Location = new System.Drawing.Point(12, 64);
            this.MinecraftWorldPathInput.Name = "MinecraftWorldPathInput";
            this.MinecraftWorldPathInput.Size = new System.Drawing.Size(503, 20);
            this.MinecraftWorldPathInput.TabIndex = 2;
            this.MinecraftWorldPathInput.Text = "level.dat";
            // 
            // FortressCraftWorldPathInput
            // 
            this.FortressCraftWorldPathInput.Location = new System.Drawing.Point(12, 25);
            this.FortressCraftWorldPathInput.Name = "FortressCraftWorldPathInput";
            this.FortressCraftWorldPathInput.Size = new System.Drawing.Size(503, 20);
            this.FortressCraftWorldPathInput.TabIndex = 0;
            this.FortressCraftWorldPathInput.Text = "world.dat";
            // 
            // MinecraftWorldPathButton
            // 
            this.MinecraftWorldPathButton.Location = new System.Drawing.Point(521, 62);
            this.MinecraftWorldPathButton.Name = "MinecraftWorldPathButton";
            this.MinecraftWorldPathButton.Size = new System.Drawing.Size(91, 23);
            this.MinecraftWorldPathButton.TabIndex = 3;
            this.MinecraftWorldPathButton.Text = "Select World";
            this.MinecraftWorldPathButton.UseVisualStyleBackColor = true;
            this.MinecraftWorldPathButton.Click += new System.EventHandler(this.MinecraftWorldPathButton_click);
            // 
            // FortressCraftWorldPathButton
            // 
            this.FortressCraftWorldPathButton.Location = new System.Drawing.Point(521, 23);
            this.FortressCraftWorldPathButton.Name = "FortressCraftWorldPathButton";
            this.FortressCraftWorldPathButton.Size = new System.Drawing.Size(91, 23);
            this.FortressCraftWorldPathButton.TabIndex = 1;
            this.FortressCraftWorldPathButton.Text = "Select World";
            this.FortressCraftWorldPathButton.UseVisualStyleBackColor = true;
            this.FortressCraftWorldPathButton.Click += new System.EventHandler(this.FortressCraftWorldPathButton_click);
            // 
            // StartConvertButton
            // 
            this.StartConvertButton.Location = new System.Drawing.Point(521, 179);
            this.StartConvertButton.Name = "StartConvertButton";
            this.StartConvertButton.Size = new System.Drawing.Size(91, 23);
            this.StartConvertButton.TabIndex = 8;
            this.StartConvertButton.Text = "Start Convert";
            this.StartConvertButton.UseVisualStyleBackColor = true;
            this.StartConvertButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(12, 208);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(600, 131);
            this.logBox.TabIndex = 9;
            // 
            // MCToFCENamePathDialog
            // 
            this.MCToFCENamePathDialog.FileName = "MCNamesToFCENames.xml";
            this.MCToFCENamePathDialog.Filter = "XML files|*.xml|All files|*.*";
            this.MCToFCENamePathDialog.Title = "Select an XML file";
            // 
            // FortressCraftTerrainDataPathInput
            // 
            this.FortressCraftTerrainDataPathInput.Location = new System.Drawing.Point(12, 142);
            this.FortressCraftTerrainDataPathInput.Name = "FortressCraftTerrainDataPathInput";
            this.FortressCraftTerrainDataPathInput.Size = new System.Drawing.Size(503, 20);
            this.FortressCraftTerrainDataPathInput.TabIndex = 6;
            this.FortressCraftTerrainDataPathInput.Text = "TerrainData.xml";
            // 
            // FortressCraftTerrainDataPathDialog
            // 
            this.FortressCraftTerrainDataPathDialog.FileName = "TerrainData.xml";
            this.FortressCraftTerrainDataPathDialog.Filter = "XML files|*.xml|All files|*.*";
            this.FortressCraftTerrainDataPathDialog.Title = "Select an XML file";
            // 
            // FortressCraftWorldPathDialog
            // 
            this.FortressCraftWorldPathDialog.DefaultExt = "dat";
            this.FortressCraftWorldPathDialog.FileName = "world.dat";
            this.FortressCraftWorldPathDialog.Filter = "World files|world.dat";
            this.FortressCraftWorldPathDialog.Title = "Select a world.dat file";
            // 
            // MinecraftWorldPathDialog
            // 
            this.MinecraftWorldPathDialog.DefaultExt = "dat";
            this.MinecraftWorldPathDialog.FileName = "level.dat";
            this.MinecraftWorldPathDialog.Filter = "Level files|level.dat|All files|*.*";
            this.MinecraftWorldPathDialog.Title = "Select a level.dat file";
            // 
            // FortressCraftTerrainDataPathButton
            // 
            this.FortressCraftTerrainDataPathButton.Location = new System.Drawing.Point(521, 140);
            this.FortressCraftTerrainDataPathButton.Name = "FortressCraftTerrainDataPathButton";
            this.FortressCraftTerrainDataPathButton.Size = new System.Drawing.Size(91, 23);
            this.FortressCraftTerrainDataPathButton.TabIndex = 7;
            this.FortressCraftTerrainDataPathButton.Text = "Select File";
            this.FortressCraftTerrainDataPathButton.UseVisualStyleBackColor = true;
            this.FortressCraftTerrainDataPathButton.Click += new System.EventHandler(this.FortressCraftTerrainDataPathButton_Click);
            // 
            // FortressCraftTerrainDataLabel
            // 
            this.FortressCraftTerrainDataLabel.AutoSize = true;
            this.FortressCraftTerrainDataLabel.Location = new System.Drawing.Point(9, 126);
            this.FortressCraftTerrainDataLabel.Name = "FortressCraftTerrainDataLabel";
            this.FortressCraftTerrainDataLabel.Size = new System.Drawing.Size(114, 13);
            this.FortressCraftTerrainDataLabel.TabIndex = 13;
            this.FortressCraftTerrainDataLabel.Text = "FCE Terrain Data Path";
            // 
            // FortressCraftWorldPathLabel
            // 
            this.FortressCraftWorldPathLabel.AutoSize = true;
            this.FortressCraftWorldPathLabel.Location = new System.Drawing.Point(9, 9);
            this.FortressCraftWorldPathLabel.Name = "FortressCraftWorldPathLabel";
            this.FortressCraftWorldPathLabel.Size = new System.Drawing.Size(122, 13);
            this.FortressCraftWorldPathLabel.TabIndex = 10;
            this.FortressCraftWorldPathLabel.Text = "FortressCraft World Path";
            // 
            // MinecraftWorldPathLabel
            // 
            this.MinecraftWorldPathLabel.AutoSize = true;
            this.MinecraftWorldPathLabel.Location = new System.Drawing.Point(9, 48);
            this.MinecraftWorldPathLabel.Name = "MinecraftWorldPathLabel";
            this.MinecraftWorldPathLabel.Size = new System.Drawing.Size(107, 13);
            this.MinecraftWorldPathLabel.TabIndex = 11;
            this.MinecraftWorldPathLabel.Text = "Minecraft World Path";
            // 
            // LogBoxLabel
            // 
            this.LogBoxLabel.AutoSize = true;
            this.LogBoxLabel.Location = new System.Drawing.Point(9, 184);
            this.LogBoxLabel.Name = "LogBoxLabel";
            this.LogBoxLabel.Size = new System.Drawing.Size(25, 13);
            this.LogBoxLabel.TabIndex = 14;
            this.LogBoxLabel.Text = "Log";
            // 
            // MCToFCEForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 351);
            this.Controls.Add(this.LogBoxLabel);
            this.Controls.Add(this.MinecraftWorldPathLabel);
            this.Controls.Add(this.FortressCraftWorldPathLabel);
            this.Controls.Add(this.FortressCraftTerrainDataLabel);
            this.Controls.Add(this.FortressCraftTerrainDataPathButton);
            this.Controls.Add(this.FortressCraftTerrainDataPathInput);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.StartConvertButton);
            this.Controls.Add(this.FortressCraftWorldPathButton);
            this.Controls.Add(this.MinecraftWorldPathButton);
            this.Controls.Add(this.FortressCraftWorldPathInput);
            this.Controls.Add(this.MinecraftWorldPathInput);
            this.Controls.Add(this.MCToFCENamePathButton);
            this.Controls.Add(this.MCToFCENamePathInput);
            this.Controls.Add(this.MCToFCENamePathLabel);
            this.Name = "MCToFCEForm";
            this.Text = "Minecraft to FortressCraft Evolved World Converter";
            this.Load += new System.EventHandler(this.MCToFCEForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MCToFCENamePathLabel;
        private System.Windows.Forms.TextBox MCToFCENamePathInput;
        private System.Windows.Forms.Button MCToFCENamePathButton;
        private System.Windows.Forms.TextBox MinecraftWorldPathInput;
        private System.Windows.Forms.TextBox FortressCraftWorldPathInput;
        private System.Windows.Forms.Button MinecraftWorldPathButton;
        private System.Windows.Forms.Button FortressCraftWorldPathButton;
        private System.Windows.Forms.Button StartConvertButton;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.OpenFileDialog MCToFCENamePathDialog;
        private System.Windows.Forms.TextBox FortressCraftTerrainDataPathInput;
        private System.Windows.Forms.OpenFileDialog FortressCraftTerrainDataPathDialog;
        private System.Windows.Forms.OpenFileDialog FortressCraftWorldPathDialog;
        private System.Windows.Forms.OpenFileDialog MinecraftWorldPathDialog;
        private System.Windows.Forms.Button FortressCraftTerrainDataPathButton;
        private System.Windows.Forms.Label FortressCraftTerrainDataLabel;
        private System.Windows.Forms.Label FortressCraftWorldPathLabel;
        private System.Windows.Forms.Label MinecraftWorldPathLabel;
        private System.Windows.Forms.Label LogBoxLabel;
    }
}

