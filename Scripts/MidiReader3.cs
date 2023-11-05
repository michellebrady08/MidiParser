using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class MidiReader3 : MonoBehaviour
{
    public string midiFilePath;
    private byte channel;
    private SevenBitNumber noteNumber;
    private long lastTime;

    private void Start()
    {
        var midiFile = MidiFile.Read(midiFilePath);

        // Diccionario para realizar un seguimiento de las notas activas en cada canal MIDI
        Dictionary<byte, Dictionary<SevenBitNumber, long>> activeNotesByChannel = new Dictionary<byte, Dictionary<SevenBitNumber, long>>();
        long currentTime = 0;

        foreach (TrackChunk trackChunk in midiFile.GetTrackChunks())
        {
            foreach (MidiEvent midiEvent in trackChunk.Events)
            {
                if (midiEvent is NoteOnEvent noteOnEvent)
                {
                    channel = noteOnEvent.Channel;
                    noteNumber = noteOnEvent.NoteNumber;

                    if (!activeNotesByChannel.ContainsKey(channel))
                    {
                        activeNotesByChannel[channel] = new Dictionary<SevenBitNumber, long>();
                    }

                    // Agregar la nota al canal activo
                    activeNotesByChannel[channel][noteNumber] = currentTime + midiEvent.DeltaTime;

                    // Detectar acordes
                }
                else if (midiEvent is NoteOffEvent noteOffEvent)
                {
                    channel = noteOffEvent.Channel;
                    noteNumber = noteOffEvent.NoteNumber;

                    if (activeNotesByChannel.ContainsKey(channel) && activeNotesByChannel[channel].ContainsKey(noteNumber))
                    {
                        var duration = currentTime;

                        // Detectar acordes nuevamente después de que se libera la nota
                        DetectChords(activeNotesByChannel[channel], duration);
                        // Eliminar la nota del canal activo
                        activeNotesByChannel[channel].Remove(noteNumber);
                    }
                }
                currentTime = midiEvent.DeltaTime;
            }
        }
    }

    private void DetectChords(Dictionary<SevenBitNumber, long> activeNotes, long duration)
    {
        if (activeNotes.Count > 1)
        {
            string chordNotes = string.Join(", ", activeNotes);
            Debug.Log($"Chord Detected: {chordNotes}, Duration {duration} ticks");
        }
    }
}
