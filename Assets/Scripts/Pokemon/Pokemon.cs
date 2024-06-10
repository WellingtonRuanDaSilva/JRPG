using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[System.Serializable]
//classe de pokemons para calculo de status por level e movimentos
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

    public PokemonBase Base
    {
        get { return _base; }
    }
    public int Level
    {
        get { return level;}
    }

    public int Exp { get; set; }
    public int HP {  get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }


    public Queue<string> StatusChanges { get; private set; }
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;

    public void Init()
    {
        //Gerador de Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.MoveBase));
            if(Moves.Count >= PokemonBase.MaxNumOfMoves)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        StatusChanges = new Queue<string>();
        CalculateStats();
        HP = MaxHp;
        ResetStatBoosts();
        Status = null;
        VolatileStatus = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
    }

    void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0},
            { Stat.Defense, 0},
            { Stat.SpAttack, 0},
            { Stat.SpDefense, 0},
            { Stat.Speed, 0},
            { Stat.Accuracy, 0},
            { Stat.Evasion, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        
        //Aplicar stats boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal * boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name} {stat} aumentou!");
            else
                StatusChanges.Enqueue($"{Base.Name} {stat} diminuiu!");

            Debug.Log($"{stat} foi boostado para {StatBoosts[stat]}");

        }
        
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn.MoveBase));
    }

    //calcular status com base no level do pokemon
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int MaxHp { get; private set; }

    //Causar dano no pokemon
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiviness = type,
            Critical = critical,
            Fainted = false,
        };


        //maneira diferente de escrever else if (se o IsSpecial for tru ele retorna SpAttack se nao ele retorn o Attack
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        //formula de dano bulbapedia
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack/ defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;

        Status = ConditionsDatabase.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDatabase.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var moveWithPP = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, moveWithPP.Count);
        return Moves[r];
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if(Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        //? = fucniona como not null se da esquerda nao for nulo executara a aprte da direita
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoosts();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiviness {  get; set; }
}
