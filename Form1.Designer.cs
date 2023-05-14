namespace PCKTool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnExtractor = new Button();
            btnOpenDir = new Button();
            btnExtractorAll = new Button();
            btnDelete = new Button();
            chbxPack = new CheckBox();
            chbxUnpack = new CheckBox();
            labPack = new Label();
            tree = new TreeView();
            statusStrip = new StatusStrip();
            tssPbr = new ToolStripProgressBar();
            tssLab = new ToolStripStatusLabel();
            btnCreateFileList = new Button();
            toolTip1 = new ToolTip(components);
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // btnExtractor
            // 
            btnExtractor.Location = new Point(12, 9);
            btnExtractor.Name = "btnExtractor";
            btnExtractor.Size = new Size(75, 44);
            btnExtractor.TabIndex = 0;
            btnExtractor.Text = "Unpack Selected";
            toolTip1.SetToolTip(btnExtractor, "Unpack the selected files in the tree view");
            btnExtractor.UseVisualStyleBackColor = true;
            btnExtractor.Click += btnExtractor_Click;
            // 
            // btnOpenDir
            // 
            btnOpenDir.Location = new Point(100, 9);
            btnOpenDir.Name = "btnOpenDir";
            btnOpenDir.Size = new Size(75, 44);
            btnOpenDir.TabIndex = 1;
            btnOpenDir.Text = "Open Unpack Dir";
            toolTip1.SetToolTip(btnOpenDir, "Open the PCKOutput directory in the explorer");
            btnOpenDir.UseVisualStyleBackColor = true;
            btnOpenDir.Click += btnOpenDir_Click;
            // 
            // btnExtractorAll
            // 
            btnExtractorAll.Location = new Point(12, 61);
            btnExtractorAll.Name = "btnExtractorAll";
            btnExtractorAll.Size = new Size(75, 44);
            btnExtractorAll.TabIndex = 2;
            btnExtractorAll.Text = "Unpack All";
            toolTip1.SetToolTip(btnExtractorAll, "Unpack all files in the PCK files");
            btnExtractorAll.UseVisualStyleBackColor = true;
            btnExtractorAll.Click += btnExtractorAll_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(100, 61);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 44);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete Selected";
            toolTip1.SetToolTip(btnDelete, "Delete the selected files in the tree view");
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // chbxPack
            // 
            chbxPack.AutoSize = true;
            chbxPack.Checked = true;
            chbxPack.CheckState = CheckState.Checked;
            chbxPack.Location = new Point(153, 119);
            chbxPack.Name = "chbxPack";
            chbxPack.Size = new Size(116, 19);
            chbxPack.TabIndex = 4;
            chbxPack.Text = "Override Packing";
            toolTip1.SetToolTip(chbxPack, "Override the file in the PCK when packing.");
            chbxPack.UseVisualStyleBackColor = true;
            // 
            // chbxUnpack
            // 
            chbxUnpack.AutoSize = true;
            chbxUnpack.Checked = true;
            chbxUnpack.CheckState = CheckState.Checked;
            chbxUnpack.Location = new Point(12, 119);
            chbxUnpack.Name = "chbxUnpack";
            chbxUnpack.Size = new Size(131, 19);
            chbxUnpack.TabIndex = 5;
            chbxUnpack.Text = "Override Unpacking";
            toolTip1.SetToolTip(chbxUnpack, "Override any files in the PCKOutput directory");
            chbxUnpack.UseVisualStyleBackColor = true;
            // 
            // labPack
            // 
            labPack.AllowDrop = true;
            labPack.BackColor = Color.SteelBlue;
            labPack.BorderStyle = BorderStyle.FixedSingle;
            labPack.Location = new Point(286, 9);
            labPack.Name = "labPack";
            labPack.Size = new Size(136, 96);
            labPack.TabIndex = 6;
            labPack.Text = "Drag files/folders here";
            labPack.TextAlign = ContentAlignment.MiddleCenter;
            labPack.DragDrop += panPack_DragDrop;
            labPack.DragEnter += panPack_DragEnter;
            // 
            // tree
            // 
            tree.CheckBoxes = true;
            tree.Location = new Point(12, 150);
            tree.Name = "tree";
            tree.Size = new Size(410, 288);
            tree.TabIndex = 7;
            tree.AfterCheck += Tree_AfterCheck;
            tree.BeforeExpand += Tree_BeforeExpand;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { tssPbr, tssLab });
            statusStrip.Location = new Point(0, 448);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(434, 22);
            statusStrip.TabIndex = 12;
            statusStrip.Text = "statusStrip1";
            // 
            // tssPbr
            // 
            tssPbr.Name = "tssPbr";
            tssPbr.Size = new Size(100, 16);
            // 
            // tssLab
            // 
            tssLab.Name = "tssLab";
            tssLab.Size = new Size(39, 17);
            tssLab.Text = "Ready";
            // 
            // btnCreateFileList
            // 
            btnCreateFileList.Location = new Point(194, 35);
            btnCreateFileList.Name = "btnCreateFileList";
            btnCreateFileList.Size = new Size(75, 44);
            btnCreateFileList.TabIndex = 13;
            btnCreateFileList.Text = "Generate Filelist";
            toolTip1.SetToolTip(btnCreateFileList, "Create a filelist.txt with the file path, file size and hash.\r\nThis can be used for client updates.");
            btnCreateFileList.UseVisualStyleBackColor = true;
            btnCreateFileList.Click += btnCreateFileList_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 470);
            Controls.Add(btnCreateFileList);
            Controls.Add(statusStrip);
            Controls.Add(tree);
            Controls.Add(labPack);
            Controls.Add(chbxUnpack);
            Controls.Add(chbxPack);
            Controls.Add(btnDelete);
            Controls.Add(btnExtractorAll);
            Controls.Add(btnOpenDir);
            Controls.Add(btnExtractor);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rusty Hearts PCK Tool";
            Shown += Form1_Shown;
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnExtractor;
        private Button btnOpenDir;
        private Button btnExtractorAll;
        private Button btnDelete;
        private CheckBox chbxPack;
        private CheckBox chbxUnpack;
        private Label labPack;
        private TreeView tree;
        private StatusStrip statusStrip;
        private ToolStripProgressBar tssPbr;
        private ToolStripStatusLabel tssLab;
        private Button btnCreateFileList;
        private ToolTip toolTip1;
    }
}