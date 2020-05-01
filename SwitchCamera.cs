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
        changePositionNomAffichage();
        ChangeNomAffichage();
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

     private void changePositionNomAffichage(){
        if (board.joueur){ //vu par joueur noir
            JoueurAffichage jB = board.tableau[0];
            jB.transform.position = new Vector3(16,0.8f,2.5f);
            jB.transform.eulerAngles = new Vector3(60,-170,7);
            JoueurAffichage jN = board.tableau[1];
            jN.transform.position = new Vector3(0.5f,0.5f,2.2f);
            jN.transform.eulerAngles = new Vector3(60,-170,7);

        }
        else { //vu par joueur blanc
            JoueurAffichage jB = board.tableau[0];
            jB.transform.position = new Vector3(-5,0.8f,8.5f);
            jB.transform.eulerAngles = new Vector3(60,0,0);
            JoueurAffichage jN = board.tableau[1];
            jN.transform.position = new Vector3(10.8f,0.8f,8.3f);
            jN.transform.eulerAngles = new Vector3(60,0,0);
        }
    }

    private void ChangeNomAffichage(){
        if (board.joueur) {
            board.tableau[1].nameJoueur.color = new Color (255,0,0);
            board.tableau[1].nameJoueur.text = "Joueur noir" + "\n Pièces : " + board.tableau[1].nbPiece;
            board.tableau[0].nameJoueur.color = new Color (255,255,255);
            board.tableau[0].nameJoueur.text = "Joueur blanc" + "\n Pièces : " + board.tableau[0].nbPiece;
        }
        else  {
            board.tableau[0].nameJoueur.color = new Color (255,0,0);
            board.tableau[0].nameJoueur.text = "Joueur blanc" + "\n Pièces : " + board.tableau[0].nbPiece;
            board.tableau[1].nameJoueur.color = new Color (255,255,255);
            board.tableau[1].nameJoueur.text = "Joueur noir" + "\n Pièces : " + board.tableau[1].nbPiece;
        }
    }
}
