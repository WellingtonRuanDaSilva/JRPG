using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//cria um menu na unity para poder criar os script
[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]

//Criando a base de dados dos pokemons
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;


    //duas formas de expor variaveis privadas para outro codigo
    // public string GetName()
    //{
    //    return name;
    //}
    //forma de propriedade (abaixo) = transforma em variavel em vez de chamar uma função.
    public string Name
    {
        get { return name; }
    }
    //tambem forma propriedade
    //  public string Name => name;
    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int SpDefense
    {
        get { return spDefense; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; } 
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBasePokemon moveBase;
    [SerializeField] int level;

    public MoveBasePokemon MoveBase => moveBase;
    public int Level => level;
}
public enum PokemonType
    {
        None,
        Normal,
        Fire,
        Water,
        Eletric,
        Grass,
        Ice,
        Fighting,
        Poison,
        Ground,
        Flying,
        Psychic,
        Bug,
        Rock,
        Ghost,
        Dragon,
    }