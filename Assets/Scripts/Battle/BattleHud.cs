using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

//atualiza informacao do pokemon na HUD
public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;

    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
        SetExp();

        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.psn, psnColor},
            { ConditionID.brn, brnColor},
            { ConditionID.slp, slpColor},
            { ConditionID.par, parColor},
            { ConditionID.frz, frzColor},
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _pokemon.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        float normalizedExp = (float)(_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    //atualiza a vida do pokemon
    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }

    }
}
