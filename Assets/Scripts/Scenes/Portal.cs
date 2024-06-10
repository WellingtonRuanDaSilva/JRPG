using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPlayerTrigger
{
    public void OnPlayerTrigged(PlayerController player)
    {
        Debug.Log("Jogador entrou no portal");
    }
}
