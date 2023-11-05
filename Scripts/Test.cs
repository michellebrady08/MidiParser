using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        MidiMaster.noteOnDelegate += NoteOn;
        MidiMaster.noteOffDelegate += NoteOff;
    }

    void OnDisable()
    {
        MidiMaster.noteOnDelegate += NoteOn;
        MidiMaster.noteOffDelegate += NoteOff;
    }

    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        Debug.Log("NoteOn: "+ channel + "," + note+ "," + velocity);
    }

    void NoteOff(MidiChannel channel, int note)
    {
        Debug.Log("NoteOff: " + note);
    }
}

