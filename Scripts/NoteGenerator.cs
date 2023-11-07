using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NoteGenerator;

public class NoteGenerator : MonoBehaviour
{
    public float distanciaCamara = 5.0f; // Distancia de la cámara para eliminarla.

    public SpriteRenderer sprite;       // staffpara referencia al momento de escalar las notas
    public Color nuevoColor;
    public bool onSharp = true;         // Marca si quieres mostrar las notas en bemoles o sostenidos
    /*  PREFABS DE NOTAS */
    public GameObject sharpPrefab;      
    public GameObject flatPrefab;
    public GameObject line;
    public List<GameObject> notePrefabs;

    private Transform camaraTransform;  // Para mover el Satff
   
    public float altura = 0.005f;       // Separacion entre notas
    public float velocidad = 1.0f;      // Velocidad de desplazamiento

    private List<GameObject> noteList = new List<GameObject>(); // Lista para mantener control de los gameObjects
    /* DATOS DE CADA NOTA */
    public class Note
    {
        public int NoteNumber;
        public long Time;
        public long Duration;
        public bool Hand;
    }

    void Start()
    {
        camaraTransform = Camera.main.transform; // Referencia a la cámara principal.
    }

    void Update()
    {
        // Calcula la posición vertical de la cámara con un ajuste de distancia.
        float posicionCamaraX = camaraTransform.position.x - distanciaCamara;

        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            GameObject notex = noteList[i];
            if (notex.transform.position.x <= posicionCamaraX)
            {
                if(notex.GetComponent<NoteData>().noteNumber != 0)
                {
                    Debug.Log($"note: {notex.GetComponent<NoteData>().noteNumber}, duration: {notex.GetComponent<NoteData>().noteDuration}");
                }
                Destroy(notex); // Destruye el objeto cuando cruza la coordenada X de destrucción.
                
                noteList.RemoveAt(i); // Elimina el elemento de la lista en la posición 'i'.
            }
        }
    }


    public void generateNote(int n, long t, long d, bool h)
    {
        Note note = new Note
        {
            NoteNumber = n,
            Time = t, // Usamos el tiempo acumulado
            Duration = d,
            Hand = h,
        };
        createNote(note);    
    }

    void printNote(Note n, bool isSharp)
    {
        Transform spriteTransform = GameObject.Find("staff").transform;
        Vector3 spriteScale = spriteTransform.localScale;
        // Get the note duration
        GameObject fNote = getNoteType(n);

        // Get the note index
        int noteYpos = getNotePos(17, n.NoteNumber);

        // Manage container size
        float height = sprite.bounds.size.y-(fNote.GetComponent<SpriteRenderer>().bounds.size.y/5f); // conteiner Height
        Bounds spriteBounds = sprite.bounds; // container bottom limit
        float bottomLimit = spriteBounds.min.y;

        float noteHeight = height / 58; // divide the height between the 58 notes
        //Debug.Log($"Note {n.NoteNumber} pos {noteYpos}");
        // Determina la posición en función del número de nota
        Vector3 notePosition = new Vector3(n.Time * altura, (noteHeight * noteYpos)+ spriteBounds.min.y, -7.0f);
        // Crea una instancia de la nota en la posición calculada
        GameObject newNote = Instantiate(fNote, notePosition, Quaternion.identity, transform);

        printLines(n, newNote, noteYpos, bottomLimit, noteHeight);
        flipNote(n, newNote);

        if (isSharp)
        {
            GameObject symbol;
            if (onSharp)
            {
                symbol = sharpPrefab;
                n.NoteNumber++;
            }
            else
            {
                symbol = flatPrefab;
                n.NoteNumber--;
            }

            notePosition = new Vector3(notePosition.x - 0.7f, notePosition.y, -7.0f);
            GameObject newSharp = Instantiate(symbol, notePosition, Quaternion.identity, transform);
            noteList.Add(newSharp);
        }
        // Save note information on note
        newNote.GetComponent<NoteData>().noteNumber = n.NoteNumber;
        newNote.GetComponent<NoteData>().noteDuration = n.Duration;
        newNote.GetComponent<NoteData>().noteTime = n.Time;
        noteList.Add(newNote);
    }

    private void flipNote(Note n, GameObject note)
    {
        float size = note.GetComponent<SpriteRenderer>().bounds.size.y;
        Vector3 newPos = new Vector3(note.transform.position.x, note.transform.position.y + (size / 3.5f), -7.0f);
        if (n.NoteNumber > 70 || ((n.NoteNumber > 49 && n.NoteNumber<58) || (n.NoteNumber > 58 && n.Hand)))
        {
            note.transform.position = newPos;
            note.GetComponent<SpriteRenderer>().flipX = true;
            note.GetComponent<SpriteRenderer>().flipY = true;
        }
    }

    private void printLines(Note n, GameObject note, int pos, float bottomLimit, float noteHeight)
    {
        if (n.NoteNumber > 80) // si esta por encima del staff
        {
            if (pos % 2 != 0)
            {
                pos--;
            }
            //numero de lineas por debajo de la nota
            int lineNumber = (pos - 38) / 2;
            for (int i = lineNumber-1; i >= 0; i--)
            {
                Vector3 linePosition = new Vector3(note.transform.position.x, noteHeight * (pos - (i * 2)) + bottomLimit, -7.0f);
                GameObject newLine = Instantiate(line, linePosition, Quaternion.identity, transform);
                noteList.Add(newLine);
            }
        }
        else if (n.NoteNumber < 41) // si esta por debajo
        {
            if (pos % 2 != 0)
            {
                pos++;
            }
            int lineNumber = -((pos - 16) / 2);
            for (int i = 0; i <= lineNumber; i++)
            {
                Vector3 linePosition = new Vector3(note.transform.position.x, noteHeight * (pos + (i * 2)) + bottomLimit, -7.0f);
                GameObject newLine = Instantiate(line, linePosition, Quaternion.identity, transform);
                noteList.Add(newLine);
            }
        }
        else if (n.NoteNumber == 60) // si es C
        {
            Vector3 linePosition = new Vector3(note.transform.position.x, note.transform.position.y, -7.0f);
            GameObject newLine = Instantiate(line, linePosition, Quaternion.identity, transform);
            noteList.Add(newLine);
        }
    }

    private int getNotePos(int startNote, int noteNumber)
    {
        // Obtenemos la octava en la que se encuentra nuestra nota
        int Octave = ((noteNumber - startNote) / 12)+1;
        return ((noteNumber - 12) / 2 )+ Octave;
    }

    private GameObject getNoteType(Note n)
    {
        GameObject fNote;
        if (n.Duration > 50 && n.Duration < 100)
        {
            fNote = notePrefabs[0];
        }
        else if (n.Duration > 100 && n.Duration < 200)
        {
            fNote = notePrefabs[1];
        }
        else if (n.Duration > 200 && n.Duration < 300)
        {
            fNote = notePrefabs[2];
        }
        else if (n.Duration > 300 && n.Duration < 500)
        {
            fNote = notePrefabs[3];
        }
        else if (n.Duration > 500 && n.Duration < 1000)
        {
            fNote = notePrefabs[4];
        }
        else
        {
            fNote = notePrefabs[5];
        }
        return fNote;
    }

    void createNote(Note n)
    {
        int Note = n.NoteNumber % 12;
        bool isSharp = false;
        // Checamos si la nota es sostenido
        if(Note == 1 ||  Note == 3 || Note == 6 || Note == 8 || Note == 10) 
        {
            if(onSharp == true)
            {
                // Si lo es le restamos para que se quede en la posision correcta
                n.NoteNumber--;
                isSharp = true;
            }
            else
            {
                n.NoteNumber++;
                isSharp = true;
            }
            
        }
        printNote(n, isSharp);
    }

    void DestroyNotes()
    {
        if(noteList.Count > 0)
        {
            GameObject lastNote = noteList[0];
            noteList.RemoveAt(0);

            if(lastNote != null )
            {
                Destroy(lastNote);
            }
        }
    }
}

