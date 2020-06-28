using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reflect.WebServer.Data;

namespace Reflect.WebServer
{
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            txtServerPort.Text = GlobalSettings.ServerPort.ToString();
            txtRootFolder.Text = GlobalSettings.ServerRoot;
        }

        private void txtServerPort_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tbox)
                tbox.Text = new string(tbox.Text.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }

        private void btnRootSelect_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = txtRootFolder.Text,
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

            txtRootFolder.Text = dialog.FileName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServerPort.Text))
                GlobalSettings.ServerPort = int.Parse(txtServerPort.Text);

            if (!string.IsNullOrEmpty(txtRootFolder.Text))
                GlobalSettings.ServerRoot = txtRootFolder.Text;

            GlobalSettings.Save();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}