using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    async void Update()
    {
        if (Robber.IsInitialized == true && Game.Instance.Robber != null && Game.Instance.Robber.NeedsToBeMovedByPlayer == true)
        {
            // Überprüfe, ob die linke Maustaste gedrückt wurde
            if (Input.GetMouseButtonDown(0))
            {
                // Erzeuge einen Ray von der Kamera durch den Mauszeiger
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Überprüfe, ob der Ray ein GameObject trifft
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Überprüfe, ob das getroffene GameObject das ist, auf das reagiert werden soll
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        UnityEngine.Debug.Log($"Clicked {this.name}");

                        //Get Field from clicked tile
                        Field clickedField = Game.Instance.AllFields.Where(x => x.FieldNumber == Game.GetNumberFromString(this.name)).First();

                        if (clickedField != null)
                        {
                            Game.Instance.PlaceRobber(clickedField);

                            Game.Instance.Robber.NeedsToBeMovedByPlayer = false;

                            await Game.Instance.CurrentPlayer.MakeMove();
                        }
                    }
                }
            }
        }
    }
}
