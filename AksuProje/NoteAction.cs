using System;
using System.Collections.Generic;
using System.IO;

namespace AksuProje
{
    public class NoteAction : INoteAction
    {
        private List<Notes> NotesList;
        private string NoteFilePath;

        public NoteAction()
        {
            NotesList = new List<Notes>();
            NoteFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Data", "Notices.txt");
            CheckAndCreateFile(NoteFilePath);
        }

        public void AddNote(Users user, string noteContent, string noteDate)
        {
            string noteLine = $"{user.CustomerId}, {noteContent}, {noteDate}\n";
            File.AppendAllText(NoteFilePath, noteLine);
            Console.WriteLine("Notunuz başarıyla eklendi.");
        }

        public void GetNoteList(Users user)
        {
            List<Notes> response = new List<Notes>();

            if (!File.Exists(NoteFilePath))
            {
                Console.WriteLine("Kullanıcının henüz not girişi bulunmamaktadır.");
            }
            else
            {
                string[] noteLines = File.ReadAllLines(NoteFilePath);
                

                    if (noteLines.Length > 0)
                {
                    List<Notes> notes = new List<Notes>();
                    foreach (var line in noteLines)
                    {
                        var noteParts = line.Split(',');
                        var id = Convert.ToInt32(noteParts[0]);
                        var not = noteParts[1];
                        var date = noteParts[2];
                        Notes note = new Notes() { Text = not, CustomerId = id, CeraetedAt = Convert.ToDateTime(date) };
                        notes.Add(note);
                    }


                    response = notes.Where(x => x.CustomerId == user.CustomerId).ToList();  
                }
                else
                {
                    Console.WriteLine("Kullanıcının notu bulunmamaktadır.");
                }
            }

            foreach (var item in response)
            {
                Console.WriteLine(item.Text);
            }
        }

        private void CheckAndCreateFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.Create(filePath).Close();
            }
        }
    }
}
