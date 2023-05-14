using PCKTool.PCK;
using System.Text;

namespace PCKTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            await ReadFileListAsync();
        }

        private List<PCKFile>? listPckFile = null;
        private List<PCKFile>? listPckFileWrite = null;
        private Thread? thread = null;
        private bool replacePackFile = false;
        private bool replackUnpackFile = false;
        private delegate void WriteLabstringDelegate(string str);

        public void DisableButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new ThreadStart(DisableButtons));
                return;
            }

            chbxPack.Enabled =
            chbxUnpack.Enabled =
            btnDelete.Enabled =
            labPack.Enabled =
            btnExtractor.Enabled =
            btnExtractorAll.Enabled = false;
            btnCreateFileList.Enabled = false;
            labPack.BackColor = Color.DarkRed;
        }

        public void EnableButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new ThreadStart(EnableButtons));
                return;
            }

            chbxPack.Enabled =
            chbxUnpack.Enabled =
            btnDelete.Enabled =
            labPack.Enabled =
            btnExtractor.Enabled =
            btnExtractorAll.Enabled = true;
            btnCreateFileList.Enabled = true;
            labPack.BackColor = Color.SteelBlue;
        }

        public void WriteStatusLabstring(string str)
        {
            if (statusStrip.InvokeRequired) statusStrip.Invoke(new WriteLabstringDelegate(WriteStatusLabstring), str);
            else tssLab.Text = str;
        }

        private async Task ReadFileListAsync()
        {
            try
            {
                if (!PCKReader.IsRuningInGameDir())
                {
                    Hide();
                    MessageBox.Show("Place in the game directory to run!");
                    Close();
                    return;
                }

                DisableButtons();
                tree.Nodes.Clear();
                tssPbr.Value = 0;
                listPckFile = null;
                listPckFileWrite = null;

                tssLab.Text = "Reading file list...";
                await Task.Run(() =>
                {
                    listPckFile = PCKReader.ReadFileList();
                });

                if (listPckFile.Count > 0)
                {
                    SortedDictionary<string, PCKFileNode> listNode = null;

                    tssLab.Text = "Creating tree structure...";
                    await Task.Run(() =>
                    {
                        listNode = PCKReader.ConvertListToNode(listPckFile);
                    });

                    tssLab.Text = "Tree structure created";

                    int fileCount = listPckFile.Count;
                    tssPbr.Minimum = 0;
                    tssPbr.Maximum = fileCount;
                    tssPbr.Value = 0;

                    tssLab.Text = "Importing tree view...";

                    tree.BeginUpdate();
                    await Task.Run(() =>
                    {
                        AddNodes(listNode, tree.Nodes);
                    });
                    tree.EndUpdate();
                }

                tssLab.Text = "Number of files：" + listPckFile.Count;
            }
            catch (Exception ex)
            {
                Hide();
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                EnableButtons();
            }
        }

        private void AddNodes(SortedDictionary<string, PCKFileNode> listNode, TreeNodeCollection treeNodes)
        {
            foreach (KeyValuePair<string, PCKFileNode> kv in listNode)
            {
                TreeNode node = new();
                if (kv.Value.IsDir)
                {
                    node.Text = kv.Key;
                }
                else
                {
                    PCKFile pckFile = kv.Value.PCKFile;
                    node.Text = string.Format("{0} [{1:X}] {2}", kv.Key, pckFile.Hash, pckFile.Archive);
                }
                node.Tag = kv.Value;

                if (!kv.Value.IsDir)
                {
                    Invoke(new Action(() => { tssPbr.Value++; }));
                }

                if (kv.Value.Nodes != null && kv.Value.Nodes.Count > 0)
                {
                    AddNodes(kv.Value.Nodes, node.Nodes);
                }

                Invoke(new Action(() => { treeNodes.Add(node); }));
            }
        }

        public void CheckChild(TreeNodeCollection childNodes, bool check)
        {
            int count = childNodes.Count;
            for (int i = 0; i < count; i++)
            {
                TreeNode node = childNodes[i];
                PCKFileNode pckFileNode = (PCKFileNode)node.Tag;
                pckFileNode.IsChecked = check;
                CheckChild(node.Nodes, check);
                if (node.Parent == null || node.Parent.IsExpanded)
                {
                    node.Checked = check;
                }
            }
        }

        public void CheckParent(TreeNode node, bool check)
        {
            PCKFileNode pckFileNode = (PCKFileNode)node.Tag;
            pckFileNode.IsChecked = check;
            if (node.Checked != check)
            {
                node.Checked = check;
            }

            TreeNode nodeParent = node.Parent;
            if (nodeParent != null)
            {
                if (check)
                {
                    if (!node.Parent.Checked) CheckParent(node.Parent, check);
                }
                else
                {
                    TreeNodeCollection listNode = nodeParent.Nodes;
                    int count = listNode.Count;
                    bool isAllUnCheck = true;
                    for (int i = 0; i < count; i++)
                    {
                        TreeNode nodeSameLever = listNode[i];
                        if (nodeSameLever.Checked)
                        {
                            isAllUnCheck = false;
                            break;
                        }
                    }

                    if (isAllUnCheck) CheckParent(nodeParent, check);
                }
            }
        }

        public static void RestoreCheckState(TreeNodeCollection treeNodes)
        {
            int count = treeNodes.Count;
            for (int i = 0; i < count; i++)
            {
                TreeNode node = treeNodes[i];
                PCKFileNode pckFileNode = (PCKFileNode)node.Tag;
                node.Checked = pckFileNode.IsChecked;
            }
        }

        private void Tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                CheckParent(e.Node, e.Node.Checked);
                CheckChild(e.Node.Nodes, e.Node.Checked);
            }
        }

        private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            RestoreCheckState(e.Node.Nodes);
        }

        private void btnExtractor_Click(object sender, EventArgs e)
        {
            if (listPckFile.Count > 0)
            {
                replackUnpackFile = chbxUnpack.Checked;
                listPckFileWrite = new List<PCKFile>();
                foreach (PCKFile file in listPckFile)
                {
                    if (file.IsChecked) listPckFileWrite.Add(file);
                }

                if (listPckFileWrite.Count == 0)
                {
                    MessageBox.Show("Please select the file to be unpacked first");
                    return;
                }

                thread = new Thread(new ThreadStart(UnpackThread));
                thread.Start();
            }
        }

        private void btnExtractorAll_Click(object sender, EventArgs e)
        {
            if (listPckFile.Count > 0)
            {
                replackUnpackFile = chbxUnpack.Checked;
                listPckFileWrite = listPckFile;
                thread = new Thread(new ThreadStart(UnpackThread));
                thread.Start();
            }
        }

        private void UnpackThread()
        {
            DisableButtons();
            WriteStatusLabstring("Unpacking...");
            PCKReader.Unpack(listPckFileWrite, replackUnpackFile, new PCKReader.UnpackProgressDelegate(UnpackProgress));
            WriteStatusLabstring("Unpacking complete");
            EnableButtons();
            thread = null;
        }

        private void UnpackProgress(string name, int pos, int count)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke(new PCKReader.UnpackProgressDelegate(UnpackProgress), new object[] { name, pos, count });
                return;
            }
            if (tssPbr.Maximum != count) tssPbr.Maximum = count;
            tssPbr.Value = pos;
            tssLab.Text = string.Format("Unpacking...{0}/{1}", pos, count);
        }

        private void WriteLabPackstring(string str)
        {
            if (labPack.InvokeRequired) labPack.Invoke(new WriteLabstringDelegate(WriteLabPackstring), str);
            else labPack.Text = str;
        }

        private void MessageBoxShow(string str)
        {
            if (InvokeRequired) Invoke(new WriteLabstringDelegate(MessageBoxShow), str);
            else MessageBox.Show(str);
        }

        private void PackingProgress(string fileName, int packPos, int count)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke(new PCKWriter.PackingDelegate(PackingProgress), new object[] { fileName, packPos, count });
                return;
            }

            if (tssPbr.Maximum != count) tssPbr.Maximum = count;
            tssPbr.Value = packPos;
            tssLab.Text = string.Format("Packing...{0}/{1}", packPos, count);
        }

        private async void PackingFilesThread(object value)
        {
            DisableButtons();
            string rootDir = (string)value;

            int pos = 0;
            List<string> list = PCKWriter.GetFilesInDrops(rootDir,
                delegate (string path, bool complete)
                {
                    pos++;
                    WriteLabPackstring("Counting files..." + pos);
                }
                );

            try
            {
                PCKWriter.Packing(rootDir, list, listPckFile, replacePackFile,
                    delegate (string fileName, int packPos, int count)
                    {
                        WriteLabPackstring(fileName);
                        PackingProgress(fileName, packPos, count);
                    }
                    );

                WriteStatusLabstring("Packing complete");
                WriteLabPackstring("Drag files/folders here");

                EnableButtons();
                await ReadFileListAsync();
            }
            catch (Exception ex)
            {
                WriteStatusLabstring("Packing error");
                WriteLabPackstring("Drag files/folders here");
                EnableButtons();

                MessageBoxShow("Error: " + ex.Message);
            }

            thread = null;
        }

        private void panPack_DragDrop(object sender, DragEventArgs e)
        {
            string[] aryFiles = ((string[])e.Data.GetData(DataFormats.FileDrop));
            if (aryFiles.Length == 1)
            {
                replacePackFile = chbxPack.Checked;
                thread = new Thread(new ParameterizedThreadStart(PackingFilesThread));
                thread.Start(aryFiles[0]);
            }
        }

        private void panPack_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Object data = e.Data.GetData(DataFormats.FileDrop);
                if (data.GetType().Equals(typeof(string[])))
                {
                    string[] list = (string[])data;
                    {
                        if (list.Length == 1)
                        {
                            if (Directory.Exists(list[0]))
                            {
                                e.Effect = DragDropEffects.Move;
                                return;
                            }
                        }
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            OpenOutputFolder();
        }

        static public void OpenOutputFolder()
        {
            string dirOutput = Path.Combine(Environment.CurrentDirectory, "PckOutput");
            if (!Directory.Exists(dirOutput)) Directory.CreateDirectory(dirOutput);
            OpenFolderAndSelectFile(dirOutput, false);
        }

        static public void OpenFolderAndSelectFile(string fileFullName, bool select)
        {
            string strSelect = (select) ? "/select," : "";
            string arguments = string.Format("/e,{0}\"{1}\"", strSelect, fileFullName);
            System.Diagnostics.Process.Start("Explorer.exe", arguments);
        }

        private async void btnCreateFileList_Click(object sender, EventArgs e)
        {
            try
            {
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string fileF00XDAT = Path.Combine(currentDir, "f00X.dat");
                byte[] compressedBytes = File.ReadAllBytes(fileF00XDAT);

                if (compressedBytes.Length == 0) return;

                await Task.Run(() =>
                {
                    for (int x = 0; x < compressedBytes.Length; ++x)
                    {
                        compressedBytes[x] ^= PCKReader.BufferTable[x & 0xFF];
                    }

                    int decompressedSize = (compressedBytes.Length << 4) - compressedBytes.Length;
                    byte[] decompressedBytes = new byte[decompressedSize];

                    int result = ZLibDll.Decompress(compressedBytes, compressedBytes.Length, decompressedBytes, ref decompressedSize);
                    if (result != 0) throw new Exception("Error decoding f00x.dat");

                    List<string> outputList = new();

                    using (MemoryStream ms = new(decompressedBytes, 0, decompressedSize, false))
                    {
                        using BinaryReader br = new(ms);
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            ushort len = br.ReadUInt16();
                            byte[] byteName = br.ReadBytes(len * 2);
                            string name = Encoding.Unicode.GetString(byteName);

                            name = new string(name.ToCharArray()
                                .Where(c => !char.IsControl(c) && c != '\t' && c != '\r' && c != '\n')
                                .ToArray());

                            byte archive = br.ReadByte(); // Which PCK archive is it in
                            int size = br.ReadInt32(); // Size of the file
                            uint hash = br.ReadUInt32(); // hash of the data
                            long offset = br.ReadInt64(); // Offset in the file

                            string outputLine = $"{name} {size} {hash:X8}";
                            outputList.Add(outputLine);
                        }
                    }

                    string outputFilePath = Path.Combine(currentDir, "filelist.txt");
                    File.WriteAllLines(outputFilePath, outputList);
                    MessageBox.Show("Filelist created successfully!");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool haveCheck = false;
                if (listPckFile.Count > 0)
                {
                    SortedDictionary<string, PCKFile> dicPckFile = new();
                    foreach (PCKFile file in listPckFile)
                    {
                        if (file.IsChecked) haveCheck = true;
                        else dicPckFile.Add(file.Name, file);
                    }

                    if (haveCheck)
                    {
                        PCKWriter.WriteF00XDAT(dicPckFile);
                        await ReadFileListAsync();
                    }
                }

                if (!haveCheck) MessageBox.Show("No option selected");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred : {ex.Message}");
            }
        }


        public struct StartEnd
        {
            public long Start;
            public long End;
        }

    }
}
