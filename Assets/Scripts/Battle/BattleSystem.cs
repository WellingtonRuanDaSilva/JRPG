using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

//Variaveis para os estados de batalha
public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

//controla as informacoes que aparecem na tela de combate
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    //carrega informações de textos das batalhas
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);


        //texto quando aprece a batalha
        yield return dialogBox.TypeDialog($"Um {enemyUnit.Pokemon.Base.Name} selvagem apareceu");

        ActionSelection();
    }
    
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

    //ação de lutar ou correr do player
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Escolha o que fazer");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    //mostrar a tela de habilidades
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    //executa a ação de ataque utilizada
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerUnit.Pokemon.Moves[currentMove];

        yield return RunMove(playerUnit, enemyUnit, move);
        
        //se a luta nao mudar o status pelo RurMove, entao ira para o proximo passo
        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    //executa o ataque do inimigo
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Pokemon.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        //se a luta nao mudar o status pelo RurMove, entao ira para o proximo passo
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    //
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} usou {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();

        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} foi nocauteado");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPLayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    //mostrar texto se o ataque for critico/efetivo
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("Um acerto critico!");
        if (damageDetails.TypeEffectiviness > 1f)
            yield return dialogBox.TypeDialog("Super efetivo");
        else if (damageDetails.TypeEffectiviness < 1f)
            yield return dialogBox.TypeDialog("Nao e efetivo");
    }

    //selecionar opções durate a batalha
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    //escolher entre as 2 opçoes diponives
    //    if (Input.GetKeyDown(KeyCode.DownArrow))
    //{
    //    if(currentAction < 1)
    //        ++currentAction;
    //}
    //else if (Input.GetKeyDown(KeyCode.UpArrow)) 
    //{ 
    //    if(currentAction > 0) 
    //        --currentAction; 
    //}
    void HandActionSelection()
    {
        //escolher entre 4 opçoes disponiveis
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Lutar
            if (currentAction == 0)
            {
                MoveSelection();
            }
            //Bag
            else if (currentAction == 1)
            {

            }
            //Pokemon
            if (currentAction == 2)
            {
                OpenPartyScreen();
            }
            //Correr
            else if (currentAction == 3)
            {

            }
        }
    }

    void HandMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("Voce nao pode usar um pokemon nocauteado");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("Voce nao pode usar o mesmo pokemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }

    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if(playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Volte {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);

        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Vai {newPokemon.Base.Name}!");

        StartCoroutine(EnemyMove());

    }
}




////escolher entre as 4 disponives forma mais complexa
//if (Input.GetKeyDown(KeyCode.RightArrow))
//{
//    if (currentMove < playerUnit.Pokemon.Moves.Count -1)
//        ++currentMove;
//}
//else if (Input.GetKeyDown(KeyCode.LeftArrow))
//{
//    if (currentMove > 0)
//        --currentMove;
//}
//else if (Input.GetKeyDown(KeyCode.DownArrow))
//{
//    if (currentMove < playerUnit.Pokemon.Moves.Count -2)
//        currentMove +=2;
//}
//else if (Input.GetKeyDown(KeyCode.UpArrow))
//{
//    if (currentMove > 1)
//        currentMove -= 2;
//}
