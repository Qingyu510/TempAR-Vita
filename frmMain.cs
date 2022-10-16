using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempAR
{
    public partial class FrmMain : Form
    {
        // Code types for converter tab
        private const string CT_CNV_CWCHEATPOPS = "CWCheat POPS";

        private const string CT_CNV_NITEPR = "NitePR";
        private const string CT_CNV_R4CCE = "R4CCE 转 CWCheat";
        private const string CT_CNV_TEMPAR = "CWCheat 转 R4CCE";

        // Code types for pointer search tab
        private const string CT_PNT_VITACHEAT = "VitaCheat";
        private const string CT_PNT_CWCHEAT = "CWCheat";
        private const string CT_PNT_AR = "AR";

        // Code types for VitaCheat Code Maker tab
        private const string VC_GEN_WRITE = "写入码 ($0...)";
        private const string VC_GEN_MOV = "传送码 ($5...)";
        private const string VC_GEN_COMP = "压缩码 ($4...)";
        private const string VC_GEN_PNTR = "指针写入码 ($3...)";
        private const string VC_GEN_PTRMOV = "指针传送码 ($8...)";
        private const string VC_GEN_PTRCOM = "指针压缩码 ($7...)";
        private const string VC_GEN_ARMWRT = "ARM写入码 ($A...)";
        private const string VC_GEN_BTNPAD = "按键码 ($C...)";
        private const string VC_GEN_CNDTN = "条件码 ($D...)";
        private const string VC_GEN_B2COD = "B200码 ($B...)";

        // Segment Options for VitaCheat Code Maker tab
        private const string VC_GEN_B2_NONE = "无";
        private const string VC_GEN_B2_SEG0 = "Seg0";
        private const string VC_GEN_B2_SEG1 = "Seg1";

        public int PointerBlk { get; private set; }
        public int PointerGrn { get; private set; }
        public int PointerBlu { get; private set; }
        public int PointerPur { get; private set; }
        public int PointerRed { get; private set; }
        public int PointerOrn { get; private set; }

        public FrmMain()
        {
            InitializeComponent();

            cbCnvCodeTypes.Items.AddRange(new object[] {
            CT_CNV_CWCHEATPOPS,
            CT_CNV_NITEPR,
            CT_CNV_R4CCE,
            CT_CNV_TEMPAR});
            cbCnvCodeTypes.Text = CT_CNV_CWCHEATPOPS;

            cbPntCodeTypes.Items.AddRange(new object[] {
            CT_PNT_VITACHEAT,
            CT_PNT_CWCHEAT,
            CT_PNT_AR});
            cbPntCodeTypes.Text = CT_PNT_VITACHEAT;

            //vita cheat code type drop list text
            comboVitaCheatCodeType.Items.AddRange(new object[] {
            VC_GEN_WRITE,
            VC_GEN_MOV,
            VC_GEN_COMP,
            VC_GEN_PNTR,
            VC_GEN_PTRMOV,
            VC_GEN_PTRCOM,
            VC_GEN_ARMWRT,
            VC_GEN_BTNPAD,
            VC_GEN_CNDTN,
            VC_GEN_B2COD,});
            comboVitaCheatCodeType.Text = VC_GEN_WRITE;

            //vita cheat B200 drop list text
            comboVitaCheatB200.Items.AddRange(new object[] {
            VC_GEN_B2_NONE,
            VC_GEN_B2_SEG0,
            VC_GEN_B2_SEG1,});
            comboVitaCheatB200.Text = VC_GEN_B2_NONE;

            //vita cheat button type drop list text and value
            List<VitaCheatData> ControllerType = new List<VitaCheatData>
            {
                new VitaCheatData() { Type = 9, Name = "无" },
                new VitaCheatData() { Type = 0, Name = "未定义" },
                new VitaCheatData() { Type = 1, Name = "PSVita" },
                new VitaCheatData() { Type = 2, Name = "PSTV" },
                new VitaCheatData() { Type = 4, Name = "DualShock 3" },
                new VitaCheatData() { Type = 8, Name = "DualShock 4" }
            };
            comboVitaCheatButtonType.DataSource = ControllerType;
            comboVitaCheatButtonType.DisplayMember = "Name";
            comboVitaCheatButtonType.ValueMember = "Type";

            //vita cheat button drop list text and value
            List<VitaCheatData> ButtonType1 = new List<VitaCheatData>
            {
                new VitaCheatData() { Type = 0,    Name = "无" },
                new VitaCheatData() { Type = 1,    Name = "SELECT" },
                new VitaCheatData() { Type = 8,    Name = "START" },
                new VitaCheatData() { Type = 10,   Name = "上" },
                new VitaCheatData() { Type = 20,   Name = "右" },
                new VitaCheatData() { Type = 40,   Name = "下" },
                new VitaCheatData() { Type = 80,   Name = "左" },
                new VitaCheatData() { Type = 100,  Name = "L" },
                new VitaCheatData() { Type = 200,  Name = "R" },
                new VitaCheatData() { Type = 1000, Name = "△" },
                new VitaCheatData() { Type = 2000, Name = "○" },
                new VitaCheatData() { Type = 4000, Name = "×" },
                new VitaCheatData() { Type = 8000, Name = "□" }
            };
            comboVitaCheatButton.DataSource = ButtonType1;
            comboVitaCheatButton.DisplayMember = "Name";
            comboVitaCheatButton.ValueMember = "Type";

            //vita cheat 2nd button drop list text and value
            List<VitaCheatData> ButtonType2 = new List<VitaCheatData>
            {
                new VitaCheatData() { Type = 0,    Name = "无" },
                new VitaCheatData() { Type = 1,    Name = "SELECT" },
                new VitaCheatData() { Type = 8,    Name = "START" },
                new VitaCheatData() { Type = 10,   Name = "上" },
                new VitaCheatData() { Type = 20,   Name = "右" },
                new VitaCheatData() { Type = 40,   Name = "下" },
                new VitaCheatData() { Type = 80,   Name = "左" },
                new VitaCheatData() { Type = 100,  Name = "L" },
                new VitaCheatData() { Type = 200,  Name = "R" },
                new VitaCheatData() { Type = 1000, Name = "△" },
                new VitaCheatData() { Type = 2000, Name = "○" },
                new VitaCheatData() { Type = 4000, Name = "×" },
                new VitaCheatData() { Type = 8000, Name = "□" }
            };
            comboVitaCheatButton2.DataSource = ButtonType2;
            comboVitaCheatButton2.DisplayMember = "Name";
            comboVitaCheatButton2.ValueMember = "Type";

            //vita cheat Condition drop list text and value
            List<VitaCheatData> ConditionalType = new List<VitaCheatData>
            {
                new VitaCheatData() { Type = 99, Name = "无" },
                new VitaCheatData() { Type = 0,  Name = "(=) 等于 X (8位)" },
                new VitaCheatData() { Type = 1,  Name = "(=) 等于 X (16位)" },
                new VitaCheatData() { Type = 2,  Name = "(=) 等于 X (32位)" },
                new VitaCheatData() { Type = 3,  Name = "(<>) 不等于 X (8位)" },
                new VitaCheatData() { Type = 4,  Name = "(<>) 不等于 X (16位)" },
                new VitaCheatData() { Type = 5,  Name = "(<>) 不等于 X (32位)" },
                new VitaCheatData() { Type = 6,  Name = "(>) 大于 X (8位)" },
                new VitaCheatData() { Type = 7,  Name = "(>) 大于 X (16位)" },
                new VitaCheatData() { Type = 8,  Name = "(>) 大于 X (32位)" },
                new VitaCheatData() { Type = 9,  Name = "(<) 小于 X (8位)" },
                new VitaCheatData() { Type = 10, Name = "(<) 小于 X (16位)" },
                new VitaCheatData() { Type = 11, Name = "(<) 小于 X (32位)" }
            };
            comboVitaCheatCondition.DataSource = ConditionalType;
            comboVitaCheatCondition.DisplayMember = "Name";
            comboVitaCheatCondition.ValueMember = "Type";

        }

        /// <summary>
        /// Converstion mode Radio Button
        /// 转换器-转换模式单选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdbConvert_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFrameMode(rdbConvertText.Checked);
        }

        /// <summary>
        /// Convert linebreaks uniformly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtTextInput_TextChanged(object sender, EventArgs e)
        {
            txtTextInput.Text = txtTextInput.Text.Replace("\r\n", "\n").Replace("\n", "\r\n");
            BtnConvert_Click(sender, e);
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "工作中...";
            lblStatus.Visible = true;
            Refresh();
            if (rdbConvertText.Checked)
            {
                switch (cbCnvCodeTypes.Text)
                {
                    case CT_CNV_CWCHEATPOPS:
                        txtTextOutput.Text = Converter.Cwcpops_pspar(txtTextInput.Text);
                        break;

                    case CT_CNV_NITEPR:
                        txtTextOutput.Text = Converter.Nitepr_pspar(txtTextInput.Text);
                        break;

                    case CT_CNV_R4CCE:
                        txtTextOutput.Text = Converter.Reformat_r4cce(txtTextInput.Text, true);
                        break;

                    case CT_CNV_TEMPAR:
                        txtTextOutput.Text = Converter.Reformat_tempar(txtTextInput.Text);
                        break;

                    default:
                        break;
                }
            }
            else if (txtInputPath.Text.Length > 0 && txtOutputPath.Text.Length > 0)
            {
                switch (cbCnvCodeTypes.Text)
                {
                    case CT_CNV_CWCHEATPOPS:
                        if (File.Exists(txtInputPath.Text) && Directory.Exists(Path.GetDirectoryName(txtOutputPath.Text)))
                            Converter.File_cwcpops_pspar(txtInputPath.Text, txtOutputPath.Text);
                        break;

                    case CT_CNV_NITEPR:
                        if (Directory.Exists(txtInputPath.Text) && Directory.Exists(Path.GetDirectoryName(txtOutputPath.Text)))
                            Converter.File_nitepr_pspar(txtInputPath.Text, txtOutputPath.Text);
                        break;

                    case CT_CNV_R4CCE:
                        MessageBox.Show("目前不支持此代码类型的文件转换！");
                        break;

                    case CT_CNV_TEMPAR:
                        if (File.Exists(txtInputPath.Text) && Directory.Exists(Path.GetDirectoryName(txtOutputPath.Text)))
                            Converter.File_reformat_tempar(txtInputPath.Text, txtOutputPath.Text);
                        break;

                    default:
                        break;
                }
            }
            lblStatus.Visible = false;
        }

        private void BtnInputBrowse_Click(object sender, EventArgs e)
        {
            switch (cbCnvCodeTypes.SelectedIndex)
            {
                case 0:
                case 1:
                    txtInputPath.Text = Utils.OpenFile(txtInputPath.Text, "CWCheat 数据库文件 (*.db)|*.db", "打开");
                    break;

                case 2:
                    txtInputPath.Text = Utils.OpenDirectory(txtInputPath.Text, "选择您的 NitePR 代码文件目录：");
                    break;

                case 3:
                    MessageBox.Show("目前不支持此代码类型的文件转换！");
                    break;

                default:
                    break;
            }
        }

        private void BtnOutputBrowse_Click(object sender, EventArgs e)
        {
            txtOutputPath.Text = Utils.SaveFile(txtOutputPath.Text, "CWCheat 数据库文件 (*.db)|*.db", "保存");
        }

        /// <summary>
        /// Select all with Ctrl+S
        /// 实现ctrl+a全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFieldSelectAll(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBoxBase)sender).SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else
                OnKeyDown(e);
        }

        /// <summary>
        /// Converter Code type mode switch
        /// 代码转换器- 文本文件模式切换
        /// </summary>
        /// <param name="mode"></param>
        private void ChangeFrameMode(bool mode)
        {
            if (mode)
            {
                pnlConvertText.BringToFront();
            }
            else
            {
                pnlConvertFile.BringToFront();
                if (!String.IsNullOrEmpty(cbCnvCodeTypes.Text) && cbCnvCodeTypes.Text == CT_CNV_R4CCE)
                {
                    cbCnvCodeTypes.Text = CT_CNV_CWCHEATPOPS;
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            comboPointerSearcherMode.SelectedIndex = 0;
            comboVitaCheatCodeType.SelectedIndex = 0;
            comboVitaCheatB200.SelectedIndex = 0;
            comboVitaCheatPointerLevel.SelectedIndex = 0;
            comboVitaCheatButtonType.SelectedIndex = 0;
            comboVitaCheatButton.SelectedIndex = 0;
            comboVitaCheatButton2.SelectedIndex = 0;
        }

        //
        //
        // Pointer Searcher Tab starts here
        //
        //
        private PointerSearcher memdump;
        private PointerSearcher memdump2;
        private PointerSearcher memdump3;
        private PointerSearcher memdump4;
        private PointerSearcher memdump5;
        private PointerSearcher memdump6;
        private uint memory_start;

        private void BtnPointerSearcherFindPointers_Click(object sender, EventArgs e)
        {
            uint address1 = Utils.ParseNum(txtPointerSearcherAddress1.Text, NumberStyles.AllowHexSpecifier);
            uint maxOffset = Utils.ParseNum(txtPointerSearcherMaxOffset.Text, NumberStyles.AllowHexSpecifier);
            memory_start = Utils.ParseNum(txtBaseAddress.Text, NumberStyles.AllowHexSpecifier);

            memdump  = new PointerSearcher(txtPointerSearcherMemDump1.Text, memory_start);
            memdump2 = new PointerSearcher(txtPointerSearcherMemDump2.Text, memory_start);
            memdump3 = new PointerSearcher(txtPointerSearcherMemDump3.Text, memory_start);
            memdump4 = new PointerSearcher(txtPointerSearcherMemDump4.Text, memory_start);
            memdump5 = new PointerSearcher(txtPointerSearcherMemDump5.Text, memory_start);
            memdump6 = new PointerSearcher(txtPointerSearcherMemDump6.Text, memory_start);

            ResetPointerCounts();

            treePointerSearcherPointers.BeginUpdate();
            treePointerSearcherPointers.Nodes.Clear();
            AddPointerTree(memdump.FindPointers(address1, maxOffset), treePointerSearcherPointers.SelectedNode);
            treePointerSearcherPointers.EndUpdate();
        }

        private void TreePointerSearcherPointers_DoubleClick(object sender, EventArgs e)
        {
            if (treePointerSearcherPointers.SelectedNode == null) return;

            var maxOffset = Utils.ParseNum(txtPointerSearcherMaxOffset.Text, NumberStyles.AllowHexSpecifier);
            treePointerSearcherPointers.SelectedNode.Nodes.Clear();
            AddPointerTree(memdump.FindPointers(new PointerSearcherLog(treePointerSearcherPointers.SelectedNode.Text, memory_start).Address, maxOffset), treePointerSearcherPointers.SelectedNode);
        }

        private void BtnPointerSearcherClear_Click(object sender, EventArgs e)
        {
            treePointerSearcherPointers.BeginUpdate();
            treePointerSearcherPointers.Nodes.Clear();
            treePointerSearcherPointers.EndUpdate();
            ResetPointerCounts();
        }

        private void ResetPointerCounts()
        {
            txtColorBlack.Text = "0";
            txtColorGreen.Text = "0";
            txtColorBlue.Text = "0";
            txtColorOrchid.Text = "0";
            txtColorRed.Text = "0";
            txtColorOrange.Text = "0";

            PointerBlk = 0;
            PointerGrn = 0;
            PointerBlu = 0;
            PointerPur = 0;
            PointerRed = 0;
            PointerOrn = 0;
        }

        private void TreePointerSearcherPointers_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treePointerSearcherPointers.SelectedNode == null) return;
            var pointers = new List<PointerSearcherLog>();
            var strArray = treePointerSearcherPointers.SelectedNode.FullPath.ToString().Split('\\');
            for (int index = 0; index < strArray.Length; ++index)
            {
                pointers.Add(new PointerSearcherLog(strArray[strArray.Length - 1 - index], memory_start));
            }

            var num = Utils.ParseNum(txtPointerSearcherValue.Text);
            var bittype = 2;
            if (rdbPointerSearcherBitType16.Checked)
            {
                bittype = 1;
                num &= (uint)ushort.MaxValue;
            }
            else if (rdbPointerSearcherBitType8.Checked)
            {
                bittype = 0;
                num &= (uint)byte.MaxValue;
            }
            //
            // Check which code is being generated
            //
            switch (cbPntCodeTypes.Text)
            {
                case CT_PNT_VITACHEAT:
                    txtPointerSearcherCode.Text = GetVitaCheatPointerCode(pointers, bittype, num).Replace("\n", "\r\n");
                    break;

                case CT_PNT_CWCHEAT:
                    txtPointerSearcherCode.Text = GetCWCheatPointerCode(pointers, bittype, num).Replace("\n", "\r\n");
                    break;

                case CT_PNT_AR:
                    txtPointerSearcherCode.Text = GetARPointerCode(pointers, bittype, num).Replace("\n", "\r\n");
                    break;

                default:
                    break;
            }
        }

        private void TxtPointerSearcherMemDump_Click(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = Utils.OpenFile(((TextBox)sender).Text, null, "打开文件...");
        }

        private void TreePointerSearcherPointers_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Space:
                    TreePointerSearcherPointers_DoubleClick(null, null);
                    break;

                case Keys.Delete:
                    if (treePointerSearcherPointers.SelectedNode == null) break;
                    treePointerSearcherPointers.SelectedNode.Remove();
                    break;

                default:
                    break;
            }
        }

        private async void AddPointerTree(List<PointerSearcherLog> pointers, TreeNode parent)
        {
            if (pointers == null)
                return;

            Utils.SortList<PointerSearcherLog>(pointers, "Address", true);
            if (chkPointerSearcherOptimizePointerPaths.Checked)
            {
                var pointerSearcherLogList = new List<PointerSearcherLog>();
                if (treePointerSearcherPointers.Nodes.Count > 0)
                {
                    var parentEqualTree = GetParentEqualTree(treePointerSearcherPointers.Nodes, treePointerSearcherPointers.SelectedNode == null ? 0 : treePointerSearcherPointers.SelectedNode.Level);
                    for (int index = 0; index < parentEqualTree.Count; ++index)
                    {
                        pointerSearcherLogList.Add(new PointerSearcherLog(parentEqualTree[index].Text, memory_start));
                    }
                }
                for (int index1 = 0; index1 < pointerSearcherLogList.Count; ++index1)
                {
                    for (int index2 = 0; index2 < pointers.Count; ++index2)
                    {
                        if ((int)pointerSearcherLogList[index1].Address == (int)pointers[index2].Address)
                            pointers.RemoveAt(index2);
                    }
                }
            }
            for (int index1 = 0; index1 < pointers.Count; ++index1)
            {
                Color color = Color.Black;
                Color rootedColor = Color.Transparent;
                int PointerColor = 0;
                string[] strArray = ((treePointerSearcherPointers.SelectedNode == null ? "" : treePointerSearcherPointers.SelectedNode.FullPath + "\\") + pointers[index1]).Split('\\');

                PointerColor = await SearchAdditionalDumps(memdump2, txtPointerSearcherAddress2.Text, strArray, PointerColor);
                PointerColor = await SearchAdditionalDumps(memdump3, txtPointerSearcherAddress3.Text, strArray, PointerColor);
                PointerColor = await SearchAdditionalDumps(memdump4, txtPointerSearcherAddress4.Text, strArray, PointerColor);
                PointerColor = await SearchAdditionalDumps(memdump5, txtPointerSearcherAddress5.Text, strArray, PointerColor);
                PointerColor = await SearchAdditionalDumps(memdump6, txtPointerSearcherAddress6.Text, strArray, PointerColor);

                switch (PointerColor)
                {
                    case 0:
                        color = Color.Black;
                        PointerBlk += 1;
                        break;

                    case 1:
                        color = Color.Green;
                        PointerGrn += 1;
                        break;

                    case 2:
                        color = Color.Blue;
                        PointerBlu += 1;
                        break;

                    case 3:
                        color = Color.DarkOrchid;
                        PointerPur += 1;
                        break;

                    case 4:
                        color = Color.Red;
                        PointerRed += 1;
                        break;

                    case 5:
                        color = Color.Chocolate;
                        PointerOrn += 1;
                        break;

                    default:
                        break;
                }

                string seg0Start = txtPointerSearcherSeg0Addr.Text;
                string seg0Range = txtPointerSearcherSeg0Range.Text;
                string seg1Start = txtPointerSearcherSeg1Addr.Text;
                string seg1Range = txtPointerSearcherSeg1Range.Text;

                if (Utils.CheckInsideSegments(pointers[index1].Address, seg0Start, seg0Range) ||
                    Utils.CheckInsideSegments(pointers[index1].Address, seg1Start, seg1Range))
                {
                    rootedColor = Color.PowderBlue;
                }

                string vitaCheatStart = txtPointerSearcherVitaCheatSeg1Address.Text;
                string vitaCheatRange = txtPointerSearcherVitaCheatSeg1Range.Text;

                if (Utils.CheckInsideSegments(pointers[index1].Address, vitaCheatStart, vitaCheatRange))
                {
                    if (chkIgnoreDBFiles.Checked) { continue; }
                    rootedColor = Color.Black;
                    color = Color.White;
                }

                if (!pointers[index1].Negative || chkPointerSearcherIncludeNegatives.Checked)
                {
                    var node = new TreeNode
                    {
                        Text = pointers[index1].ToString(chkPointerSearcherRealAddresses.Checked ? 0U : memory_start),
                        ForeColor = color,
                        BackColor = rootedColor
                    };
                    if (parent == null)
                    {
                        treePointerSearcherPointers.Nodes.Add(node);
                    }
                    else
                    {
                        parent.Nodes.Add(node);
                    }
                }
            }

            TotalPointersPerColor();
        }

        private void TotalPointersPerColor()
        {
            txtColorBlack.Text  = PointerBlk.ToString();
            txtColorGreen.Text  = PointerGrn.ToString();
            txtColorBlue.Text   = PointerBlu.ToString();
            txtColorOrchid.Text = PointerPur.ToString();
            txtColorRed.Text    = PointerRed.ToString();
            txtColorOrange.Text = PointerOrn.ToString();
        }

        private async Task<int> SearchAdditionalDumps(PointerSearcher dump, string txtAddress, string[] strArray, int PointerColor)
        {
            await Task.Run(() =>
            {
                if (!String.IsNullOrEmpty(txtAddress))
                {
                    uint num = Utils.ParseNum(txtAddress, NumberStyles.AllowHexSpecifier);
                    if (num < memory_start) num += memory_start;
                    uint address = 0u;
                    for (int index2 = 0; index2 < strArray.Length; ++index2)
                    {
                        PointerSearcherLog pointerSearcherLog = new PointerSearcherLog(strArray[strArray.Length - 1 - index2], memory_start);
                        if (index2 == 0) address = pointerSearcherLog.Address;
                        address = dump.GetPointerAddress(address, pointerSearcherLog.Offset, pointerSearcherLog.Negative);
                    }
                    if ((int)num == (int)address) PointerColor += 1;
                }
            });
            return PointerColor;
        }

        private TreeNodeCollection GetParentEqualTree(TreeNodeCollection nodes, int level)
        {
            using (var treeView = new TreeView())
            {
                foreach (TreeNode node in nodes)
                {
                    if (node.Level <= level)
                    {
                        treeView.Nodes.Add(node.Text);
                        foreach (TreeNode treeNode in GetParentEqualTree(node.Nodes, level))
                            treeView.Nodes.Add(treeNode.Text);
                    }
                }
                return treeView.Nodes;
            }
        }

        //
        // Default values for "Base Address"
        //
        private void ComboPointerSearcherMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBaseAddress.Enabled = false;
            switch (comboPointerSearcherMode.SelectedIndex)
            {
                case 0: // Sony Vita
                    memory_start = 0x81000000U;
                    break;

                case 1: // Sony PSP
                    memory_start = 0x8800000U;
                    break;

                case 2: // Nintendo DS
                    memory_start = 0x2000000U;
                    break;

                case 3: // Other..
                    memory_start = 0x0U;
                    txtBaseAddress.Enabled = true;
                    break;

                default:
                    break;
            }
            txtBaseAddress.Text = string.Format("0x{0:X08}", (object)memory_start);
        }

        //
        // AR Code Generation
        //
        private string GetARPointerCode(List<PointerSearcherLog> pointers, int bittype, uint value)
        {
            switch (bittype)
            {
                case 0:
                    bittype = 2;
                    break;

                case 1:
                    bittype = 1;
                    break;

                case 2:
                    bittype = 0;
                    break;

                default:
                    bittype = 0;
                    break;
            }
            string str1 = "";
            for (int index = 0; index < pointers.Count; ++index)
                str1 = !pointers[index].Negative ? str1 + string.Format("{0:X01}{1:X07} {2:X08}\n", (object)(index == pointers.Count - 1 ? bittype : 11), (object)pointers[index].Offset, (object)(uint)(index == pointers.Count - 1 ? (int)value : 0)) : str1 + string.Format("DC000000 {0:X08}\n{1:X01}0000000 {2:X08}\n", (object)(4294967296L - (long)pointers[index].Offset), (object)(index == pointers.Count - 1 ? bittype : 11), (object)(uint)(index == pointers.Count - 1 ? (int)value : 0));
            string str2 = string.Format("6{0:X07} 00000000\nB{0:X07} 00000000\n{1}D2000000 00000000", (object)pointers[0].Address, (object)str1);
            return (!chkPointerSearcherRAWCode.Checked ? "" : "::生成代码\n") + str2;

            

        }

        //
        // VitaCheat Code Generation
        //
        private string GetVitaCheatPointerCode(List<PointerSearcherLog> pointers, int bittype, uint value)
        {
            switch (bittype)
            {
                case 0:
                    bittype = 0;
                    break;

                case 1:
                    bittype = 1;
                    break;

                case 2:
                    bittype = 2;
                    break;

                default:
                    bittype = 2;
                    break;
            }

            var str1 = "";
            var str2 = "";
            var str3 = $"$3300 00000000 {value:X08}\n";
            for (int index = 1; index < pointers.Count; ++index)
            {
                str1 = !pointers[index].Negative ? $"{str1}$3{bittype}00 00000000 {pointers[index].Offset:X08}\n" : $"{str1}$3{bittype}00 00000000 {(0x100000000L - pointers[index].Offset):X08}\n";
            }

            if (pointers.Count > 1) str1 += string.Format("");

            uint targetAddress = pointers[0].Address;
            string strB200 = "";
            GenerateB200(pointers, ref targetAddress, ref strB200);

            str2 = !pointers[0].Negative ? $"{str2}$3{bittype:X01}{pointers.Count:X02} {targetAddress:X08} {pointers[0].Offset:X08}\n" + str1 : $"{str2}$3{bittype:X01}{pointers.Count:X02} {targetAddress:X08} {(0x100000000L - pointers[0].Offset):X08}\n{str1}";

            return (!chkPointerSearcherRAWCode.Checked ? $"{strB200}{str2}{str3}" : $"_V0 生成代码\n{strB200}{str2}{str3}");
        }

        private void GenerateB200(List<PointerSearcherLog> pointers, ref uint targetAddress, ref string strB200)
        {
            if (chkPointerSearcherB200.Checked)
            {
                string seg0Start = txtPointerSearcherSeg0Addr.Text;
                string seg0Range = txtPointerSearcherSeg0Range.Text;
                string seg1Start = txtPointerSearcherSeg1Addr.Text;
                string seg1Range = txtPointerSearcherSeg1Range.Text;

                if (Utils.CheckInsideSegments(pointers[0].Address, seg0Start, seg0Range))
                {
                    strB200 = $"$B200 00000000 00000000\n";
                    targetAddress -= Utils.ParseNum(seg0Start, NumberStyles.AllowHexSpecifier);
                }
                else if (Utils.CheckInsideSegments(pointers[0].Address, seg1Start, seg1Range))
                {
                    strB200 = $"$B200 00000001 00000000\n";
                    targetAddress -= Utils.ParseNum(seg1Start, NumberStyles.AllowHexSpecifier);
                }
            }
        }

        //
        // CWCheat Code Generation
        //
        private string GetCWCheatPointerCode(List<PointerSearcherLog> pointers, int bittype, uint value)
        {
            if (bittype != 0 && bittype != 1 && bittype != 2) bittype = 2;
            if (pointers[0].Negative) bittype += 3;
            var str1 = "";
            for (int index = 0; index < pointers.Count - 1; ++index)
            {
                str1 = index % 2 != 0 ? str1 + $" 0x{(pointers[index].Negative ? 3 : 2):X01}{pointers[index].Offset:X07}\n" : $"{str1}{(!chkPointerSearcherRAWCode.Checked ? "" : "_L ")}0x{(pointers[index].Negative ? 3 : 2):X01}{pointers[index].Offset:X07}";
            }

            if (pointers.Count % 2 == 0) str1 += string.Format(" 0x00000000");

            var str2 = $"{(!chkPointerSearcherRAWCode.Checked ? "" : "_L ")}0x6{(uint)((int)pointers[0].Address - (int)memory_start):X07} 0x{value:X08}\n{(!chkPointerSearcherRAWCode.Checked ? "" : "_L ")}0x000{bittype:X01}{pointers.Count:X04} 0x{pointers[pointers.Count - 1].Offset:X08}\n{str1}";

            return (!chkPointerSearcherRAWCode.Checked ? $"{str2}" : $"_C0 生成代码\n{str2}");
        }

        private void TxtFileDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TxtFileDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            ((Control)sender).Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
        }

        private void TxtValidateHexString_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Regex.IsMatch(e.KeyChar.ToString(), "[^0-9a-fA-F\x0001\x0003\b\x0016]")) return;
            e.Handled = true;
        }

        private void CbCodeTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;

            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                btnConvert.Enabled = true;
                BtnConvert_Click(sender, e);
            }
        }

        private void TxtPointerSearcherMemDump_TextChanged(object sender, EventArgs e)
        {
            txtPointerSearcherMemDump2.Enabled = txtPointerSearcherAddress2.Enabled = txtPointerSearcherMemDump1.Text.Length > 0;
            txtPointerSearcherMemDump3.Enabled = txtPointerSearcherAddress3.Enabled = txtPointerSearcherMemDump2.Text.Length > 0;
            txtPointerSearcherMemDump4.Enabled = txtPointerSearcherAddress4.Enabled = txtPointerSearcherMemDump3.Text.Length > 0;
            txtPointerSearcherMemDump5.Enabled = txtPointerSearcherAddress5.Enabled = txtPointerSearcherMemDump4.Text.Length > 0;
            txtPointerSearcherMemDump6.Enabled = txtPointerSearcherAddress6.Enabled = txtPointerSearcherMemDump5.Text.Length > 0;
        }
        private void TxtVCInstructions_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
        private void ComboVitaCheatButtonType_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (comboVitaCheatButtonType.SelectedValue)
            {
                case 9:
                    comboVitaCheatButton.Enabled = false;
                    comboVitaCheatButton2.Enabled = false;
                    break;

                default:
                    comboVitaCheatButton.Enabled = true;
                    comboVitaCheatButton2.Enabled = true;
                    break;
            }
        }

        private void ComboVitaCheatCondition_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (comboVitaCheatCondition.SelectedValue)
            {
                case 99:
                    txtVitaCheatCondAddr.Enabled = false;
                    txtVitaCheatCondValue.Enabled = false;
                    break;

                default:
                    txtVitaCheatCondAddr.Enabled = true;
                    txtVitaCheatCondValue.Enabled = true;
                    break;

            }
        }

        private void ComboVitaCheatCodeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Git Wiki url
            var strWiki = "https://github.com/r0ah/vitacheat/wiki/";
            txtVitaCheatAddress2.Enabled = false;
            txtVitaCheatValue.Enabled = true;
            comboVitaCheatPointerLevel.Enabled = false;
            groupVitaCheatAddress1Offset.Enabled = false;
            groupVitaCheatAddress2Offset.Enabled = false;
            groupVitaCheatCompression.Enabled = false;
            pnlVitaCheatMain.Enabled = true;
            btnVitaCheatGenerate.Enabled = true;
            string strPage;
            switch (comboVitaCheatCodeType.Text)
            {
                case VC_GEN_WRITE: // Write
                    strPage = "Write";
                    txtVCInstructions.Text = "写入码\r\n\r\n创建一个代码，该代码会将地址处的值锁定为指定的数值。\r\n\r\n将所需地址放入“目标地址”框中，然后将所需值放入标有“数值”的框中\r\n\r\n例如，要将HP锁定为100，我们需要将HP的地址（我将使用83001337）放入“目标地址”，将100放入“数值”。\r\n\r\n这将生成代码：\r\n_V0 无限 HP\r\n$0200 83001337 00000064\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_MOV: // MOV
                    strPage = "MOV";
                    txtVitaCheatAddress2.Enabled = true;
                    txtVitaCheatValue.Enabled = false;
                    txtVCInstructions.Text = "传送码\r\n传送码能将值从一个地址复制到另一个地址上。\r\n\r\n将要更改的地址放入“目标地址”，将要从中复制数值的地址放入“来源地址”。\r\n\r\n例：\r\n要创建“始终满HP”代码，我们可以将当前HP（83001337）的地址放入“目标地址”中。然后将我们的最大HP（83001333）的地址放入“来源地址”。代码生成器将给出以下代码：\r\n_V0 始终满HP\r\n$5200 83001337 83001333\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_COMP: // Compress
                    strPage = "Compression";
                    groupVitaCheatCompression.Enabled = true;
                    numericVitaCheatCompressionLevelOffset.Enabled = false;
                    lblVitaCheatCompressionLevelOffset.Enabled = false;
                    txtVCInstructions.Text = "压缩码\r\n压缩码是一种高级代码，它以有序的方式在不同位置多次应用“写入”码。\r\n\r\n在“目标地址”框中放入第一个地址，并在“数值”中写入所需值。\r\n\r\n计算与第二个地址间的距离。您可以使用十六进制计算器相减这两个地址。将该偏移量放入标有“地址间隔”的框中\r\n\r\n如果您希望在每次应用代码时数值有序增加，请使用“数值间隔”来增加该值。\r\n\r\n最后，在“压缩次数”框中选择或输入需要此代码重复的次数。\r\n\r\n例：\r\n为了给每种药水改为99个，我们将首先找到游戏中第1种药水的地址（我们将使用83001337），并知道有多少药水。我们假设有普通药水、大药水和高级药水，所以总共3次压缩。大药水为83001347，高级药水83001357。这会将值偏移量设置为0x00000010。我们希望它们都为99，因此“数值”将为99，“数值间隔”将保持为0。然后，生成的代码将是：\r\n_V0 无限药水\r\n$4200 83001337 00000063\r\n$0003 00000010 00000000\r\n\r\n这与以下代码具有相同的效果：\r\n_V0 无限药水\r\n$0200 83001337 00000063\r\n$0200 83001347 00000063\r\n$0200 83001357 00000063\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_PNTR: // Pointer Write
                    strPage = "Pointer-Write";
                    comboVitaCheatPointerLevel.Enabled = true;
                    groupVitaCheatAddress1Offset.Enabled = true;
                    txtVCInstructions.Text = "指针写入码\r\n指针是写入会移动地址的高级代码。\r\n\r\n有时开发人员会移动RAM区域。为了跟踪此移动，特定地址会跟踪该区域的起点。该区域内地址的位置称为偏移，是从区域的起点到所需位置的距离。通常，该位置是另一个指针，导致出现新的可移动区域。要跟踪第二个、第三个或更多指针，请使用多级指针。\r\n\r\n将指针的地址放入“目标地址”框中，并将所需值放入“数值”框中。\r\n\r\n在“指针级数”框中选择需要跟踪的指针数，并将其每个偏移量依次放入偏移框中。第一个偏移量位于顶部，最后一个偏移量位于底部。\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_PTRMOV: // Pointer MOV
                    strPage = "Pointer-MOV";
                    comboVitaCheatPointerLevel.Enabled = true;
                    groupVitaCheatAddress1Offset.Enabled = true;
                    groupVitaCheatAddress2Offset.Enabled = true;
                    txtVitaCheatValue.Enabled = false;
                    txtVitaCheatAddress2.Enabled = true;
                    txtVCInstructions.Text = "指针传送码\r\n指针传送码使用指针作为前提，将一个地址复制到另一个地址。\r\n\r\n例子\r\n\r\n一级指针传送码的示例：\r\n\r\n_V0 无限 MP\r\n$8201 818714E8 00000E0C\r\n$8800 00000000 00000000\r\n$8601 815715D9 00000FDC\r\n$8900 00000000 00000000\r\n\r\n将(32位)值从\r\n[0x815715D9]+0xFDC\r\n复制到\r\n[0x818714E8]+0xE0C\r\n\r\n三级指针传送码的示例：\r\n\r\n_V0 无限 MP\r\n$8203 818714E8 00000E0C\r\n$8200 00000000 0000000D\r\n$8200 00000000 0000124D\r\n$8800 00000000 00000000\r\n$8603 815715D9 00000FDC\r\n$8600 00000000 0000000C\r\n$8600 00000000 00001255\r\n$8900 00000000 00000000\r\n\r\n将(32位)值从\r\n[[[0x815715D9]+0xFDC]+0x0C]+0x1255\r\n复制到\r\n[[[0x818714E8]+0xE0C]+0x0D]+0x124D\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_PTRCOM: // Pointer Compress
                    strPage = "Pointer-Compression";
                    comboVitaCheatPointerLevel.Enabled = true;
                    groupVitaCheatAddress1Offset.Enabled = true;
                    groupVitaCheatCompression.Enabled = true;
                    numericVitaCheatCompressionLevelOffset.Enabled = true;
                    lblVitaCheatCompressionLevelOffset.Enabled = true;
                    txtVCInstructions.Text = "指针压缩码\r\n以指针为基础，有序方式创建多个“写入”代码。\r\n\r\n确保设置代码要压缩的指针级数。将其设置为“1”将在第一个偏移处应用压缩。\r\n将其设置为“0”，将通过所选“偏移间隔”更改地址值，而不是偏移量。\r\n\r\n例：\r\n\r\n_V0 状态MAX\r\n$7203 818714E8 00000FDC\r\n$7200 00000000 0000000C\r\n$7200 00000000 000005DD\r\n$7703 00000000 000003E6\r\n$0002 00000004 00000000\r\n\r\n压缩第三个指针偏移量0x05DD，地址相差0x04。压缩次数为0x02。\r\n\r\n上面的代码等价于:\r\n\r\n$3203 818714E8 00000FDC\r\n$3200 00000000 0000000C\r\n$3200 00000000 000005DD\r\n$3300 00000000 000003E6\r\n\r\n\r\n$3203 818714E8 00000FDC\r\n$3200 00000000 0000000C\r\n$3200 00000000 000005E1\r\n$3300 00000000 000003E6\r\n #递增地址差距:`0x05DD`+`0x04`=`0x05E1`\r\n#数值为静态的 3E6\r\nC\r\n更改具有已知地址间隔的地址\r\n\r\n_V0 WH 道具槽修改器\r\n$7201 862BFEC8 0000001C\r\n$7700 00000000 00000032\r\n$0060 00000004 00000000\r\n\r\n压缩指针地址为0x862BFEC8，地址间隔0x04，压缩次数0x60。\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_ARMWRT: // ARM Write
                    strPage = "ARM-Write";
                    txtVitaCheatAddress2.Enabled = true;
                    txtVitaCheatValue.Enabled = false;
                    txtVCInstructions.Text = "ARM写入码\r\n编写ARM指令。ARM写入码还可用于存储被覆盖内容的原始值，并在关闭代码时还原。\r\n\r\n这就像一个开关，用于作弊的ON和OFF，以将其恢复到以前的值。\r\n\r\n注意：ARM体系结构使用小端存储格式。\r\n\r\n例：\r\n_V0 EXTRA最大值\r\n$A100 8114C8A2 0000BF00\r\n\r\n将0x00BF写入0x8114C8A2(16位)，默认值为关闭。\r\n\r\n0x00BF在ARM中的表示为 NOP(Thumb-2 HEX)。\r\n\r\n_V1 分支测试\r\n$A200 81132EA8 EA01D709\r\n\r\n将0x75F012BE写入0x81132EA8(32位)，默认值为打开。\r\n\r\n0x09D701EA在ARM中(ARM GBD/LLBD)中表示为 b #0x75C24。\r\n\r\n_V0 穿过墙壁\r\n$A000 810A12A6 00000001\r\n\r\n将0x01写入地址0x810A12A6，默认值为0x00\r\n关闭代码后，它将还原为原始值0x00。\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_BTNPAD: // Button Pad
                    strPage = "Button-PAD";
                    pnlVitaCheatMain.Enabled = false;
                    btnVitaCheatGenerate.Enabled = false;
                    txtVCInstructions.Text = "按键码\r\n当你希望根据输入对应按键激活代码时，按键码会很有用。\r\n\r\n按键类型\r\n\r\n0000 - 未定义\r\n0001 - Vita (默认)\r\n0002 - PSTV\r\n0004 - DualShock 3\r\n0008 - DualShock 4\r\n\r\n按键\r\n00000001 - psvita-SELECT\r\n00000008 - psvita-START\r\n00000010 - psvita-上\r\n00000020 - psvita-右\r\n00000040 - psvita-下\r\n00000080 - psvita-左\r\n00000100 - psvita-Ｌ\r\n00000200 - psvita-Ｒ\r\n00001000 - psvita-△\r\n00002000 - psvita-○\r\n00004000 - psvita-×\r\n00008000 - psvita-□\r\n00000000 - 无\r\n\r\n例：\r\n\r\n_V0 按键码\r\n$C201 00000001 00000300\r\n$0200 8xxxxxxx xxxxxxxx\r\n\r\n按下“psvita-Ｌ”+“psvita-Ｒ”将执行以下0x01个数量相关代码行\r\n$0200 8xxxxxxx xxxxxxxx\r\n\r\n按键组合\r\n“psvita-□”+“psvita-SELECT”:\r\n\r\n_V0 Button PAD\r\n$C201 00000001 00008001\r\n$0200 8xxxxxxx xxxxxxxx\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_CNDTN: // Condition
                    strPage = "Condition";
                    pnlVitaCheatMain.Enabled = false;
                    btnVitaCheatGenerate.Enabled = false;
                    txtVCInstructions.Text = "条件码\r\n条件码会检查是否满足特定要求，然后执行代码。有关更多详细信息，请阅读条件（计算机编程）。\r\n\r\n目前仅支持静态地址。\r\n\r\nX：运算符\r\n0 - (=) 等于 X (8位)\r\n1 - (=) 等于 X (16位)\r\n2 - (=) 等于 X (32位)\r\n3 - (<>) 不等于 X (8位)\r\n4 - (<>) 不等于 X (16位)\r\n5 - (<>) 不等于 X (32位)\r\n6 - (>) 大于 X (8位)\r\n7 - (>) 大于 X (16位)\r\n8 - (>) 大于 X (32位)\r\n9 - (<) 小于 X (8位)\r\nA - (<) 小于 X (16位)\r\nB - (<) 小于 X (32位)\r\n\r\n例子\r\n\r\n运算符：等于 X (=)\r\n\r\n_V0 条件运算码\r\n$D201 81000000 FFA8FF2D #代码类型 标识符\r\n$0200 8xxxxxxx xxxxxxxx #行 #1\r\n\r\n如果0x81000000的值等于0xFFA8FF2D(32位)，则执行以下0x01个相关代码行。\r\n\r\n运算符：大于 X (>)\r\n\r\n__V0 条件运算码\r\n$D605 81000000 00000005 #代码类型 标识符\r\n$0200 8xxxxxxx xxxxxxxx #行 #1\r\n$0200 8xxxxxxx xxxxxxxx #行 #2\r\n$0200 8xxxxxxx xxxxxxxx #行 #3\r\n$0200 8xxxxxxx xxxxxxxx #行 #4\r\n$0200 8xxxxxxx xxxxxxxx #行 #5\r\n\r\n如果0x81000000的值大于0x00000005(8位)，则执行以下0x05个相关代码行。\r\n\r\n运算符：小于 X (<)\r\n\r\n_V0 条件运算码\r\n$D90A 81000000 00000005 #代码类型 标识符\r\n$0200 8xxxxxxx xxxxxxxx #行 #1\r\n$0200 8xxxxxxx xxxxxxxx #行 #2\r\n$0200 8xxxxxxx xxxxxxxx #行 #3\r\n$0200 8xxxxxxx xxxxxxxx #行 #4\r\n$0200 8xxxxxxx xxxxxxxx #行 #5\r\n$0200 8xxxxxxx xxxxxxxx #行 #6\r\n$0200 8xxxxxxx xxxxxxxx #行 #7\r\n$0200 8xxxxxxx xxxxxxxx #行 #8\r\n$0200 8xxxxxxx xxxxxxxx #行 #9\r\n$0200 8xxxxxxx xxxxxxxx #行 #10\r\n\r\n如果0x81000000的值小于0x00000005(8位)，则执行以下0x0A个相关代码行。\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;

                case VC_GEN_B2COD: // B2 Code
                    strPage = "B2-Code";
                    pnlVitaCheatMain.Enabled = false;
                    btnVitaCheatGenerate.Enabled = false;
                    txtVCInstructions.Text = "B2码\r\n这种代码类型基本上使所有地址都成为相对地址。例如，0x816652E0绝对地址成为相对地址0x000652E0，而基数(segX)0x816由VitaCheat自动获取。要查看segX信息，请浏览内存，然后右摇杆上推。\r\n\r\n当作弊地址在0x81000000-0x83000000范围时，查找指针地址非常有用，因为99%的情况下它可以用来在没有TempAR时创建指针地址，只需减去发现的作弊地址和Seg1数据。\r\n\r\n例子\r\n_V0 无限HP Talis\r\n$B200 00000001 00000000\r\n$0200 00017B3C 0000270F\r\n\r\n这是通过减去地址81317B3C-81300000，Seg1=17B3C找到的\r\n\r\n此外，这种类型的代码解决了不同版本的基址出现的情况。\r\n\r\n注：$B2码在固件3.60上不起作用。您必须在3.65上使用z05或z06（最好），否则VitaCheat在使用时会崩溃。\r\n\r\n例子\r\n例如：mai版 PCSH00181 伊苏塞尔塞塔树海与vitamin版本具有不同的偏移量。\r\nExamples\r\nFor example: the mai version of the PCSH00181 Ys: Memories of Celceta has a different offset from the vitamin version.\r\n\r\nPCSH00181 伊苏树海-1.00-MAI5 by dask\r\n\r\n_V0 Money MAX\r\n$A100 810C6872 0000BF00\r\nSeg0:81000000-811F9188\r\n\r\n# PCSH00181 伊苏树海-1.00-vitamin by dask\r\n\r\nV0 Money MAX\r\n$A100 810C68D2 0000BF00\r\nSeg0:81000060-811F91E8\r\n\r\n\r\n您可以使用B码解决此问题。\r\n\r\n# PCSH00181 伊苏树海-1.00 by dask\r\n\r\n_V0 Money MAX\r\n$B200 00000000 00000000\r\n$A100 000C6872 0000BF00\r\n\r\n以上是seg0的一个示例。seg1的应用可以自己尝试忍者龙剑传∑2 +。\r\n写入$B200，可用于欧洲版（PCSB00294）和香港版（PCSG00157）。\r\n\r\n更多信息，请访问：" + strWiki + strPage;
                    break;
            }
        }

        private void ComboVitaCheatPointerLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtVitaCheatAddress1Offset2.Enabled = false;
            txtVitaCheatAddress2Offset2.Enabled = false;
            txtVitaCheatAddress1Offset3.Enabled = false;
            txtVitaCheatAddress2Offset3.Enabled = false;
            txtVitaCheatAddress1Offset4.Enabled = false;
            txtVitaCheatAddress2Offset4.Enabled = false;
            txtVitaCheatAddress1Offset5.Enabled = false;
            txtVitaCheatAddress2Offset5.Enabled = false;
            if (comboVitaCheatPointerLevel.SelectedIndex >= 1)
            {
                txtVitaCheatAddress1Offset2.Enabled = true;
                txtVitaCheatAddress2Offset2.Enabled = true;
            }
            if (comboVitaCheatPointerLevel.SelectedIndex >= 2)
            {
                txtVitaCheatAddress1Offset3.Enabled = true;
                txtVitaCheatAddress2Offset3.Enabled = true;
            }
            if (comboVitaCheatPointerLevel.SelectedIndex >= 3)
            {
                txtVitaCheatAddress1Offset4.Enabled = true;
                txtVitaCheatAddress2Offset4.Enabled = true;
            }
            if (comboVitaCheatPointerLevel.SelectedIndex >= 4)
            {
                txtVitaCheatAddress1Offset5.Enabled = true;
                txtVitaCheatAddress2Offset5.Enabled = true;
            }
        }

        private void BtnVitaCheatGenerate_Click(object sender, EventArgs e)
        {
            CheckValidNumbers();

            //Declared variables for the code types
            string VCstr1 = "_V0 生成代码\r\n\r\n";
            uint VCAddr1 = Utils.ParseNum(txtVitaCheatAddress1.Text, NumberStyles.AllowHexSpecifier);
            uint VCAddr2 = Utils.ParseNum(txtVitaCheatAddress2.Text, NumberStyles.AllowHexSpecifier);
            uint VCAddGp = Utils.ParseNum(txtVitaCheatAddressGap.Text, NumberStyles.AllowHexSpecifier);
            uint VCValGp = Utils.ParseNum(txtVitaCheatValueGap.Text, NumberStyles.AllowHexSpecifier);
            uint VCComps = Utils.ParseNum(numericVitaCheatCompressions.Text);
            uint VCValue = Utils.ParseNum(txtVitaCheatValue.Text);
            uint VCBtntype = Utils.ParseNum(comboVitaCheatButtonType.SelectedValue.ToString());
            uint VCBtn = Utils.ParseNum(comboVitaCheatButton.SelectedValue.ToString());
            uint VCBtn2 = Utils.ParseNum(comboVitaCheatButton2.SelectedValue.ToString());
            //uint VCRelLin = Utils.ParseNum("1");
            //uint VCSeg = Utils.ParseNum("1");
            //uint VCNull = Utils.ParseNum("0");

            //
            //Get Seg0/Seg1 State And Apply to VCstr1
            //
            switch (comboVitaCheatB200.Text)
            {
                case VC_GEN_B2_SEG0:
                    VCstr1 += "$B200 00000000 00000000\r\n";
                    break;

                case VC_GEN_B2_SEG1:
                    VCstr1 += "$B200 00000001 00000000\r\n";
                    break;

                case VC_GEN_B2_NONE:
                    break;
            }

            //
            // Get Conditional State and apply to VCstr1
            //
            int RelatedLines = Utils.RelatedLines(comboVitaCheatPointerLevel.SelectedIndex, comboVitaCheatCodeType.Text, VCBtntype, 1);
            if (comboVitaCheatCondition.Text != "无")
            {

                uint VCCondAddr    = Utils.ParseNum(txtVitaCheatCondAddr.Text, NumberStyles.AllowHexSpecifier);
                uint VCCondVal     = Utils.ParseNum(txtVitaCheatCondValue.Text);
                uint VCOperators   = Utils.ParseNum(comboVitaCheatCondition.SelectedValue.ToString());
                string VCGenCNDTN1 = $"$D{VCOperators:X01}{RelatedLines:X02} {VCCondAddr:X08} {VCCondVal:X08}\r\n";
                VCstr1 += VCGenCNDTN1;
            }

            //
            //Get Bit Type from radio buttons
            //
            int bittype = rdbVitaCheatBitType8Bit.Checked ? 0 : rdbVitaCheatBitType16Bit.Checked ? 1 : 2;

            //
            //Get Button Pad State And Apply to VCstr1
            //
            RelatedLines = Utils.RelatedLines(comboVitaCheatPointerLevel.SelectedIndex, comboVitaCheatCodeType.Text, VCBtntype, 0);
            switch (VCBtntype)
            {
                case 9:
                    break;

                default:
                    var VCBtnMath = VCBtn + VCBtn2;
                    VCstr1 += $"$C2{RelatedLines:X02} {VCBtntype:X08} {VCBtnMath:X08}\r\n";
                    break;
            }

            //
            // Generate code Types
            //
            switch (comboVitaCheatCodeType.Text)
            {
                case VC_GEN_WRITE:
                    var VCGenWrite1 = $"$0{bittype}00 {VCAddr1:X08} {VCValue:X08}\r\n";
                    txtVitaCheatCode.Text = VCstr1 + VCGenWrite1;
                    break;

                case VC_GEN_MOV:
                    var VCGenMov1 = string.Format("$5{0}00 {1:X08} {2:X08}\r\n", bittype, VCAddr1, VCAddr2);
                    txtVitaCheatCode.Text = VCstr1 + VCGenMov1;
                    break;

                case VC_GEN_COMP:
                    var VCGenComp1 = $"$4{bittype}00 {VCAddr1:X08} {VCValue:X08}\r\n";
                    var VCGenComp2 = $"${VCComps:X04} {VCAddGp:X08} {VCValGp:X08}\r\n";
                    txtVitaCheatCode.Text = VCstr1 + VCGenComp1 + VCGenComp2;
                    break;

                case VC_GEN_PNTR:
                    var VCGenPtrstr2 = "";
                    var VCGenptroff1 = Utils.ParseNum(txtVitaCheatAddress1Offset1.Text, NumberStyles.AllowHexSpecifier);
                    var VCGenPtr1 = $"$3{bittype}0{comboVitaCheatPointerLevel.Text} {VCAddr1:X08} {VCGenptroff1:X08}\r\n";
                    var VCGenPtr3 = $"$3300 00000000 {VCValue:X08}";

                    foreach (Control x in groupVitaCheatAddress1Offset.Controls)
                    {
                        if (x is TextBox box && x.Enabled)
                        {
                            var VCGenptr2 = Utils.ParseNum(box.Text, NumberStyles.AllowHexSpecifier);
                            if (box.TabIndex != 0)
                            {
                                VCGenPtrstr2 = $"$3{bittype}00 00000000 {VCGenptr2:X08}\r\n{VCGenPtrstr2}";
                            }
                        }
                    }
                    txtVitaCheatCode.Text = VCstr1 + VCGenPtr1 + VCGenPtrstr2 + VCGenPtr3;
                    break;

                case VC_GEN_PTRMOV:
                    var VCGenPtrMovstr2 = "";
                    var VCGenptrmovoff1 = Utils.ParseNum(txtVitaCheatAddress1Offset1.Text, NumberStyles.AllowHexSpecifier);
                    var VCGenPtrMov1 = $"$8{bittype}0{comboVitaCheatPointerLevel.Text} {VCAddr1:X08} {VCGenptrmovoff1:X08}\r\n";
                    var VCGenPtrMov3 = string.Format("$8800 00000000 00000000\r\n");
                    foreach (Control x in groupVitaCheatAddress1Offset.Controls)
                    {
                        if (x is TextBox box && x.Enabled)
                        {
                            var VCGenptrmov2 = Utils.ParseNum(box.Text, NumberStyles.AllowHexSpecifier);
                            if (box.TabIndex != 0)
                            {
                                VCGenPtrMovstr2 = $"$8{bittype}00 00000000 {VCGenptrmov2:X08}\r\n{VCGenPtrMovstr2}";
                            }
                        }
                    }
                    var VCGenPtr2str2 = "";
                    var VCGenptrmov2off1 = Utils.ParseNum(txtVitaCheatAddress2Offset1.Text, NumberStyles.AllowHexSpecifier);
                    var VCGenPtrMov21 = $"$8{bittype + 4}0{comboVitaCheatPointerLevel.Text} {VCAddr2:X08} {VCGenptrmov2off1:X08}\r\n";
                    var VCGenPtrMov23 = string.Format("$8900 00000000 00000000");
                    foreach (Control x in groupVitaCheatAddress2Offset.Controls)
                    {
                        if (x is TextBox box && x.Enabled)
                        {
                            uint VCGenptrmov22 = Utils.ParseNum(box.Text, NumberStyles.AllowHexSpecifier);
                            if (box.TabIndex != 0)
                            {
                                VCGenPtr2str2 = $"$8{bittype + 4}00 00000000 {VCGenptrmov22:X08}\r\n{VCGenPtr2str2}";
                            }
                        }
                    }
                    txtVitaCheatCode.Text = VCstr1 + VCGenPtrMov1 + VCGenPtrMovstr2 + VCGenPtrMov3 + VCGenPtrMov21 + VCGenPtr2str2 + VCGenPtrMov23;
                    break;

                default:
                    break;

                case VC_GEN_PTRCOM:
                    var VCGenPtrComstr2 = "";
                    var VCGenptrcomoff1 = Utils.ParseNum(txtVitaCheatAddress1Offset1.Text, NumberStyles.AllowHexSpecifier);
                    var VCGenPtrCom1 = $"$7{bittype}0{comboVitaCheatPointerLevel.Text} {VCAddr1:X08} {VCGenptrcomoff1:X08}\r\n";
                    var VCGenPtrCom3 = $"$770{numericVitaCheatCompressionLevelOffset.Text} 00000000 {VCValue:X08}\r\n";
                    var VCGenPtrCom4 = $"${VCComps:X04} 0000{VCAddGp:X04} 0000{VCValGp:X04}";
                    foreach (Control x in groupVitaCheatAddress1Offset.Controls)
                    {
                        if (x is TextBox box && x.Enabled)
                        {
                            var VCGenptr2 = Utils.ParseNum(box.Text, NumberStyles.AllowHexSpecifier);
                            if (box.TabIndex != 0)
                            {
                                VCGenPtrComstr2 = $"$7{bittype}00 00000000 {VCGenptr2:X08}\r\n{VCGenPtrComstr2}";
                            }
                        }
                    }
                    txtVitaCheatCode.Text = VCstr1 + VCGenPtrCom1 + VCGenPtrComstr2 + VCGenPtrCom3 + VCGenPtrCom4;
                    break;

                case VC_GEN_ARMWRT:
                    var VCGenARMWRT1 = $"$A{bittype}00 {VCAddr1:X08} {VCAddr2:X08}\r\n";
                    txtVitaCheatCode.Text = VCstr1 + VCGenARMWRT1;
                    break;
            }
        }

        private void CheckValidNumbers()
        {
            //
            //Check for hex numbers and give error if bad
            //
            Utils.ParseNum(txtVitaCheatAddress1.Text, NumberStyles.AllowHexSpecifier, "无法分析目标地址，请确保地址是有效的十六进制数。");
            Utils.ParseNum(txtVitaCheatAddress2.Text, NumberStyles.AllowHexSpecifier, "无法分析来源地址（复制自），请确保地址是有效的十六进制数。");
            Utils.ParseNum(txtVitaCheatAddressGap.Text, NumberStyles.AllowHexSpecifier, "无法分析地址间隔，请确保值是有效的十六进制数。");
            Utils.ParseNum(txtVitaCheatValueGap.Text, NumberStyles.AllowHexSpecifier, "无法分析数值间隔，请确保值是有效的十六进制数。");
            Utils.ParseNum(numericVitaCheatCompressions.Text, NumberStyles.AllowHexSpecifier, "你不应该看到此错误！这是我的错。错误：他妈的是压缩码。");
            Utils.ParseGroupNums(groupVitaCheatAddress1Offset);
            Utils.ParseGroupNums(groupVitaCheatAddress2Offset);
        }
    }
}