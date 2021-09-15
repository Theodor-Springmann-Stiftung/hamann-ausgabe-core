namespace HaInformator
{
    partial class HaProgram
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HaProgram));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveLogDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ArribValDesc = new System.Windows.Forms.GroupBox();
            this.AttrList = new System.Windows.Forms.ListBox();
            this.ValList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NodeList = new System.Windows.Forms.ListBox();
            this.NodeTree = new System.Windows.Forms.TreeView();
            this.GoBtn = new System.Windows.Forms.Button();
            this.SaveLogBtn = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.InFileBtn = new System.Windows.Forms.Button();
            this.InFilepath = new System.Windows.Forms.TextBox();
            this.Description = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.footerStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.cMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tHeader = new System.Windows.Forms.ToolStripTextBox();
            this.tNewBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.tEditBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.tAddAttributeBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.tDeleteBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.tDeleteNodeKeepContentBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.tSaveBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lHeader = new System.Windows.Forms.ToolStripTextBox();
            this.lEditBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.lAddAttributeBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.lDeleteBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.lDeleteNodeKeepContentBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aHeader = new System.Windows.Forms.ToolStripTextBox();
            this.aNewBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.aEditBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.aDeleteBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.vHeader = new System.Windows.Forms.ToolStripTextBox();
            this.vEditBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.ArribValDesc.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.cMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.FileName = "openFile";
            this.openFileDialog.Filter = "XML-Dateien|*.xml";
            // 
            // saveLogDialog
            // 
            this.saveLogDialog.DefaultExt = "log";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ArribValDesc);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NodeList);
            this.groupBox1.Controls.Add(this.NodeTree);
            this.groupBox1.Location = new System.Drawing.Point(15, 306);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(1283, 658);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Analyse und Bearbeitung";
            // 
            // ArribValDesc
            // 
            this.ArribValDesc.Controls.Add(this.AttrList);
            this.ArribValDesc.Controls.Add(this.ValList);
            this.ArribValDesc.Location = new System.Drawing.Point(675, 18);
            this.ArribValDesc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ArribValDesc.Name = "ArribValDesc";
            this.ArribValDesc.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ArribValDesc.Size = new System.Drawing.Size(596, 631);
            this.ArribValDesc.TabIndex = 8;
            this.ArribValDesc.TabStop = false;
            this.ArribValDesc.Text = "Attribute und Werte";
            // 
            // AttrList
            // 
            this.AttrList.Enabled = false;
            this.AttrList.FormattingEnabled = true;
            this.AttrList.ItemHeight = 16;
            this.AttrList.Location = new System.Drawing.Point(8, 52);
            this.AttrList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AttrList.Name = "AttrList";
            this.AttrList.Size = new System.Drawing.Size(579, 148);
            this.AttrList.Sorted = true;
            this.AttrList.TabIndex = 1;
            // 
            // ValList
            // 
            this.ValList.Enabled = false;
            this.ValList.FormattingEnabled = true;
            this.ValList.ItemHeight = 16;
            this.ValList.Location = new System.Drawing.Point(8, 203);
            this.ValList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ValList.Name = "ValList";
            this.ValList.Size = new System.Drawing.Size(579, 420);
            this.ValList.Sorted = true;
            this.ValList.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(357, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tags - Listenansicht";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tags - Baumansicht";
            // 
            // NodeList
            // 
            this.NodeList.FormattingEnabled = true;
            this.NodeList.ItemHeight = 16;
            this.NodeList.Location = new System.Drawing.Point(361, 39);
            this.NodeList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.NodeList.Name = "NodeList";
            this.NodeList.Size = new System.Drawing.Size(304, 612);
            this.NodeList.Sorted = true;
            this.NodeList.TabIndex = 3;
            // 
            // NodeTree
            // 
            this.NodeTree.FullRowSelect = true;
            this.NodeTree.Indent = 24;
            this.NodeTree.Location = new System.Drawing.Point(8, 38);
            this.NodeTree.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.NodeTree.Name = "NodeTree";
            this.NodeTree.Size = new System.Drawing.Size(344, 611);
            this.NodeTree.TabIndex = 0;
            // 
            // GoBtn
            // 
            this.GoBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.GoBtn.Location = new System.Drawing.Point(1095, 270);
            this.GoBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GoBtn.Name = "GoBtn";
            this.GoBtn.Size = new System.Drawing.Size(203, 28);
            this.GoBtn.TabIndex = 8;
            this.GoBtn.Text = "Go";
            this.GoBtn.UseVisualStyleBackColor = true;
            this.GoBtn.Click += new System.EventHandler(this.GoBtn_Click);
            // 
            // SaveLogBtn
            // 
            this.SaveLogBtn.Enabled = false;
            this.SaveLogBtn.Location = new System.Drawing.Point(871, 270);
            this.SaveLogBtn.Margin = new System.Windows.Forms.Padding(860, 4, 4, 4);
            this.SaveLogBtn.Name = "SaveLogBtn";
            this.SaveLogBtn.Size = new System.Drawing.Size(216, 28);
            this.SaveLogBtn.TabIndex = 7;
            this.SaveLogBtn.Text = "Log speichern...";
            this.SaveLogBtn.UseVisualStyleBackColor = true;
            this.SaveLogBtn.Click += new System.EventHandler(this.SaveLogBtn_Click);
            // 
            // LogBox
            // 
            this.LogBox.AcceptsReturn = true;
            this.LogBox.AcceptsTab = true;
            this.LogBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.LogBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogBox.ForeColor = System.Drawing.SystemColors.Window;
            this.LogBox.Location = new System.Drawing.Point(15, 67);
            this.LogBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LogBox.MaxLength = 1000000;
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(1281, 195);
            this.LogBox.TabIndex = 6;
            this.LogBox.Text = "Log";
            this.LogBox.TextChanged += new System.EventHandler(this.LogBox_TextChanged);
            // 
            // InFileBtn
            // 
            this.InFileBtn.Location = new System.Drawing.Point(1146, 31);
            this.InFileBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.InFileBtn.Name = "InFileBtn";
            this.InFileBtn.Size = new System.Drawing.Size(149, 28);
            this.InFileBtn.TabIndex = 2;
            this.InFileBtn.Text = "Durchsuchen...";
            this.InFileBtn.UseVisualStyleBackColor = true;
            this.InFileBtn.Click += new System.EventHandler(this.InFileBtn_Click);
            // 
            // InFilepath
            // 
            this.InFilepath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InFilepath.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InFilepath.Location = new System.Drawing.Point(15, 37);
            this.InFilepath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.InFilepath.Name = "InFilepath";
            this.InFilepath.ReadOnly = true;
            this.InFilepath.Size = new System.Drawing.Size(1123, 22);
            this.InFilepath.TabIndex = 1;
            this.InFilepath.Text = "XML-Datei";
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(15, 10);
            this.Description.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Description.MinimumSize = new System.Drawing.Size(1283, 0);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(1283, 17);
            this.Description.TabIndex = 9;
            this.Description.Text = "Beschreibung";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.Description);
            this.flowLayoutPanel1.Controls.Add(this.InFilepath);
            this.flowLayoutPanel1.Controls.Add(this.InFileBtn);
            this.flowLayoutPanel1.Controls.Add(this.LogBox);
            this.flowLayoutPanel1.Controls.Add(this.SaveLogBtn);
            this.flowLayoutPanel1.Controls.Add(this.GoBtn);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(1360, 966);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(1333, 966);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1333, 966);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.footerStatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 956);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1309, 26);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // footerStatusText
            // 
            this.footerStatusText.Name = "footerStatusText";
            this.footerStatusText.Size = new System.Drawing.Size(48, 20);
            this.footerStatusText.Text = "Bereit";
            // 
            // cMenu
            // 
            this.cMenu.BackColor = System.Drawing.SystemColors.Control;
            this.cMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tHeader,
            this.tNewBtn,
            this.tEditBtn,
            this.tAddAttributeBtn,
            this.tDeleteBtn,
            this.tDeleteNodeKeepContentBtn,
            this.tSaveBtn,
            this.toolStripSeparator1,
            this.lHeader,
            this.lEditBtn,
            this.lAddAttributeBtn,
            this.lDeleteBtn,
            this.lDeleteNodeKeepContentBtn,
            this.toolStripSeparator2,
            this.aHeader,
            this.aNewBtn,
            this.aEditBtn,
            this.aDeleteBtn,
            this.toolStripSeparator3,
            this.vHeader,
            this.vEditBtn});
            this.cMenu.Name = "cMenu";
            this.cMenu.ShowImageMargin = false;
            this.cMenu.ShowItemToolTips = false;
            this.cMenu.Size = new System.Drawing.Size(261, 450);
            // 
            // tHeader
            // 
            this.tHeader.BackColor = System.Drawing.SystemColors.Control;
            this.tHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tHeader.Enabled = false;
            this.tHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tHeader.Margin = new System.Windows.Forms.Padding(3, 2, 2, 1);
            this.tHeader.Name = "tHeader";
            this.tHeader.ReadOnly = true;
            this.tHeader.Size = new System.Drawing.Size(190, 20);
            this.tHeader.Text = "Aktionen im Baum";
            // 
            // tNewBtn
            // 
            this.tNewBtn.Enabled = false;
            this.tNewBtn.Name = "tNewBtn";
            this.tNewBtn.Size = new System.Drawing.Size(260, 24);
            this.tNewBtn.Text = "Neu...";
            // 
            // tEditBtn
            // 
            this.tEditBtn.Enabled = false;
            this.tEditBtn.Name = "tEditBtn";
            this.tEditBtn.Size = new System.Drawing.Size(260, 24);
            this.tEditBtn.Text = "Bearbeiten...";
            // 
            // tAddAttributeBtn
            // 
            this.tAddAttributeBtn.Enabled = false;
            this.tAddAttributeBtn.Name = "tAddAttributeBtn";
            this.tAddAttributeBtn.Size = new System.Drawing.Size(260, 24);
            this.tAddAttributeBtn.Text = "Attribut hinzufügen...";
            // 
            // tDeleteBtn
            // 
            this.tDeleteBtn.Enabled = false;
            this.tDeleteBtn.Name = "tDeleteBtn";
            this.tDeleteBtn.Size = new System.Drawing.Size(260, 24);
            this.tDeleteBtn.Text = "Knoten löschen";
            // 
            // tDeleteNodeKeepContentBtn
            // 
            this.tDeleteNodeKeepContentBtn.Enabled = false;
            this.tDeleteNodeKeepContentBtn.Name = "tDeleteNodeKeepContentBtn";
            this.tDeleteNodeKeepContentBtn.Size = new System.Drawing.Size(260, 24);
            this.tDeleteNodeKeepContentBtn.Text = "Knoten löschen, Inhalt behalten";
            // 
            // tSaveBtn
            // 
            this.tSaveBtn.Enabled = false;
            this.tSaveBtn.Name = "tSaveBtn";
            this.tSaveBtn.Size = new System.Drawing.Size(260, 24);
            this.tSaveBtn.Text = "Knoten als Datei speichern...";
            this.tSaveBtn.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(257, 6);
            // 
            // lHeader
            // 
            this.lHeader.BackColor = System.Drawing.SystemColors.Control;
            this.lHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lHeader.Enabled = false;
            this.lHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lHeader.Margin = new System.Windows.Forms.Padding(3, 2, 2, 1);
            this.lHeader.Name = "lHeader";
            this.lHeader.ReadOnly = true;
            this.lHeader.Size = new System.Drawing.Size(190, 20);
            this.lHeader.Text = "Aktionen in der Liste";
            // 
            // lEditBtn
            // 
            this.lEditBtn.Enabled = false;
            this.lEditBtn.Name = "lEditBtn";
            this.lEditBtn.Size = new System.Drawing.Size(260, 24);
            this.lEditBtn.Text = "Bearbeiten...";
            // 
            // lAddAttributeBtn
            // 
            this.lAddAttributeBtn.Enabled = false;
            this.lAddAttributeBtn.Name = "lAddAttributeBtn";
            this.lAddAttributeBtn.Size = new System.Drawing.Size(260, 24);
            this.lAddAttributeBtn.Text = "Attribut hinzufügen...";
            // 
            // lDeleteBtn
            // 
            this.lDeleteBtn.Enabled = false;
            this.lDeleteBtn.Name = "lDeleteBtn";
            this.lDeleteBtn.Size = new System.Drawing.Size(260, 24);
            this.lDeleteBtn.Text = "Knoten löschen";
            // 
            // lDeleteNodeKeepContentBtn
            // 
            this.lDeleteNodeKeepContentBtn.Enabled = false;
            this.lDeleteNodeKeepContentBtn.Name = "lDeleteNodeKeepContentBtn";
            this.lDeleteNodeKeepContentBtn.Size = new System.Drawing.Size(260, 24);
            this.lDeleteNodeKeepContentBtn.Text = "Knoten löschen, Inhalt behalten";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(257, 6);
            // 
            // aHeader
            // 
            this.aHeader.BackColor = System.Drawing.SystemColors.Control;
            this.aHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.aHeader.Enabled = false;
            this.aHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.aHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aHeader.Margin = new System.Windows.Forms.Padding(3, 2, 2, 1);
            this.aHeader.Name = "aHeader";
            this.aHeader.ReadOnly = true;
            this.aHeader.Size = new System.Drawing.Size(190, 20);
            this.aHeader.Text = "Aktionen in der Attributsliste";
            // 
            // aNewBtn
            // 
            this.aNewBtn.Enabled = false;
            this.aNewBtn.Name = "aNewBtn";
            this.aNewBtn.Size = new System.Drawing.Size(260, 24);
            this.aNewBtn.Text = "Neu...";
            // 
            // aEditBtn
            // 
            this.aEditBtn.Enabled = false;
            this.aEditBtn.Name = "aEditBtn";
            this.aEditBtn.Size = new System.Drawing.Size(260, 24);
            this.aEditBtn.Text = "Bearbeiten...";
            // 
            // aDeleteBtn
            // 
            this.aDeleteBtn.Enabled = false;
            this.aDeleteBtn.Name = "aDeleteBtn";
            this.aDeleteBtn.Size = new System.Drawing.Size(260, 24);
            this.aDeleteBtn.Text = "Löschen";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(257, 6);
            // 
            // vHeader
            // 
            this.vHeader.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.vHeader.BackColor = System.Drawing.SystemColors.Control;
            this.vHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vHeader.Enabled = false;
            this.vHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.vHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.vHeader.Margin = new System.Windows.Forms.Padding(3, 2, 2, 1);
            this.vHeader.Name = "vHeader";
            this.vHeader.ReadOnly = true;
            this.vHeader.Size = new System.Drawing.Size(190, 20);
            this.vHeader.Text = "Aktionen in der Werteliste";
            // 
            // vEditBtn
            // 
            this.vEditBtn.Enabled = false;
            this.vEditBtn.Name = "vEditBtn";
            this.vEditBtn.Size = new System.Drawing.Size(260, 24);
            this.vEditBtn.Text = "Bearbeiten...";
            // 
            // HaProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1309, 982);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1327, 1029);
            this.MinimumSize = new System.Drawing.Size(1327, 1029);
            this.Name = "HaProgram";
            this.Text = "HaProgram";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ArribValDesc.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cMenu.ResumeLayout(false);
            this.cMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveLogDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ValList;
        private System.Windows.Forms.ListBox AttrList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox NodeList;
        private System.Windows.Forms.TreeView NodeTree;
        private System.Windows.Forms.Button GoBtn;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.TextBox InFilepath;
        private System.Windows.Forms.Button InFileBtn;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.Button SaveLogBtn;
        private System.Windows.Forms.GroupBox ArribValDesc;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel footerStatusText;
        private System.Windows.Forms.ContextMenuStrip cMenu;
        private System.Windows.Forms.ToolStripTextBox tHeader;
        private System.Windows.Forms.ToolStripMenuItem tNewBtn;
        private System.Windows.Forms.ToolStripMenuItem tEditBtn;
        private System.Windows.Forms.ToolStripMenuItem tAddAttributeBtn;
        private System.Windows.Forms.ToolStripMenuItem tDeleteBtn;
        private System.Windows.Forms.ToolStripMenuItem tDeleteNodeKeepContentBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox lHeader;
        private System.Windows.Forms.ToolStripMenuItem lEditBtn;
        private System.Windows.Forms.ToolStripMenuItem lAddAttributeBtn;
        private System.Windows.Forms.ToolStripMenuItem lDeleteBtn;
        private System.Windows.Forms.ToolStripMenuItem lDeleteNodeKeepContentBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripTextBox aHeader;
        private System.Windows.Forms.ToolStripMenuItem aNewBtn;
        private System.Windows.Forms.ToolStripMenuItem aEditBtn;
        private System.Windows.Forms.ToolStripMenuItem aDeleteBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripTextBox vHeader;
        private System.Windows.Forms.ToolStripMenuItem vEditBtn;
        private System.Windows.Forms.ToolStripMenuItem tSaveBtn;
    }
}

