using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;

public class NoteReader : MonoBehaviour
{
    public string midiFile; // Arrastra tu archivo MIDI aquí desde el inspector
    public float noteSpeed = 1.0f; // Ajusta la velocidad de visualización
    public NoteGenerator noteGenerator;

    private void Start()
    {
        // Cargar el archivo MIDI
        var midiData = MidiFile.Read(midiFile);
        // Inicializar diccionario de notas
        Dictionary<int, List<Note>> notas = new Dictionary<int, List<Note>>();

        //Recorremos el archivo midi
        foreach (TrackChunk chunk in midiData.GetTrackChunks())
        {
            long accumulatedTime = 0; // Lleva un registro del tiempo acumulado
            foreach (var eventObj in chunk.Events)
            {
                if (eventObj is NoteOnEvent noteOnEvent)
                {
                    byte channel = noteOnEvent.Channel;
                    if (!notas.ContainsKey(channel)) // Si es acorde se agrega al canal
                    {
                        notas[channel] = new List<Note>();
                    }
                    Note note = new Note
                    {
                        NoteNumber = noteOnEvent.NoteNumber,
                        Time = accumulatedTime, // Usamos el tiempo acumulado
                        Duration = 0
                    };
                    notas[channel].Add(note);

                }
                else if (eventObj is NoteOffEvent noteOffEvent)
                {
                    Note note = null;
                    byte channel = noteOffEvent.Channel;
                    // Igualamos la duración y tiempode los acordes 
                    if (notas.ContainsKey(channel))
                    {
                        if (notas[channel].Count > 1)
                        {
                            long mayor = -1;
                            long tiempo = -1;
                            for (int i = 0; i < notas[channel].Count; i++)
                            {
                                notas[channel][i].acorde = true;
                                notas[channel][i].Duration = noteOffEvent.DeltaTime;
                                if (notas[channel][i].Duration > mayor)
                                {
                                    tiempo = notas[channel][i].Time;
                                    mayor = notas[channel][i].Duration;
                                }
                            }
                            for (int i = 0; i < notas[channel].Count; i++)
                            {
                                notas[channel][i].Time = tiempo;
                                notas[channel][i].Duration = mayor;
                            }
                        }


                        // en caso de no ser acorde, calculamos su diración individual
                        for (int i = 0; i < notas[channel].Count; i++)
                        {
                            if (notas[channel][i].acorde == false)
                            {
                                notas[channel][i].Duration = noteOffEvent.DeltaTime;
                            }
                        }
                    
                        for (int i = notas[channel].Count - 1; i >= 0; i--)
                        {
                            if (notas[channel][i].NoteNumber == noteOffEvent.NoteNumber)
                            {
                                note = notas[channel][i];
                                notas[channel].Remove(note);
                                break;
                            }
                        }
                    }
                    
                    if (note != null)
                    {
                        //Debug.Log("Nota: " + note.NoteNumber + " Duración: " + note.Duration + "Time: " +note.Time);
                        noteGenerator.generateNote(note.NoteNumber, note.Time, note.Duration);
                        notas[channel].Remove(note);
                    }

                }
                accumulatedTime += eventObj.DeltaTime; // Actualizamos el tiempo acumulado
            }
        }
    }

    private class Note
    {
        public int NoteNumber;
        public long Time;
        public long Duration;
        public bool acorde = false;
    }
}