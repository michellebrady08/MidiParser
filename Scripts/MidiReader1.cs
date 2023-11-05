using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using UnityEngine;

public class MidiReader1 : MonoBehaviour
{
    public string midiFilePath;

    private void Start()
    {
        var midiFile = MidiFile.Read(midiFilePath);

        // Diccionario para realizar un seguimiento de las notas activas en cada canal MIDI
        Dictionary<byte, List<SevenBitNumber>> activeNotesByChannel = new Dictionary<byte, List<SevenBitNumber>>();

        foreach (TrackChunk trackChunk in midiFile.GetTrackChunks())
        {
            foreach (MidiEvent midiEvent in trackChunk.Events)
            {
                if (midiEvent is NoteOnEvent noteOnEvent)
                {
                    byte channel = noteOnEvent.Channel;
                    SevenBitNumber noteNumber = noteOnEvent.NoteNumber;

                    if (!activeNotesByChannel.ContainsKey(channel))
                    {
                        activeNotesByChannel[channel] = new List<SevenBitNumber>();
                    }

                    // Agregar la nota al canal activo
                    activeNotesByChannel[channel].Add(noteNumber);

                    // Detectar acordes
                    DetectChords(activeNotesByChannel[channel]);
                }
                else if (midiEvent is NoteOffEvent noteOffEvent)
                {
                    byte channel = noteOffEvent.Channel;
                    SevenBitNumber noteNumber = noteOffEvent.NoteNumber;

                    if (activeNotesByChannel.ContainsKey(channel))
                    {
                        // Eliminar la nota del canal activo
                        activeNotesByChannel[channel].Remove(noteNumber);

                        // Detectar acordes nuevamente después de que se libera la nota
                        DetectChords(activeNotesByChannel[channel]);
                    }
                }
            }
        }
    }

    private void DetectChords(List<SevenBitNumber> activeNotes)
    {
        if (activeNotes.Count > 1)
        {
            string chordNotes = string.Join(", ", activeNotes);
            Debug.Log($"Chord Detected: {chordNotes}");
        }
    }
}