using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightClickScript : MonoBehaviour {

    // Use this for initialization

    public GameObject panel;
    public Image self;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void mouseOver(BaseEventData data)
    {
        PointerEventData pointerEventData = data as PointerEventData;
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            if (pointerEventData.clickCount == 1)
            {
                Debug.Log("asd");
                GameObject myRoadInstance = Instantiate(panel, new Vector2(Input.mousePosition.x, Input.mousePosition.y-20), Quaternion.identity) as GameObject;
                myRoadInstance.transform.parent = self.transform;
                //Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                //S//criptUt.Instance.CreateContextMenu(contextMenuItems, new Vector2(pos.x, pos.y));
            }

        }
        //if(Input.GetMouseButtonDown(1))
        //{

        //Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        //ScriptUt.Instance.CreateContextMenu(contextMenuItems, new Vector2(pos.x, pos.y));
        //}

        //Debug.Log("onover");

    }
}
