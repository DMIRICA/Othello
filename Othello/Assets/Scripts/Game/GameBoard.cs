using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Networking;
using Assets.Scripts.Singleton;

public class GameBoard : MonoBehaviour
{



    public static Tile[,] gameBoard = new Tile[8, 8];
    Tile[] helperBoard;
    void initBoard()
    {
        helperBoard = GetComponentsInChildren<Tile>();
        foreach (Tile t in helperBoard)
        {
            gameBoard[t.row - 1, t.column - 1] = t;
        }        
    }
    public static void RemoveDrawMoves()
    {
        foreach (Tile t in gameBoard)
        {
            t.selfTile.color = t.initColor;
            t.ChangeText.text = "";
        }
    }

    

    // Use this for initialization
    void Start()
    {
        var ugly = UnityThreadHelper.Dispatcher;
        initBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        
    }

  
}
