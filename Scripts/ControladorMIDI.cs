using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class ControladorMIDI : MonoBehaviour
{
    public NoteGenerator noteGenerator;

    void Start()
    {
        MidiJack.MidiMaster.noteOnDelegate += NoteOn;
        MidiJack.MidiMaster.noteOffDelegate += NoteOff;
    }

    void NoteOn(MidiJack.MidiChannel channel, int note, float velocity)
    {
        Debug.Log("Nota MIDI " + note + " presionada con velocidad: " + velocity);
        //noteGenerator.UserInteracted(); // Llama a la funci�n en el NoteGenerator
        //noteGenerator.wait = false;
        // Realiza la acci�n deseada cuando se presiona una nota MIDI espec�fica
        // Por ejemplo, puedes mover un objeto o reproducir un sonido.
    }

    void NoteOff(MidiJack.MidiChannel channel, int note)
    {
        Debug.Log("Nota MIDI " + note + " liberada.");
    }
}

