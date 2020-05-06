using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    public int size = 10;
    public Piece[,] pieces = new Piece[100,100];
    public GameObject tilePrefab;
    public GameObject piecePrefab;
    public GameObject pieceObject;
    public GameObject tileObject;
    //public GameObject borderPrefab;
    public GameObject nomPrefab;
    public JoueurAffichage[] tableau = new JoueurAffichage[2];

    private RaycastHit hit;
    private Piece ptouch;
    private Vector2 mouseOver = new Vector2(0,0), mouseDestination = new Vector2(0,0), lastPieceMoved = new Vector2(0,0);
    private bool next = false;
    public bool joueur = false; // false = blanc ; true = noir
    private int ok=0;
    private bool sup = false;
    private bool haut = false, droite = false;
    private float X1=0, Y1=0;
    private int type = 0;   // =1 dplcmt simple   =2 tentative manger pion    =0 dplcmt non possible
    private int rejoue = 0;
    private int time = 0;
    private bool countDown = false;
    private int dplcmnt = 0;
    

    // Start is called before the first frame update
    void Start()
    {   
        int idTile = 1;
        for (int i= 1; i<= size; i++){
            for (int j=1; j<=size; j++){
                //Initialisation du tableau de jeu
                GenerateBoard(i,j,idTile);
                
                //Initilisation des pièces du jeu
                GeneratePiece(i,j);
                idTile++;
            }
        }
        GenerateNomAffichage();
    }


    void Update()
    {
        if (countDown) {
            Debug.Log("time = "  +time);
            time ++;
            if (sup) {
                if (checkIfMoveAgainPossible()){ //Le joueur peut encore manger un pion
                    checkSiJoueurRejoue(); 
                }
                else {
                    sup = false;
                    dplcmnt = 1;
                }
            }
            if (time == 100 ||time == 200 || time == 300) {
                Debug.Log(time);
            }
            
            if (time >= 300){
                consequenceFinAction();
            }
        }
        else {
            if (Input.GetMouseButtonDown(0)){
                if (joueur) Debug.Log("joueur N");
                else Debug.Log("joueur B");

                //Trace un rayon laser
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)){
                    //Recupere le nom de l'objet touché
                    string nameObject = hit.transform.name;
                    
                    //Si c'est une pièce alors on effectue les actions suivantes
                    if (nameObject == "PieceB" || nameObject == "PieceN"){
                        if (!next){
                            Debug.Log("Sélection pion");
                            verificationPiece(nameObject);

                            if (ok != 0) {
                                //récupère les coordonnées de la pièce
                                selectionPiece();
                                next = true;
                            }
                        }
                        else {
                            //Si sélection d'une autre pièce de la même couleur, on repart à la première étape
                            if ((joueur && nameObject == "PieceN") || (!joueur && nameObject == "PieceB")) {
                                Debug.Log("Vous avez sélectionné une autre pièce.");
                                //récupère les coordonnées de la pièce
                                selectionPiece();
                            }
                            else { 
                                Debug.Log("Sélection dplcmt");
                                mouseDestinationUpdate();

                                //On regarde si on peut déplacer la pièce
                                if (checkIfMovePossible(joueur,rejoue)){
                                    movePiece();
                                    
                                    countDown = true;
                                    

                                }
                            }
                        }                    
                        
                    }
                    else { // Pas de pièce sur cette case
                        if (next) {
                            mouseDestinationUpdate();

                            //On regarde si on peut déplacer la pièce
                            if (checkIfMovePossible(joueur,rejoue)){
                                movePiece();
                                countDown = true;
                            }
                        }
                        else {
                            Debug.Log("Veuillez sélectionner un jeton.");
                        }

                    }
                }  
            }
        }
        
    }


    private void checkSiJoueurRejoue(){
        Debug.Log ("move again possible");

        if (Input.GetMouseButtonDown(0)){ //Si le joueur essaye de rebouger son pion
            Debug.Log("nouvelle tentative de manger");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)){
                //Recupere le nom de l'objet touché
                string nameObject = hit.transform.name;
                Debug.Log ("essaye d'aller sur "+nameObject);
                if (nameObject != "PieceB" && nameObject != "PieceN"){
                    mouseDestinationUpdate();
                    if (checkIfMovePossible(joueur,rejoue)){
                        movePiece();
                    }
                }
            }
        }
    }

    private void consequenceFinAction() {
        Debug.Log ("consequenceFinAction");
        if (sup && dplcmnt == 0){ //Le joueur pouvait encore manger un pion mais ne l'a pas fait // son pion est supprimé
            Debug.Log("Vous n'avez pas continué le mouvement alors que vous le pouvez.\nVotre pion est mangé automatiquement par l'adversaire");
            
            //Supprimer le pion du joueur
            Debug.Log("LastPieceMoved = " + lastPieceMoved.x + lastPieceMoved.y);
            Piece destroy = pieces[(int)(lastPieceMoved.x -1),(int)(lastPieceMoved.y -1)];
            if (destroy != null) {
                pieces[(int)(lastPieceMoved.x -1),(int)(lastPieceMoved.y -1)] = null;
                Destroy(destroy.gameObject);

                if (!joueur) {
                    tableau[0].nbPiece--; //nbPionB--;
                }
                else {
                    tableau[1].nbPiece--; //nbPionN--;
                }
                Debug.Log("NbPionB : " + tableau[0].nbPiece + ", NbPionN : " + tableau[1].nbPiece);
            }
            

        } 
        else {
            Debug.Log("Bien joué, au tour de l'autre joueur");
        }

        
        //Réinitialisation des indicateurs
        sup = false;
        ok = 0;
        X1 = 0;
        Y1 = 0;
        haut = false;
        droite = false;
        type = 0;
        rejoue = 0;
        countDown = false;
        dplcmnt = 0;
        time = 0;
        changementJoueur();
        
    }




    private void verificationPiece(string namePiece){
        if (joueur && namePiece == "PieceN") {
            ok = 1;
        }     
        else if (!joueur && namePiece == "PieceB") {
            ok = 2;
        } 
        else {
            Debug.Log("Veuillez sélectionner un pion de votre couleur");
        }
    }

    private void selectionPiece(){
        mouseOverUpdate();
        if (ok == 1) {
            Debug.Log("click on pieceN at x="+ mouseOver.x + " and y=" + mouseOver.y);
        }
        else{
            Debug.Log("click on pieceB at x="+ mouseOver.x + " and y=" + mouseOver.y);
        }
    }

    private void changementJoueur(){
        if (joueur) {
            joueur = false;
            Debug.Log ("Changement de joueur N->B");
        }
        else {
            joueur = true;
            Debug.Log ("Changement de joueur B->N");
        }
        next = false;
    }

    private void movePiece(){
        Piece toMove = pieces[(int)(mouseOver.x -1),(int)(mouseOver.y -1)];
        toMove.transform.position = new Vector3(mouseDestination.x,0.6f,mouseDestination.y);
        toMove.coordinates = new Vector2(mouseDestination.y,mouseDestination.x);
        
        if (joueur) {
            toMove.color = new Color(0,0,0);
        }
        else {
            toMove.color = new Color(1,1,1);
        }
        
        if (sup == true) {
            Piece toDestroy = pieces[(int)(X1-1),(int)(Y1-1)];
            if (toDestroy!=null) {
                pieces[(int)(mouseDestination.x -1),(int)(mouseDestination.y -1)] = null;
                
                Debug.Log("Destroy " + toDestroy.coordinates);
                Destroy(toDestroy.gameObject);
            
                if (ok == 1) {
                    tableau[0].nbPiece--; //nbPionB--;
                }
                else {
                    tableau[1].nbPiece--; //nbPionN--;
                }
                Debug.Log("NbPionB : " + tableau[0].nbPiece + ", NbPionN : " + tableau[1].nbPiece);
            }
        }

        pieces[(int)(mouseDestination.x -1),(int)(mouseDestination.y -1)] = toMove;
        if ( pieces[(int)(mouseDestination.x -1),(int)(mouseDestination.y -1)] != null) {
            Debug.Log("Enregistrement nouvelle case réussite");
        }
        pieces[(int)(mouseOver.x -1),(int)(mouseOver.y -1)] = null;

        
    }

    private bool checkIfMoveAgainPossible(){
        int check = 0, cpt=0;
        float mDx = mouseDestination.x, mDy = mouseDestination.y;
        Debug.Log("Check Si dplcmt encore possible");
        //Check si case libre à deux cases de la pièce puis si pièce adverse à une case
        Debug.Log("XP=" + mDx + "YP=" + mDy);

        if (pieces[(int)(mDx+1),(int)(mDy+1)] == null){ //En haut à droite
            Debug.Log("haut/droite");
            check = checkSiPionAdverse((int)(mDx+1),(int)(mDy+1));
            if (check != 0) cpt++;
        }
        if (pieces[(int)(mDx+1),(int)(mDy-1)] == null){ //En bas à droite
            Debug.Log("bas/droite");
            check = checkSiPionAdverse((int)(mDx+1),(int)(mDy-1));
            if (check != 0) cpt++;
        }
        if (pieces[(int)(mDx-1),(int)(mDy+1)] == null){ //En haut à gauche
            Debug.Log("haut/gauche");
            check = checkSiPionAdverse((int)(mouseDestination.x-1),(int)(mDy+1));
            if (check != 0) cpt++;
        }
        if (pieces[(int)(mDx-1),(int)(mDy-1)] == null){ //En bas à gauche
            Debug.Log("bas/gauche");
            check = checkSiPionAdverse((int)(mDx-1),(int)(mDy-1));
            if (check != 0) cpt++;
        }
        
        Debug.Log("cpt = " + cpt);

        if (cpt != 0) {
            Debug.Log("dplcmt possible");
            mouseOver = mouseDestination;
            return true;
        }
        else {
            Debug.Log("dplcmt non possible");
            return false;
        }
    }
    private int checkSiPionAdverse(int XP,int YP) {
        Debug.Log("XPA = " + XP + " YPA = "+YP);
        Piece c1 = pieces[XP-1,YP-1];
        int check =0;
        if (c1 != null && c1.color == new Color (0,0,0) && !joueur) {
            check = 5;
            Debug.Log("joueur blanc mange joueur noir");
        }
        else if (c1 != null && c1.color == new Color (1,1,1) && joueur) {
            check = 5;
            Debug.Log("joueur noir mange joueur blanc");
        }
        return check;
    }
    
    private bool checkIfMovePossible(bool joueur, int rejoue){
       
        if (mouseDestination.x == mouseOver.x + 1  || mouseDestination.x == mouseOver.x - 1  || mouseDestination.x == mouseOver.x + 2  || mouseDestination.x == mouseOver.x - 2) {
            
            //Regarde quel type de dplcmt le joueur veut effectuer
            if ((mouseDestination.y == mouseOver.y +1 || mouseDestination.y == mouseOver.y -1) && (mouseDestination.x == mouseOver.x + 1  || mouseDestination.x == mouseOver.x - 1)){  // Dplcmt simple
                type = 1;
            }
            else if ((mouseDestination.y == mouseOver.y +2 || mouseDestination.y == mouseOver.y -2) && (mouseDestination.x == mouseOver.x + 2  || mouseDestination.x == mouseOver.x - 2)){ //Tentative manger pion
                type = 2; 
            }
            else {
                Debug.Log("Vous ne pouvez pas effectuer ce déplacement là");
            }
            
            
            //Check condition dplcmt
            if (type == 1 && rejoue == 0) {  //Test si dplcmt simple possible
                
                Piece p = pieces[(int)(mouseDestination.x - 1),(int)(mouseDestination.y - 1)];
                
                if (p != null) { //Une pièce se trouve déjà sur place => non 
                    Debug.Log("Un pièce se trouve déjà sur cet emplacement là.");
                    return false;
                }
                else {  // Dplcmt possible
                    Debug.Log("Pas de pion sur cette case. Check si dplcmt avant ou arrière");

                    //Détermine le sens du dplcmt
                    if (mouseDestination.y == mouseOver.y +1) {
                        haut = true;
                    }

                    if ((joueur && !haut) || (!joueur && haut)){
                        Debug.Log("Dplcmt autorisé");
                        haut = false;
                        return true;
                    }
                    else {
                        Debug.Log("Dplcmt en arrière non autorisé");
                        haut = false;
                        return false;
                    }

                }
            }
            else if (type == 2) {  //Test si tentative de manger possible

                Piece p = pieces[(int)(mouseDestination.x - 1),(int)(mouseDestination.y - 1)];

                if (p != null) { //Une pièce se trouve déjà sur place => non 
                    Debug.Log("Un pièce se trouve déjà sur cet emplacement là.");
                    return false;
                }
                else {  // Dplcmt possible  => check si le pion a manger est de couleur différente
                    Debug.Log("Dplcmt possible  => check si le pion a manger est de couleur différente");
                    
                    checkMvmtDplcmt();
                    Debug.Log("X1 = " + X1 + "Y1 = " + Y1);
                    Piece toKill = pieces[(int)(X1-1),(int)(Y1-1)];

                    if (toKill != null) { //Si une pièce se trouve bien sur la case d'avant
                        
                        if (toKill.color == new Color(0,0,0)) {  //La pièce toKill est noire
                            Debug.Log("Vous voulez manger une pièce noire.");
            
                            if (joueur) { //joueur noir  => dplcmnt non
                                Debug.Log("Vous ne pouvez pas manger vos propres pions");
                                return false;
                            }
                            else { //joueur blanche  
                                Debug.Log("Tentative de manger un pion adverse réussite.");
                                sup = true;
                                lastPieceMoved = mouseDestination;
                                return true;
                            }
                        }
                        else {  //La pièce toKill est blanche
                            Debug.Log("Vous voulez manger une pièce blanche.");
            
                            if (!joueur) { //joueur blanc  => dplcmnt non
                                Debug.Log("Vous ne pouvez pas manger vos propres pions");
                                return false;
                            }
                            else { //joueur noir  
                                Debug.Log("Tentative de manger un pion adverse réussite.");
                                sup = true;
                                lastPieceMoved = mouseDestination;
                                return true;
                            }
                        }

                    }
                    else {
                        Debug.Log("Vous ne pouvez pas vous déplacer de 2 cases");
                        return false;
                    }
                }
            }
            else { //Dplcmnt non autorisé
                return false;
            }
        }
        else {
            Debug.Log(" mouseDestination.x = " + mouseDestination.x + ", mouseOver.x = " + mouseOver.x + ", mouseDestination.y = " + mouseDestination.y + ", mouseOver.y = " + mouseOver.y);
            Debug.Log("Vous ne pouvez jouer que en diagonale et en avant");
            return false;
        }
    }

    private void checkMvmtDplcmt(){
        if (mouseDestination.y == mouseOver.y +2) {
            haut = true;
        }
        if (mouseDestination.x == mouseOver.x + 2) {
            droite = true;
        }

        if (droite && haut) {
            Debug.Log("Dplcmt haut/droite");
            Y1 = mouseOver.y +1;
            X1 = mouseOver.x +1;
        }
        else if (droite && !haut) {
            Debug.Log("Dplcmt bas/droite");
            Y1 = mouseOver.y -1;
            X1 = mouseOver.x +1;
        }
        else if (!droite && haut) {
            Debug.Log("Dplcmt haut/gauche");
            Y1 = mouseOver.y +1;
            X1 = mouseOver.x -1;
        }
        else if (!droite && !haut) {
            Debug.Log("Dplcmt bas/gauche");
            Y1 = mouseOver.y -1;
            X1 = mouseOver.x -1;
        }
    }

    private void mouseOverUpdate(){
        mouseOver.x = (int)(hit.point.x + 0.3f);
        mouseOver.y = (int)(hit.point.z + 0.5f);
        Debug.Log("xO="+ mouseOver.x + " and yO=" + mouseOver.y);
    }

    private void mouseDestinationUpdate(){
        mouseDestination.x = (int)(hit.point.x + 0.3f);
        mouseDestination.y = (int)(hit.point.z + 0.5f);
        Debug.Log("xD="+ mouseDestination.x + " and yD=" + mouseDestination.y);
    }

    private void GenerateBoard(int i, int j, int idTile){
        tileObject = Instantiate(tilePrefab) as GameObject;
        tileObject.transform.SetParent(transform);
        tileObject.name = "Tile" + idTile;
        tileObject.transform.position = new Vector3(j,0,i);
        
        Tile t = tileObject.AddComponent<Tile>();
        t.coordinates = new Vector2(i,j);
        
        if (i%2==0 && j%2==1 || i%2==1 && j%2==0){
            t.color = new Color(255,255,255); // blanc
        }
        else{
            t.color = new Color(255,0,255); //marron                                 A MODIFIER
        }
        MeshRenderer tileObjectMeshRenderer = tileObject.GetComponent<MeshRenderer>();
        Material tileObjectMaterial = new Material(Shader.Find("Standard"));
        tileObjectMaterial.color = t.color;
        tileObjectMeshRenderer.material = tileObjectMaterial;

    }

    private void GeneratePiece(int y, int x){
         if ((y<=4 || y>=7) && (y%2==0 && x%2==0 || y%2==1 && x%2==1)) {
            pieceObject = Instantiate(piecePrefab) as GameObject;
            pieceObject.transform.SetParent(transform);
            pieceObject.transform.position = new Vector3(x,0.6f,y);
            
            Piece p = pieceObject.AddComponent<Piece>();
            p = pieceObject.GetComponent<Piece>();
            p.coordinates = new Vector2(x,y);

            pieces[x-1,y-1] = p;                       

            if (y <= 4) {
                pieceObject.name = "PieceB";
                p.color = new Color(255,255,255); //blanc
            }
            else if (y >= 7 ){
                pieceObject.name = "PieceN";
                p.color = new Color(0,0,0); //noir
            }
            
            
            MeshRenderer pieceObjectMeshRenderer = pieceObject.GetComponent<MeshRenderer>();
            Material pieceObjectMaterial = new Material(Shader.Find("Standard"));
            pieceObjectMaterial.color = p.color;
            pieceObjectMeshRenderer.material = pieceObjectMaterial;
        }
    }

    private void GenerateNomAffichage() {
        for (int i=0; i<2; i++) {
            GameObject nomObject = Instantiate(nomPrefab) as GameObject;
            nomObject.transform.SetParent(transform);

            JoueurAffichage jA = nomObject.AddComponent<JoueurAffichage>();
            jA = nomObject.GetComponent<JoueurAffichage>();
            jA.nbPiece = 20;

            jA.nameJoueur = nomObject.GetComponent<TextMesh>();
            if (i == 0) {
                nomObject.name = "joueurBlanc";
                jA.nameJoueur.text = "Joueur noir" + "\nPièces : " + 20;
            }
            else  {
                jA.nameJoueur.text = "Joueur blanc" + "\n Pièces : " + 20;
                jA.nameJoueur.color = new Color (255,0,0);
                nomObject.name = "joueurNoir";
            }

            if (i==0) nomObject.transform.position = new Vector3(-5,0.8f,8.5f);
            else nomObject.transform.position = new Vector3(10.8f,0.8f,8.5f);
            
            tableau[i] = jA;  //tableau[0] = joueurBlanc  | [1] = joueurNoir                     
        }
        
    }

}
