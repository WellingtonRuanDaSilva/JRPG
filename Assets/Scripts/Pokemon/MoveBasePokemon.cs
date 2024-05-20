using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public string Name => name;
    public string Description => description;
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;

    public bool IsSpecial
    {
        get
        {
            if (type == PokemonType.Fire || type == PokemonType.Water || type == PokemonType.Ice || type == PokemonType.Eletric || type == PokemonType.Dragon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
