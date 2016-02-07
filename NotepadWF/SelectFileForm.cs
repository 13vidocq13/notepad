using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using DBTypes;

namespace NotepadWF
{
    public partial class SelectFileForm : Form
    {
        private IList<Notes> _notesListStorage; 

        private Notes _selectedNote;

        public Notes SelectedNote
        {
            get { return _selectedNote; }
            set { _selectedNote = value; }
        }

        public SelectFileForm()
        {
            InitializeComponent();
            BindData();
        }

        void BindData()
        {
            Task.Factory.StartNew(LoadText).ContinueWith(task => UpdateUiAfterLoad(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LoadText()
        {
            _notesListStorage = new NotesManagerBL().GetData(Properties.Settings.Default.ConnectionString);
        }

        private void UpdateUiAfterLoad()
        {
            foreach (var listViewItem in _notesListStorage.Select(item => new ListViewItem(new[]
            {   
                item.NoteName,
                item.DateCreated.ToString(),
                item.DateModified.ToString()})))
            {
                listView1.Items.Add(listViewItem);
            }
        }

        Notes GetData(string name)
        {
            return _notesListStorage.FirstOrDefault(x => x.NoteName == name);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Nothing selected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var name = listView1.SelectedItems[0].Text;

            _selectedNote = GetData(name);

            DialogResult = DialogResult.OK;
        }
    }
}
