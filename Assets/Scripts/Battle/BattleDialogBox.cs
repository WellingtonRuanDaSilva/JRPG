using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Color highLighedColor;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List <TextMeshProUGUI> actionTexts;
    [SerializeField] List <TextMeshProUGUI> moveTexts;

    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI tyoeText;

    //mostra o texto da batalha
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    //Mostrar o texto da batlha animado uma letra por vez em sequencia
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    //funcao para ativar as caixa de dialogo
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector (bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    //mudar a cor da opçao de escolha
    public void UpdateActionSelection (int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
                actionTexts[i].color = highLighedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection (int selectedMove, Move move)
    {
        for (int i = 0;i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
                moveTexts[i].color = highLighedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        tyoeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;
    }

    //chama os nome das habildiades dos pokemons
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-"; 
             
        }
    }
}
