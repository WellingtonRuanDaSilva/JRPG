using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveEffects;

//cria menu para criar scrpit de Habilidades/move do Pokemon
[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
//Criando base de dados para as habilidades
public class MoveBasePokemon : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;


    public string Name => name;
    public string Description => description;
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;
    public MoveCategory Category => category;
    public MoveEffects Effects => effects;
    public MoveTarget Target => target;
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    public List<StatBoost> Boosts => boosts;
    public ConditionID Status => status;
}


[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}
    