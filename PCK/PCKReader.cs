using System.Text;

namespace PCKTool.PCK
{
    public static class PCKReader
    {
        public static readonly byte[] BufferTable = new byte[]
                {
                      0x30, 0x22, 0x41, 0xA8, 0x5B, 0xA6, 0x6A, 0x49, 0xBF, 0x53, 0x35, 0xE5, 0x9E, 0x0E, 0xEC, 0xB8, 0x5E, 0x15, 0x1F, 0xC1, 0x4F, 0xEC, 0x77, 0xE8, 0xB7, 0x4E, 0x87, 0xE6, 0xF5, 0x3C, 0xB3, 0x43
                    , 0xCC, 0x53, 0x36, 0xAC, 0x5A, 0x77, 0xB8, 0xDD, 0x30, 0x74, 0x8C, 0x4A, 0x9A, 0x9B, 0xBC, 0x0A, 0xA4, 0xAD, 0xBB, 0x13, 0x4B, 0x8C, 0xD4, 0x80, 0xCE, 0x65, 0x1D, 0x08, 0x5A, 0x6A, 0x6F, 0x25
                    , 0xF9, 0x3F, 0xEF, 0x1B, 0xA4, 0x72, 0x14, 0xED, 0x97, 0x22, 0x4A, 0x2E, 0xB8, 0x96, 0x4B, 0x8E, 0x96, 0x93, 0xF1, 0x28, 0xB2, 0x0B, 0x3C, 0xF8, 0x5D, 0xAA, 0xA9, 0x82, 0x13, 0x6E, 0xC1, 0xA9
                    , 0x20, 0x57, 0xB2, 0x5B, 0x16, 0xCF, 0x9E, 0x5F, 0xD4, 0xCC, 0x2E, 0xF5, 0xC9, 0x4C, 0x1C, 0xEE, 0xE3, 0x3F, 0x29, 0xB3, 0x06, 0x70, 0x43, 0x3D, 0xF5, 0x90, 0xA2, 0x42, 0x02, 0x98, 0x50, 0xFD
                    , 0x5D, 0x4E, 0x92, 0xAD, 0xAD, 0x7F, 0xAB, 0x60, 0x2C, 0xB8, 0x43, 0x76, 0x8F, 0x5F, 0xE6, 0xA7, 0x19, 0xE0, 0xB9, 0xB5, 0x62, 0x6B, 0xD4, 0x47, 0x69, 0x34, 0x0E, 0x6D, 0xA4, 0x52, 0xE3, 0x64
                    , 0x4A, 0x65, 0x47, 0xF5, 0x3F, 0x53, 0x5E, 0x8B, 0x1B, 0xFD, 0x21, 0xF7, 0xBA, 0x68, 0xF9, 0xDF, 0x68, 0xA8, 0x96, 0x0F, 0x8B, 0x01, 0x97, 0x58, 0x8C, 0x1E, 0xEF, 0xB3, 0x41, 0x44, 0x21, 0xDA
                    , 0xE0, 0xF4, 0xE0, 0x2D, 0xCD, 0x0B, 0xF0, 0x5C, 0x59, 0xD6, 0x99, 0xE7, 0x01, 0x15, 0x67, 0x32, 0xE0, 0x12, 0x2F, 0xCD, 0xA2, 0xDE, 0x52, 0xCE, 0xEC, 0xEF, 0x77, 0x0E, 0xBC, 0x38, 0x64, 0x8D
                    , 0xB4, 0xDB, 0x67, 0xFF, 0xC8, 0x66, 0x0C, 0x8A, 0x60, 0xE1, 0x2E, 0x00, 0x43, 0xA9, 0x37, 0x9C, 0x11, 0xAA, 0xB9, 0x98, 0xED, 0x21, 0x35, 0xD4, 0xC3, 0xDE, 0x65, 0x54, 0x9D, 0x1C, 0xB0, 0xA9
                };

        /// <summary>
        /// Whether to run in the game directory
        /// </summary>
        /// <returns></returns>
        public static bool IsRuningInGameDir()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string fileF00XDAT = Path.Combine(currentDir, "f00X.dat");
            return File.Exists(fileF00XDAT);
        }

        /// <summary>
        /// Get all files in PCK
        /// </summary>
        /// <returns></returns>
        public static List<PCKFile> ReadFileList()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string fileF00XDAT = Path.Combine(currentDir, "f00X.dat");

            byte[] compressed_bytes = File.ReadAllBytes(fileF00XDAT);

            if (compressed_bytes.Length == 0) return new List<PCKFile>();

            for (int x = 0; x < compressed_bytes.Length; ++x)
            {
                compressed_bytes[x] ^= BufferTable[x & 0xFF];
            }

            int decompressed_size = (compressed_bytes.Length << 4) - compressed_bytes.Length;
            byte[] decompressed_bytes = new byte[decompressed_size];

            int result = ZLibDll.Decompress(compressed_bytes, compressed_bytes.Length, decompressed_bytes, ref decompressed_size);
            if (result != 0) throw new Exception("Error decoding f00x.dat");

