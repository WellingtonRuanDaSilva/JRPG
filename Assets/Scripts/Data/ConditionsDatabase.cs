using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDatabase
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "foi envenenado",
                OnAfterTurn = (Pokemon pokemon) => 
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} sofre dano de envenenamento");
                }
            }
        },
                {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "foi queimado",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} sofre dano do queimado");
                }
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}