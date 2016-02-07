using System.Collections.Generic;
using DataAccess;
using DBTypes;

namespace BusinessLogic
{
    public class NotesManagerBL
    {
        public void AddData(string noteName, string noteText, string connectionString)
        {
            new NotesManagerDA().AddData(noteName, noteText, connectionString);
        }

        public void UpdateData(int noteId, string noteText, string connectionString)
        {
            new NotesManagerDA().UpdateData(noteId, noteText, connectionString);
        }

        public IList<Notes> GetData(string connectionString)
        {
            return new NotesManagerDA().GetData(connectionString);
        }
    }
}
