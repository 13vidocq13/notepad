using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Text;
using DBTypes;

namespace DataAccess
{
    public class NotesManagerDA
    {
        public static string Compress(string data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    using (var streamWriter = new StreamWriter(gzipStream))
                    {
                        streamWriter.Write(data);
                    }

                    var byteArray = memoryStream.ToArray();

                    return Encoding.Default.GetString(byteArray);
                }
            }
        }

        public static string Decompress(string comressedData)
        {
            var byteArray = Encoding.Default.GetBytes(comressedData);

            using (var memoryStream = new MemoryStream(byteArray))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var streamReader = new StreamReader(gzipStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        public void AddData(string noteName, string noteText, string connectionString)
        {
            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                var compressedData = Compress(noteText);

                if(IsExistName(noteName, connection))
                    throw new InvalidOperationException("This name is already exist");

                var command = new OleDbCommand("INSERT into Notes" +
                                               "(NoteName, NoteText, DateCreated, DateModified)" +
                                               "values(@noteName, @noteText, @dateCreated, @dateModified)",
                    connection);

                if (connection.State == ConnectionState.Open)
                {
                    command.Parameters.Add("@noteName", OleDbType.Char, 20).Value = noteName;
                    command.Parameters.Add("@noteText", OleDbType.Char, Int32.MaxValue).Value = compressedData;
                    command.Parameters.Add("@dateCreated", OleDbType.Date).Value = DateTime.Now;
                    command.Parameters.Add("@dateModified", OleDbType.Date).Value = DateTime.Now;
                    
                    command.ExecuteNonQuery();
                }
                else
                {
                    throw new InvalidOperationException("Connection failed");
                }
            }
        }

        public void UpdateData(int noteId, string noteText, string connectionString)
        {
            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                noteText = Compress(noteText);

                var command = new OleDbCommand("UPDATE Notes" +
                                               " set NoteText=@noteText, DateModified=@dateModified" +
                                               " where (Id=@id)")
                {
                    Connection = connection
                };

                if (connection.State != ConnectionState.Open)
                    throw new InvalidOperationException("Connection failed");

                command.Parameters.Add("@noteText", OleDbType.Char).Value = noteText;
                command.Parameters.Add("@dateModified", OleDbType.Date).Value = DateTime.Now;
                command.Parameters.Add("@id", OleDbType.Integer).Value = noteId;

                command.ExecuteNonQuery();
            }
        }

        public IList<Notes> GetData(string connectionString)
        {
            IList<Notes> listNotes = new List<Notes>();

            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                var command = new OleDbCommand("SELECT * from Notes", connection);

                var dataReader = command.ExecuteReader();

                while (dataReader != null && dataReader.Read())
                {
                    var note = new Notes
                    {
                        Id = int.Parse(dataReader["Id"].ToString()),
                        NoteName = dataReader["NoteName"].ToString(),
                        NoteText = Decompress(dataReader["NoteText"].ToString()),
                        DateCreated = Convert.ToDateTime(dataReader["DateCreated"].ToString()),
                        DateModified = Convert.ToDateTime(dataReader["DateModified"].ToString())
                    };

                    listNotes.Add(note);
                }
            }

            return listNotes;
        }

        static bool IsExistName(string noteName, OleDbConnection connection)
        {
            var command = new OleDbCommand("SELECT Id from Notes " +
                                           "where NoteName=@noteName", connection);

            if (connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection failed");
           
            command.Parameters.Add("@noteName", OleDbType.Char).Value = noteName;

            var dataReader = command.ExecuteReader();

            while (dataReader != null && dataReader.Read())
            {
                int check;
                if (int.TryParse(dataReader["Id"].ToString(), out check))
                    return true;
            }

            return false;
        }
    }
}