            List<PCKFile> listPck = new(100000);
            using (MemoryStream ms = new(decompressed_bytes, 0, decompressed_size, false))
            {
                using BinaryReader br = new(ms);
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    string name = null;
                    try
                    {
                        ushort len = br.ReadUInt16();
                        byte[] byteName = br.ReadBytes(len * 2);

                        name = Encoding.Unicode.GetString(byteName);
                        byte archive = br.ReadByte(); // Which archive is it in
                        int size = br.ReadInt32(); // Size of the file
                        uint hash = br.ReadUInt32(); // Guessing the hash of the data
                        long offset = br.ReadInt64(); // Offset in the file

                        if (name.Length > 0)
                        {
                            if (name.Contains('/')
                                || name.Contains(':')
                                || name.Contains('*')
                                || name.Contains('?')
                                || name.Contains('<')
                                || name.Contains('>'))
                            {
                                Console.WriteLine("Invalid name:" + name);
                                continue;
                            }

                            PCKFile file = new(name, archive, size, hash, offset);
                            listPck.Add(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error:{0}\n{1}", name, ex);
                    }
                }
            }

            return listPck;
        }

        public delegate void UnpackProgressDelegate(string fileName, int pos, int count);
        /// <summary>
        /// unpack file list
        /// </summary>
        /// <param name="listPckFile"></param>
        public static void Unpack(List<PCKFile> listPckFile, bool replaceUnpack, UnpackProgressDelegate upDelegate)
        {
            Dictionary<int, FileStream> archives = new();
            Dictionary<int, BinaryReader> readers = new();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pck");
            foreach (string file in files)
            {
                FileStream ofs = new(file, FileMode.Open, FileAccess.Read);
                BinaryReader obr = new(ofs);
                int key = int.Parse(Path.GetFileNameWithoutExtension(file));
                archives.Add(key, ofs);
                readers.Add(key, obr);
            }

            int count = listPckFile.Count;
            for (int i = 0; i < count; i++)
            {
                PCKFile pckFile = listPckFile[i];
                upDelegate(pckFile.Name, i + 1, count);
                try
                {
                    string filePath = Directory.GetCurrentDirectory() + "\\PckOutput\\" + pckFile.Name;
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    bool canwrite = true;

                    if (!replaceUnpack)
                        if (File.Exists(filePath)) canwrite = false;

                    if (canwrite)
                    {
                        using FileStream ofs = new(filePath, FileMode.Create, FileAccess.Write);
                        using BinaryWriter bw = new(ofs);
                        BinaryReader br = readers[pckFile.Archive];
                        br.BaseStream.Position = pckFile.Offset;
                        byte[] fileBytes = br.ReadBytes(pckFile.FileSize);
                        bw.Write(fileBytes);
                        bw.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:{0}\n{1}", pckFile.Name, ex);
                }
            }

            foreach (KeyValuePair<int, FileStream> kvp in archives)
            {
                kvp.Value.Close();
                kvp.Value.Dispose();
            }
        }

        /// <summary>
        /// organized into a tree structure
        /// </summary>
        /// <param name="listPckFile"></param>
        /// <returns></returns>
        public static SortedDictionary<string, PCKFileNode> ConvertListToNode(List<PCKFile> listPckFile)
        {
            TrieNode rootNode = new("PCK");
            SortedDictionary<string, PCKFileNode> result = new(new SortstringComparer());

            foreach (PCKFile pckFile in listPckFile)
            {
                string[] elements = pckFile.PathElements;
                TrieNode curNode = rootNode;

                // Traverse the Trie to find the node corresponding to the path elements
                foreach (string element in elements)
                {
                    if (!curNode.Children.TryGetValue(element, out TrieNode childNode))
                    {
                        // Create a new node if it doesn't already exist
                        childNode = new TrieNode(element);
                        curNode.Children.Add(element, childNode);
                    }
                    curNode = childNode;
                }

                // Assign the PCKFile to the leaf node
                curNode.PCKFile = pckFile;
            }

            // Traverse the Trie to create the PCKFileNode tree structure
            Stack<(TrieNode node, SortedDictionary<string, PCKFileNode> parentNodes)> stack = new();
            stack.Push((rootNode, result));
            while (stack.Count > 0)
            {
                (TrieNode node, SortedDictionary<string, PCKFileNode> parentNodes) = stack.Pop();

                PCKFileNode newNode = new(node.Name, node.PCKFile);
                parentNodes.Add(node.Name, newNode);

                if (node.Children.Count > 0)
                {
                    SortedDictionary<string, PCKFileNode> childNodes = new(new SortstringComparer());
                    newNode.Nodes = childNodes;

                    foreach (TrieNode childNode in node.Children.Values)
                    {
                        stack.Push((childNode, childNodes));
                    }
                }
            }

            return result;
        }

        public class TrieNode
        {
            public string Name { get; private set; }
            public PCKFile PCKFile { get; set; }
            public Dictionary<string, TrieNode> Children { get; private set; }

            public TrieNode(string name)
            {
                Name = name;
                Children = new Dictionary<string, TrieNode>();
            }
        }


    }

    /// <summary>
    /// tree sorting
    /// </summary>
    public class SortstringComparer : IComparer<string>
    {
        // Compares by Height, Length, and Width.
        public int Compare(string x, string y)
        {
            int x1 = x.IndexOf(".");
            int y1 = y.IndexOf(".");

            if (x1 >= 0 && y1 < 0) return 1;
            if (x1 < 0 && y1 >= 0) return -1;

            int c = x.CompareTo(y);
            return c;
        }
    }

}
