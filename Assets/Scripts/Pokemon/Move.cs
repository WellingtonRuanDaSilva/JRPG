using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//classe para saber os status das habilidades/Moves
public class Move 
{
    public MoveBasePokemon Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBasePokemon pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}
