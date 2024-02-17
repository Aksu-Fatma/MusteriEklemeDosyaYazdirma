using System;

namespace AksuProje
{
    public interface INoteAction
    {
        void AddNote(Users user, string noteContent, string noteDate);
        void GetNoteList(Users user);
    }
}
