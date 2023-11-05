using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;

public class NoteController : MonoBehaviour
{
    public string midiFile;
    public NoteGenerator noteGenerator;

    private List<int> n = new List<int>();

    private void Start()
    {
        // Cargar el archivo MIDI
        var midiData = MidiFile.Read(midiFile);
        Dictionary<int, List<int>> notas = new Dictionary<int, List<int>>();
        // Inicializar la lista de notas

        foreach (TrackChunk chunk in midiData.GetTrackChunks())
        {

            foreach (var eventObj in chunk.Events)
            {
                if (eventObj is NoteOnEvent noteOnEvent)
                {
                    byte channel = noteOnEvent.Channel;
                    int noteNumber = noteOnEvent.NoteNumber;

                    if (!notas.ContainsKey(channel))
                    {
                        notas[channel] = new List<int>();
                    }

                    // Agregar la nota al canal activo
                    notas[channel].Add(noteNumber);
                    // Detectar acordes
                    //DetectChords(notas[channel]);
                }
                else if (eventObj is NoteOffEvent noteOffEvent)
                {
                    byte channel = noteOffEvent.Channel;
                    int noteNumber = noteOffEvent.NoteNumber;

                    if (notas.ContainsKey(channel))
                    {
                        // Eliminar la nota del canal activo
                        notas[channel].Remove(noteNumber);

                        // Detectar acordes nuevamente después de que se libera la nota
                        //DetectChords(notas[channel]);
                    }
                }
            }
        }
    }
    private void DetectChords(List<int> activeNotes)
    {
        if (activeNotes.Count > 1)
        {
            string chordNotes = string.Join(", ", activeNotes);
            //noteGenerator.generateNote(activeNotes);
            Debug.Log($"Chord Detected: {chordNotes}");
        }
        //noteGenerator.generateNote(activeNotes);
    }
}

