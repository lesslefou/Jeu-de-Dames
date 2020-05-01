using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Board board;
    public Camera camBlanche, camNoire;
    public Light lightBlanche, lightNoire;
    // Start is called before the first frame update
    void Start()
    {
        camNoire.enabled = false;
        camBlanche.enabled = true;
        lightBlanche.enabled = true;
        lightNoire.enabled = false;

        GameObject boardObject = new GameObject("Board");
        boardObject.transform.parent = transform;
        board = boardObject.AddComponent<Board>();
        board.tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
        board.piecePrefab = Resources.Load("Prefabs/Piece") as GameObject;
        //board.borderPrefab = (GameObject)Resources.Load("Prefabs/Border");
        board.nomPrefab = Resources.Load("Prefabs/JoueurAffichage") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        changeCamera();
    }
    private void changeCamera(){
        if (board.joueur) { // camera Noire Active
            camNoire.enabled = true;
            camBlanche.enabled = false;

            lightNoire.enabled = true;
            lightBlanche.enabled = false;
        }
        else {// camera Blanche Active
            camBlanche.enabled = true;
            camNoire.enabled = false;

            lightBlanche.enabled = true;
            lightNoire.enabled = false;
          
        }
    }
}
