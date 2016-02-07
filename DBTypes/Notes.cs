using System;

namespace DBTypes
{
    public class Notes
    {
        public int Id { get; set; }
        public string NoteName { get; set; }
        public string NoteText { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
