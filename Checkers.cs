using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkers : MonoBehaviour
{
    public Board board; 

    void Start()
    {
        GameObject boardObject = new GameObject("Board");
        boardObject.transform.parent = transform;
        board = boardObject.AddComponent<Board>();
        board.tilePrefab = (GameObject)Resources.Load("Prefabs/Tile");
        board.piecePrefab = (GameObject)Resources.Load("Prefabs/Piece");
        //board.borderPrefab = (GameObject)Resources.Load("Prefabs/Border");
    }

}
