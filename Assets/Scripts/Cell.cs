﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cell : MonoBehaviour
{
    public Block containedBlock;
    public Vector2Int coOrdXY;
    private BoxCollider2D bc;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    void OnMouseDown()
    {
        Summoner.magic.SelectCell(this);
    }

    public void Update(){
        bc.enabled = (containedBlock==null);
    }

    public Cell CellBelow(){
        if(coOrdXY.y > 0){
            return GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1];
        }else{
            return null;
        }
    }


    
}
