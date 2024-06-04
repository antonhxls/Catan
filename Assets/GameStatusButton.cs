using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatusButton : MonoBehaviour
{

    public void ButtonPressed()
    {
        Game.Instance.PrintGameStatus();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
