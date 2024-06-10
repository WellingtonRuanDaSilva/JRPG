using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] string name;

    const float offsetY = 0.3f;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private Vector2 input;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        //verifica se o player nao esta movendo para verificar o se ha input para mover  o player
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            

            //remove movimento na diagonal
            if (input.x != 0) input.y = 0;

            //se o movimento não é 0 vai mover para o destino do input
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact()
    {
        var facingDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDirection;

        // colocar uma linha para ver a funcionalidade
        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTrigger>();
            if (triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTrigged(this);
                break;
            }
        }
    }
    //codigo refatorado
    ////verificar se esta na grama para encontro de batalhas
    //private void CheckForEncounters()
    //{
    //    if ((Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.GrassLayer) != null))
    //    {
    //        if (UnityEngine.Random.Range(1,101) <= 10)
    //        {
    //            character.Animator.IsMoving = false;
    //            OnEncountered();
    //        }
    //    }
    //}
    //private void CheckIfInTrainerView()
    //{

    //    var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.FovLayer);
    //    if (collider != null)
    //    {
    //        character.Animator.IsMoving = false;
    //        OnEnterTrainersView?.Invoke(collider);
    //    }
    //}

    public string Name
    {
        get => name;
    }
    public Sprite Sprite
    {
        get => sprite;
    }
}

////Codigo refratorado para Character
////move o jogador dentro de um peiordo de tempo da posicao do player ate a targetPos
//IEnumerator Move(Vector3 targetPos)
//{
//    isMoving = true;

//    while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
//    {
//        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
//        yield return null;
//    }
//    transform.position = targetPos;

//    isMoving = false;

//    CheckForEncounters();
//}

////verifica se o jogador esta indo para uma posição que é possivel andar
//private bool IsWalkable(Vector3 targetPos)
//{
//    if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
//    {
//        return false;
//    }

//    return true;
//}