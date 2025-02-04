using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{

    public int GravityDivideFactor = 6;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity /= GravityDivideFactor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
