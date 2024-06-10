using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTrigger
{
    public void OnPlayerTrigged(PlayerController player)
    {
        GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
