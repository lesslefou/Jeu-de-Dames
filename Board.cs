using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int size = 10;
    public Dictionary<int,Tile> tiles = new Dictionary<int,Tile>();
    public GameObject tilePrefab;
    public GameObject piecePrefab;
    //public GameObject borderPrefab;

    // Start is called before the first frame update
    void Start()
    {
        int idTile = 1;
        int idPieceB = 1;
        int idPieceN = 20;

        for (int i= 1; i<= size; i++){

            for (int j=1; j<=size; j++){
                GameObject tileObject = (GameObject)Instantiate(tilePrefab);
                Tile tile = tileObject.AddComponent<Tile>();
                tileObject.transform.parent = transform;
                tileObject.name = "Tile" + idTile;
                tile.coordinates = new Vector2(i,j);
                tileObject.transform.position = new Vector3(j,0,i);
                if (i%2==0 && j%2==1 || i%2==1 && j%2==0){
                    tile.color = new Color(255,255,255); // blanc
                }
                else{
                    tile.color = new Color(111,55,55); //marron
                }
                
                MeshRenderer tileObjectMeshRenderer = tileObject.GetComponent<MeshRenderer>();
                Material tileObjectMaterial = new Material(Shader.Find("Standard"));
                tileObjectMaterial.color = tile.color;
                tileObjectMeshRenderer.material = tileObjectMaterial;
                
                idTile++;

                
                if ((i%2==0 && j%2==0 || i%2==1 && j%2==1) && idPieceB <=20){  
                    GameObject pieceObject = (GameObject)Instantiate(piecePrefab);
                    Piece piece = pieceObject.AddComponent<Piece>();
                    pieceObject.transform.parent = transform;
                    pieceObject.name = "PieceB" + idPieceB;
                    piece.coordinates = new Vector2(i,j);
                    pieceObject.transform.position = new Vector3(j,0.6f,i);
                    piece.color = new Color(255,255,255); //blanc

                    
                    MeshRenderer pieceObjectMeshRenderer = pieceObject.GetComponent<MeshRenderer>();
                    Material pieceObjectMaterial = new Material(Shader.Find("Standard"));
                    pieceObjectMaterial.color = piece.color;
                    pieceObjectMeshRenderer.material = pieceObjectMaterial;
                    idPieceB++;
                }
                
                if (i >= 7 && (i%2==0 && j%2==0 || i%2==1 && j%2==1)){
                    GameObject pieceObject = (GameObject)Instantiate(piecePrefab);
                    Piece piece = pieceObject.AddComponent<Piece>();
                    pieceObject.transform.parent = transform;
                    pieceObject.name = "PieceN" + idPieceN;
                    piece.coordinates = new Vector2(i,j);
                    pieceObject.transform.position = new Vector3(j,0.6f,i);
                    piece.color = new Color(0,0,0); //noir

                    
                    MeshRenderer pieceObjectMeshRenderer = pieceObject.GetComponent<MeshRenderer>();
                    Material pieceObjectMaterial = new Material(Shader.Find("Standard"));
                    pieceObjectMaterial.color = piece.color;
                    pieceObjectMeshRenderer.material = pieceObjectMaterial;
                    idPieceN--;
                }
                
                

            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
