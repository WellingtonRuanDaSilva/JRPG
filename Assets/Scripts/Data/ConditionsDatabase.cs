using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDatabase
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

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
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Parylized",
                StartMessage = "foi paralizado",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} foi paralizado e nao pode se mover");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Frozen",
                StartMessage = "foi congelado",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} nao esta mais congelado");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "caiu no Sono",
                OnStart = (Pokemon pokemon) =>
                {
                    //colocar o pokemon dormir por tunrnos
                    pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"ira ficar dormindo por {pokemon.StatusTime} turnos");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} acordou!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} esta dormindo");
                    return false;
                }
            }
        },
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "ficou confuso",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = Random.Range(1,4);
                    Debug.Log($"ira ficar confuso por {pokemon.VolatileStatusTime} turnos");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} nao esta mais confuso!");
                        return true;
                    }

                    pokemon.VolatileStatusTime--;

                    //50% de chance de fazer um move
                    if (Random.Range(1,3) == 1)
                        return true;

                    //se machuca pela confusao
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} esta confuso");
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"Se machucou devido a confusao");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
            return 1.5f;

        return 1f;
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}