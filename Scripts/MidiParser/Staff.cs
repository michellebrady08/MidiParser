using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    public GameObject middleNote, lineNote;
    public GameObject staff;

    public class Note
    {
        public int NoteNumber;
        public long Time;
        public long Duration;
    }

    // Start is called before the first frame update
    void Start()
    {
        Note n = new Note
        {
            NoteNumber = 62,
            Time = 0, // Usamos el tiempo acumulado
            Duration = 10,
        };
        printNote(middleNote, n);
        n.NoteNumber = 64;
        n.Time = 240;
        printNote(middleNote, n);
    }

    void printNote(GameObject noteType, Note n)
    {
        float height = staff.GetComponent<RectTransform>().rect.height;
        float width = staff.GetComponent<RectTransform>().rect.width;
        // Para obtener la altura de las notas solo dividimos la altura del contenedor entre el número de notas que deben de caber
        float noteHeight = height / 58;
        float noteWidth = width / 25;

        //GameObject note = instantiateNote(noteType, n.NoteNumber, Quaternion.identity);
        GameObject note = Instantiate(noteType);
        note.transform.SetParent(staff.transform, false);
        //newNote.GetComponent<PianoTile>().midiNote = startNote + actualNoteIndex;

        note.GetComponent<RectTransform>().sizeDelta = new Vector2(noteWidth, noteHeight);
        note.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(n.Time, n.NoteNumber * 1 , 0);
    }
}
