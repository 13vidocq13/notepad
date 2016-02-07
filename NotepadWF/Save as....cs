using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using DBTypes;

namespace NotepadWF
{
    public partial class Save_as : Form
    {
        private string _noteName;
        private IList<Notes> _notesList; 

        public string NameNote
        {
            get { return _noteName; }
            set { _noteName = value; }
        }

        public Save_as()
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
            _notesList = new NotesManagerBL().GetData(Properties.Settings.Default.ConnectionString);
        }

        private void UpdateUiAfterLoad()
        {
            foreach (var item in _notesList)
            {
                listView1.Items.Add(new ListViewItem(item.NoteName));
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _noteName = textBox1.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
