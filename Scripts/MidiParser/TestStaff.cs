using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStaff : MonoBehaviour
{
    public NoteGenerator noteGenerator;
    public int startNote=28;
    public int lastNote = 105;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = startNote; i<lastNote; i++)
        {
            if(i%12 == 1 || i%12 == 3 || i%12 == 6 || i%12 == 8 || i%12 == 10)
            {
                i++;
                startNote++;
            }
            int n = i;
            long t = (i - startNote)*200;
            
            noteGenerator.generateNote(n, t, 240);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
