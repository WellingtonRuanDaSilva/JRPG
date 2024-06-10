using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] Color highlightedColor;

    int currentSelecion = 0;

    public void SetMoveDate(List<MoveBasePokemon> currentMoves, MoveBasePokemon newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }
    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelecion;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelecion;

        currentSelecion = Mathf.Clamp(currentSelecion, 0, PokemonBase.MaxNumOfMoves);

        UpdateMoveSelection(currentSelecion);

        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke(currentSelecion);
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0;i < PokemonBase.MaxNumOfMoves+1; i++) 
        {
            if (i == selection)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
    }
}
