using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class MidiReader2 : MonoBehaviour
{
    public string midiFilePath;
    private SevenBitNumber noteNumber;

    private void Start()
    {
        var midiFile = MidiFile.Read(midiFilePath);

        // Diccionario para realizar un seguimiento de las notas activas en cada canal MIDI
        Dictionary<byte, Dictionary<SevenBitNumber, long>> activeNotesByChannel = new Dictionary<byte, Dictionary<SevenBitNumber, long>>();
        long currentTime = 0;

        foreach (var timedEvent in midiFile.GetTimedEvents())
        {
            var midiEvent = timedEvent.Event;
            var channelEvent = midiEvent as ChannelEvent;

            if (channelEvent != null)
            {
                byte channel = channelEvent.Channel;

                if (channelEvent is NoteOnEvent noteOnEvent)
                {
                    noteNumber = noteOnEvent.NoteNumber;
                }
                else if (channelEvent is NoteOffEvent noteOffEvent)
                {
                    noteNumber = noteOffEvent.NoteNumber;
                }

                if (!activeNotesByChannel.ContainsKey(channel))
                {
                    activeNotesByChannel[channel] = new Dictionary<SevenBitNumber, long>();
                }

                if (channelEvent.EventType == MidiEventType.NoteOn)
                {
                    //var noteonevent = (NoteOnEvent)channelEvent;
                    activeNotesByChannel[channel][noteNumber] = currentTime;
                }
                else if (channelEvent.EventType == MidiEventType.NoteOff)
                {
                    //var noteOffEvent = (NoteOffEvent)channelEvent;
                    if (activeNotesByChannel[channel].ContainsKey(noteNumber))
                    {
                        var startTime = activeNotesByChannel[channel][noteNumber];
                        var duration = currentTime - startTime;

                        // Detectar acordes
                        DetectChord(channel, noteNumber, duration);

                        activeNotesByChannel[channel].Remove(noteNumber);
                    }
                }
            }

            currentTime = timedEvent.Time;
        }
    }

    private void DetectChord(byte channel, SevenBitNumber noteNumber, long duration)
    {
        Debug.Log($"Chord Detected: Channel {channel}, Note {noteNumber}, Duration: {duration} ticks");
    }
}
