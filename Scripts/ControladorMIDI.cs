using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class ControladorMIDI : MonoBehaviour
{
    public GameObject fNote;
    public SpriteRenderer sprite;

    public bool onSharp = true;
    public GameObject sharpPrefab;
    public GameObject flatPrefab;
    public GameObject line;
    private List<GameObject> noteList = new List<GameObject>();
    private bool[] isKeyPressed = new bool[105];
    public float speed = 0.05f;

    public GameObject noteManager;
    private GameObject[] notesPressed = new GameObject[105]; // bars linked to the pressed key
    private List<GameObject> notesReleased = new List<GameObject>(); // bars linked to the released key
    private Transform camaraTransform;

    public class Note
    {
        public int NoteNumber;
        public double Time;
        public double Duration;
    }

    void Start()
    {
        camaraTransform = Camera.main.transform; // Referencia a la cámara principal.
        MidiJack.MidiMaster.noteOnDelegate += NoteOn;
        MidiJack.MidiMaster.noteOffDelegate += NoteOff;
    }

    void Update()
    {
        // currently pressed keys
        for (int i = 0; i < 88; i++)
        {
            if (isKeyPressed[i] && notesPressed[i] != null)
            {
                Vector2 scale = notesPressed[i].GetComponent<SpriteRenderer>().size;
                scale.x += speed;
                notesPressed[i].GetComponent<SpriteRenderer>().size = scale;
                //Vector3 pos = notesPressed[i].transform.position;
                //pos.x += speed;
                //notesPressed[i].transform.position = pos;
            }
        }

        // released keys
        for (int i = notesReleased.Count - 1; i >= 0; i--)
        {
            Vector3 pos = notesReleased[i].transform.position;

            // destroy bars when it reached upperPositionLimit
            if (pos.x < camaraTransform.position.x-5.5f)
            {
                Destroy(notesReleased[i]);
                notesReleased.RemoveAt(i);
            }
            
        }
    }

    void printNote(int n, bool isSharp)
    {
        Transform spriteTransform = GameObject.Find("staff").transform;
        Vector3 spriteScale = spriteTransform.localScale;

        // Get the note index
        int noteYpos = getNotePos(17, n);

        // Manage container size
        float height = sprite.bounds.size.y - 0.5f; // conteiner Height
        Bounds spriteBounds = sprite.bounds; // container bottom limit
        float bottomLimit = spriteBounds.min.y + spriteScale.x * 1.1f;
        float noteHeight = height / 58; // divide the height between the 58 notes

        // Determina la posición en función del número de nota
        Vector3 notePosition = new Vector3(0, (noteHeight * noteYpos) + bottomLimit, -7.0f);
        // Crea una instancia de la nota en la posición calculada
        GameObject newNote = Instantiate(fNote, notePosition, Quaternion.identity, transform);

        if (n > 80) // si esta por encima del staff
        {
            if (noteYpos % 2 != 0)
            {
                noteYpos--;
            }
            //numero de lineas por debajo de la nota
            int lineNumber = (noteYpos - 38) / 2;
            for (int i = lineNumber; i > 0; i--)
            {
                Vector3 linePosition = new Vector3(notePosition.x- 0.2f, (noteHeight * (noteYpos - (i * 2))) + bottomLimit, -7.0f);
                GameObject newLine = Instantiate(line, linePosition, Quaternion.identity, transform);
                noteList.Add(newLine);
            }
        }
        else if (n < 41) // si esta por debajo
        {
            if (noteYpos % 2 != 0)
            {
                noteYpos++;
            }
            int lineNumber = -((noteYpos - 16) / 2);
            for (int i = -1; i < lineNumber; i++)
            {
                Vector3 linePosition = new Vector3(notePosition.x - 0.2f, (noteHeight * (noteYpos + (i * 2))) + bottomLimit, -7.0f);
                GameObject newLine = Instantiate(line, linePosition, Quaternion.identity, transform);
                noteList.Add(newLine);
            }
        }
        else if (n == 60) // si es C
        {
            Vector3 linePosition = new Vector3(notePosition.x - 0.2f, (noteHeight * (noteYpos - 2)) + bottomLimit, -7.0f);
            GameObject newLine = Instantiate(line, linePosition, Quaternion.identity, transform);
            noteList.Add(newLine);
        }

        if (isSharp)
        {
            GameObject symbol;
            if (onSharp)
            {
                symbol = sharpPrefab;
                n++;
            }
            else
            {
                symbol = flatPrefab;
                n--;
            }

            notePosition = new Vector3(notePosition.x - 0.7f, notePosition.y - 0.7f, -7.0f);
            GameObject newSharp = Instantiate(symbol, notePosition, Quaternion.identity, transform);
            noteList.Add(newSharp);
        }
        // Save note information on note
        noteList.Add(newNote);
    }

    private int getNotePos(int startNote, int noteNumber)
    {
        // Obtenemos la octava en la que se encuentra nuestra nota
        int Octave = ((noteNumber - startNote) / 12) + 1;
        return ((noteNumber - 12) / 2) + Octave;
    }

    void createNote(int n)
    {
        int Note = n % 12;
        bool isSharp = false;
        // Checamos si la nota es sostenido
        if (Note == 1 || Note == 3 || Note == 6 || Note == 8 || Note == 10)
        {
            if (onSharp == true)
            {
                // Si lo es le restamos para que se quede en la posision correcta
                n++;
                isSharp = true;
            }
            else
            {
                n--;
                isSharp = true;
            }

        }
        printNote(n, isSharp);
    }

    void NoteOn(MidiJack.MidiChannel channel, int note, float velocity)
    {
        Note res = onNoteOn(note, velocity);
        if(res!= null)
        {
            Debug.Log($"Nota: {res.NoteNumber} time: {res.Time} dur: {res.Duration}");
        }
        
        //noteGenerator.UserInteracted(); // Llama a la función en el NoteGenerator
        //noteGenerator.wait = false;
        // Realiza la acción deseada cuando se presiona una nota MIDI específica
        // Por ejemplo, puedes mover un objeto o reproducir un sonido.
    }

    void NoteOff(MidiJack.MidiChannel channel, int note)
    {
        Debug.Log("Nota MIDI " + note + " liberada.");
        onNoteOff(note);
    }

    public Note onNoteOn(int noteNumber, float velocity)
    {
        Transform spriteTransform = GameObject.Find("staff").transform;
        Vector3 spriteScale = spriteTransform.localScale;
        // Get the note index
        int noteYpos = getNotePos(17, noteNumber);
        float size = fNote.GetComponent<SpriteRenderer>().bounds.size.y;
        // Manage container size
        float height = sprite.bounds.size.y - 0.5f; // conteiner Height
        Bounds spriteBounds = sprite.bounds; // container bottom limit
        float noteHeight = height / 58; // divide the height between the 58 notes

        // clearfy that the key is pressed
        isKeyPressed[noteNumber] = true;
        GameObject notePrefab = fNote;
        
        Vector3 notePosition = new Vector3(camaraTransform.position.x - 5f, (noteHeight * noteYpos)+(size/2) + spriteBounds.min.y, -7);
        notesPressed[noteNumber] = Instantiate(fNote, notePosition, Quaternion.identity, transform);
        Bounds bounds = notesPressed[noteNumber].GetComponent<SpriteRenderer>().bounds;
        float bottomLimit = bounds.min.x;
        float topLimit = bounds.max.x;
        if(velocity != 0)
        {
            Note note = new Note
            {
                NoteNumber = noteNumber,
                Time = (bottomLimit) / 0.005, // Usamos el tiempo acumulado
                Duration =  (topLimit - bottomLimit)/0.002,
            };
            return note;
        }

        return null;
        //Debug.Log($"note: {noteNumber} time: {time} velocity: {velocity}");
    }

    public void onNoteOff(int noteNumber)
    {
        notesReleased.Add(Clone(notesPressed[noteNumber]));
        Destroy(notesPressed[noteNumber]);

        isKeyPressed[noteNumber] = false;
    }

    GameObject Clone(GameObject obj)
    // reference: https://develop.hateblo.jp/entry/2018/06/30/142319
    {
        var clone = GameObject.Instantiate(obj) as GameObject;
        clone.transform.parent = obj.transform.parent;
        clone.transform.localPosition = obj.transform.localPosition;
        clone.transform.localScale = obj.transform.localScale;
        return clone;
    }
}



