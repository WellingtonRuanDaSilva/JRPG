using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    //deixa a lista publica para acesso/utilizar
    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
    }

    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    //verificar na PT pokemon com vida
    public Pokemon GetHealthyPokemon()
    {
        //Funcao where do Link funciona como um loop que vai ver a lista de pokemons e retornar o pokemon que satisfaz a condicao
        //retornando o pokemon que tem HP maior que 0 e Firstordefaul pegara o primeiro e se nao encontrar retorna null
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon pokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(pokemon);
        }
        else
        {
            //FAZER AINDA: implementar o sistema de PC
        }
    }
}
