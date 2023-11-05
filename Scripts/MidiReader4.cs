using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;

public class MidiReader4 : MonoBehaviour
{
    public string midiFile; // Arrastra tu archivo MIDI aquí desde el inspector
    public float noteSpeed = 1.0f; // Ajusta la velocidad de visualización

    private float currentTime = 0;
    private List<Note> notes;

    private void Start()
    {
        // Cargar el archivo MIDI
        var midiData = MidiFile.Read(midiFile);

        // Inicializar la lista de notas
        notes = new List<Note>();

        foreach (TrackChunk chunk in midiData.GetTrackChunks())
        {
            long accumulatedTime = 0; // Lleva un registro del tiempo acumulado

            foreach (var eventObj in chunk.Events)
            {
                if (eventObj is NoteOnEvent noteOnEvent)
                {
                    Note note = new Note
                    {
                        NoteNumber = noteOnEvent.NoteNumber,
                        Time = accumulatedTime, // Usamos el tiempo acumulado
                        Duration = 0
                    };

                    notes.Add(note);
                }
                else if (eventObj is NoteOffEvent noteOffEvent)
                {
                    // Buscamos la nota correspondiente en la lista y calculamos la duración
                    Note note = null;

                    // Buscamos la nota correspondiente en la lista
                    for (int i = notes.Count - 1; i >= 0; i--)
                    {
                        if (notes[i].NoteNumber == noteOffEvent.NoteNumber)
                        {
                            note = notes[i];
                            break;
                        }
                    }

                    if (note != null)
                    {
                        note.Duration = noteOffEvent.DeltaTime;
                    }
                }

                accumulatedTime += eventObj.DeltaTime; // Actualizamos el tiempo acumulado
            }
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime * noteSpeed;

        // Recorre la lista de notas y visualiza las notas en el tiempo adecuado
        for (int i = notes.Count - 1; i >= 0; i--)
        {
            var note = notes[i];
            if (note.Time <= currentTime)
            {
                Debug.Log("Nota: " + note.NoteNumber + " Duración: " + note.Duration);
                notes.RemoveAt(i);
            }
        }
    }

    private class Note
    {
        public SevenBitNumber NoteNumber;
        public long Time;
        public long Duration;
    }
}

