using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//atualiza a barra de vida no HUD
public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

}
