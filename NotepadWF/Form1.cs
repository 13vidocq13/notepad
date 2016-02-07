using System;
using System.Windows.Forms;
using BusinessLogic;
using DBTypes;

namespace NotepadWF
{
    public partial class Form1 : Form
    {
        private Notes _note;

        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SelectFileForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var currentNote = form.SelectedNote;
                    _note = currentNote;

                    textBox1.Text = currentNote.NoteText;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new Save_as())
            {
                if (form.ShowDialog() != DialogResult.OK)
                    return;

                var name = form.NameNote;

                try
                {
                    new NotesManagerBL().AddData(name, textBox1.Text, Properties.Settings.Default.ConnectionString);
                    MessageBox.Show("Note added successfully", "Success", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_note != null && _note.Id != 0)
            {
                try
                {
                    new NotesManagerBL().UpdateData(_note.Id, textBox1.Text, Properties.Settings.Default.ConnectionString);
                    MessageBox.Show("Note update successfully", "Success", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                }
            }
            else
            {
                using (var form = new Save_as())
                {
                    if (form.ShowDialog() != DialogResult.OK)
                        return;

                    var name = form.NameNote;

                    try
                    {
                        new NotesManagerBL().AddData(name, textBox1.Text, Properties.Settings.Default.ConnectionString);
                        MessageBox.Show("Note added successfully", "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }
                    catch (InvalidOperationException exception)
                    {
                        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    }
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            _note = null;
        }
    }
}
