using System;
using System.Windows.Forms;

namespace NbtViewer
{
    public partial class Find : Form
    {
        private static String lastSearch = "";

        public Find()
        {
            InitializeComponent();
        }

        private void Find_Load(object sender, EventArgs e)
        {
            txtFind.Text = lastSearch;
        }

        private void Find_FormClosing(object sender, FormClosingEventArgs e)
        {
            lastSearch = txtFind.Text;
        }

        private void Find_Activated(object sender, EventArgs e)
        {
            txtFind.Focus();
            txtFind.SelectAll();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnFindNext_Click(this, null);
                btnFindNext.Focus();
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            if (Search(txtFind.Text, ((Form1)Owner).TextBox, radDown.Checked))
                ((Form1)Owner).TextBox.ScrollToCaret();
            else
                MessageBox.Show(this, String.Format("Unable to find \"{0}\".", txtFind.Text), "Find", MessageBoxButtons.OK);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool Search(String search, TextBox target, bool forward)
        {
            if (forward)
            {
                int start = target.SelectionStart + target.SelectionLength;
                int i = target.Text.IndexOf(search, start, StringComparison.InvariantCultureIgnoreCase);
                if (i >= 0)
                {
                    target.Select(i, search.Length);
                    return true;
                }
                else
                {
                    i = target.Text.IndexOf(search, 0, StringComparison.InvariantCultureIgnoreCase);
                    if (i >= 0)
                    {
                        target.Select(i, search.Length);
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                int start = target.SelectionStart;
                int i = target.Text.LastIndexOf(search, start, start + 1, StringComparison.InvariantCultureIgnoreCase);
                if (i >= 0)
                {
                    target.Select(i, search.Length);
                    return true;
                }
                else
                {
                    i = target.Text.LastIndexOf(search, target.Text.Length - 1, target.Text.Length - start, StringComparison.InvariantCultureIgnoreCase);
                    if (i >= 0)
                    {
                        target.Select(i, search.Length);
                        return true;
                    }
                    else
                        return false;
                }
            }

        }
    }
}
