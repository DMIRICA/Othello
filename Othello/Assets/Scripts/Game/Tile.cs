using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Assets.Scripts.Singleton;
using Assets.Scripts.Networking;
using System.Text;
using Assets.Scripts.Networking.GamePackets;

public enum CellColor { Empty, Red, Black };

public class Tile : MonoBehaviour
{

    #region Unity Elements
    public Image selfTile;
    public Image image;
    public Color initColor;
    public Text ChangeText;
    private Animator animator;
    #endregion

    #region Variables
    public int row;
    public int column;
    private CellColor cellColor;
    #endregion

    #region Merthods
    public CellColor CellColor
    {
        get { return cellColor; }
        set
        {
            cellColor = value;
            setSprite(cellColor);
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        initColor = selfTile.color;
    }

    public void setSprite(CellColor color)
    {
        switch (color)
        {
            case CellColor.Red:
                image.sprite = Singleton.Instance.sprites[1];
                image.color = new Color(255, 255, 255);
                break;

            case CellColor.Black:
                image.sprite = Singleton.Instance.sprites[0];
                image.color = new Color(255, 255, 255);
                break;

            case CellColor.Empty:
                image.sprite = null;
                image.color = initColor;
                break;
        }

    }

    public void PlayAddAnimation()
    {
        animator.SetTrigger("AddChip");
    }

    public void PlayFlipAnimation()
    {
        animator.SetTrigger("FlipChip");
    }

    void Update()
    {
        
    }

    public bool AbleToClick()
    {
        return Singleton.Instance.IsYourTurn && selfTile.color == Color.white;
    }
    public void Click()
    {
        if (AbleToClick())
        {
            CellColor = Singleton.Instance.DiskColor;
            PlayFlipAnimation();
            GameBoard.RemoveDrawMoves();
            Singleton.Instance.LegalMoves.Clear();
            string ClickMessage = Convert.ToString(this.row - 1) + ':' + Convert.ToString(this.column - 1) + '|' + Singleton.Instance.DiskColor.ToString();
            GamePacket TurnPacket = new GamePacket(GameProtocol.TurnMovePacket(),Singleton.Instance.RoomID,ClickMessage);
            Singleton.Instance.Connection.SendPacket(TurnPacket.getData());
            
        }
    }
    #endregion

}