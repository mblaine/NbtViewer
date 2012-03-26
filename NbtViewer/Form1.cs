using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Minecraft;

namespace NbtViewer
{
    public partial class Form1 : Form
    {
        private String lastPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\saves";
        private Find findForm = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetTabWidth(txtOutput, 1);            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = lastPath;
            dialog.Filter = "Minecraft (*.dat, mcr, mca)|*.dat;*.mcr;*.mca";
            dialog.RestoreDirectory = false;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            lastPath = dialog.FileName;
            txtOutput.Text = "";
            if (Path.GetExtension(lastPath) == ".dat")
            {
                MemoryStream data = new MemoryStream();
                using (GZipStream decompress = new GZipStream(dialog.OpenFile(), CompressionMode.Decompress))
                {
                    decompress.CopyTo(data);
                    decompress.Close();
                }
                data.Seek(0, SeekOrigin.Begin);
                TAG_Compound rootContainer = new TAG_Compound(data);
                txtOutput.Text += rootContainer.ToString();
            }
            else
            {
                txtOutput.Text = new RegionFile(lastPath).ToString();
            }

            int indent = 0;
            StringBuilder sb = new StringBuilder();
            foreach (String line in txtOutput.Lines)
            {
                if (line.Length > 0 && line[0] == '{')
                {
                    sb.Append(new String('\t', indent)).AppendLine(line);
                    indent++;
                }
                else if (line.Length > 0 && line[0] == '}')
                {
                    indent--;
                    sb.Append(new String('\t', indent)).AppendLine(line);
                }
                else
                    sb.Append(new String('\t', indent)).AppendLine(line);
            }
            txtOutput.Text = sb.ToString();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFindForm();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Find.LastSearch == null || Find.LastSearch.Length == 0)
                OpenFindForm();
            else
                Find.FindNext(Find.LastSearch, true, this, this);
        }


        private void findPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Find.LastSearch == null || Find.LastSearch.Length == 0)
                OpenFindForm();
            else
                Find.FindNext(Find.LastSearch, false, this, this);
        }

        private void OpenFindForm()
        {
            if (findForm == null || findForm.IsDisposed)
                findForm = new Find();
            if (findForm.Visible)
                findForm.Focus();
            else
                findForm.Show(this);
        }

        internal TextBox TextBox
        {
            get
            {
                return this.txtOutput;
            }
        }

        //http://schotime.net/blog/index.php/2008/03/12/select-all-ctrla-for-textbox/
        private void txtOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
            {
                txtOutput.SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else
                base.OnKeyDown(e);
        }

        //http://stackoverflow.com/questions/1298406/how-to-set-the-tab-width-in-a-windows-forms-textbox-control
        private const int EM_SETTABSTOPS = 0x00CB;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        public static void SetTabWidth(TextBox textbox, int tabWidth)
        {
            System.Drawing.Graphics graphics = textbox.CreateGraphics();
            var characterWidth = (int)graphics.MeasureString("M", textbox.Font).Width;
            SendMessage(textbox.Handle, EM_SETTABSTOPS, 1,
                        new int[] { tabWidth * characterWidth });
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog(this);
        }
    }
}
