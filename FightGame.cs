using Godot;
using System;

public partial class FightGame : Node2D
{
    private LineEdit nameInput;
    private Button confirmButton;
    private Button quickAttackButton;
    private Button heavyAttackButton;
    private Button preciseAttackButton;
    private Button healButton;
    private Label playerLabel = null!;
    private Player playerOne;
    private Player playerTwo;

    private int round = 1;
    private HealthBar playerHealthBar = null!;
    private HealthBar monsterHealthBar = null!;
    private Control endScreen = null!;
    private Label endScreenLabel = null!;
    private Button endScreenReplayButton = null!;

    private Button playButton = null!;

    private Control mainMenu = null!;

    private Control nameMenu = null!;

    private Sprite2D sprite = null!;

    private Sprite2D monsterSprite = null!;

    public override void _Ready()
    {
        // Find the nodes that already exist in the Godot scene.
        nameInput = GetNode<LineEdit>("NameMenu/NameInput");
        confirmButton = GetNode<Button>("NameMenu/NameConfirmButton");
        quickAttackButton = GetNode<Button>("PanelContainer/GridContainer/QuickAttackButton");
        heavyAttackButton = GetNode<Button>("PanelContainer/GridContainer/HeavyAttackButton");
        preciseAttackButton = GetNode<Button>("PanelContainer/GridContainer/PreciseAttackButton");
        healButton = GetNode<Button>("PanelContainer/GridContainer/HealButton");
        playerLabel = GetNode<Label>("PlayerLabel");
        playerHealthBar = GetNode<HealthBar>("PlayerHealthBar");
        monsterHealthBar = GetNode<HealthBar>("VBoxContainer/MonsterHealthBar");
        endScreen = GetNode<Control>("EndScreen");
        endScreenLabel = GetNode<Label>("EndScreen/PanelContainer/GridContainer/Label");
        endScreenReplayButton = GetNode<Button>("EndScreen/PanelContainer/GridContainer/EndScreenReplayButton");
        mainMenu = GetNode<Control>("MainMenu");
        playButton = GetNode<Button>("MainMenu/PanelContainer/GridContainer/PlayButton");
        nameMenu = GetNode<Control>("NameMenu");
        sprite = GetNode<Sprite2D>("Player");
        monsterSprite = GetNode<Sprite2D>("VBoxContainer/Monster");

        confirmButton.Pressed += ConfirmName;
        quickAttackButton.Pressed += QuickAttackPressed;
        heavyAttackButton.Pressed += HeavyAttackPressed;
        preciseAttackButton.Pressed += PreciseAttackPressed;
        healButton.Pressed += HealPressed;
        endScreenReplayButton.Pressed += ReplayGame;
        playButton.Pressed += NameMenu; 

        SetActionButtonsDisabled(true);
    }

private void ReplayGame()
    {
        GetTree().ReloadCurrentScene();
    }


private void MainMenu()
    {
        mainMenu.Visible = true;
        
    }

private void NameMenu()
    {
        mainMenu.Visible = false;

    }

public void AnimateAttack()
    {
var currentPosition = sprite.Position;
var tween = CreateTween();
tween.TweenProperty(sprite, "position", 
    new Vector2(currentPosition.X + 30, currentPosition.Y - 5), 
    0.1f)
    .SetTrans(Tween.TransitionType.Quad)
    .SetEase(Tween.EaseType.Out);

tween.TweenProperty(sprite, "position", currentPosition, 0.1f).SetDelay(0.1f);

    }

    public void AnimateMonsterAttack()
    {
var currentPosition = monsterSprite.Position;
var tween = CreateTween();
tween.TweenProperty(monsterSprite, "position", 
    new Vector2(currentPosition.X - 30, currentPosition.Y + 5), 
    0.1f)
    .SetTrans(Tween.TransitionType.Quad)
    .SetEase(Tween.EaseType.Out);

tween.TweenProperty(monsterSprite, "position", currentPosition, 0.1f).SetDelay(0.1f);

    }

public void AnimatePlayerHit()
    {
        var currentPosition = sprite.Position;
        var tween = CreateTween();
        tween.TweenProperty(sprite, "position",
        new Vector2(currentPosition.X, currentPosition.Y - 10), 0.1f)
        .SetTrans(Tween.TransitionType.Quad)
        .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(sprite, "position", currentPosition, 0.1f).SetDelay(0.1f);
    }

    public void AnimateMonsterHit()
    {
        var currentPosition = monsterSprite.Position;
        var tween = CreateTween();
        tween.TweenProperty(monsterSprite, "position",
        new Vector2(currentPosition.X, currentPosition.Y - 10), 0.1f)
        .SetTrans(Tween.TransitionType.Quad)
        .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(monsterSprite, "position", currentPosition, 0.1f).SetDelay(0.1f);
    }
private void ConfirmName()
{
    nameMenu.Visible = false;
    endScreen.Visible = false;
    string enteredName = nameInput.Text.Trim();


    if (string.IsNullOrWhiteSpace(enteredName))
    {
        enteredName = "Parry Hotter";
    }

    playerOne = new Player(enteredName.ToUpper(), 100, true);
    playerTwo = new Player("Grimace", 100, false);

    playerLabel.Text = playerOne.Name;

    SetActionButtonsDisabled(false);
    confirmButton.Disabled = true;
    nameInput.Editable = false;
}

    private void QuickAttackPressed()
    {
        RunTurn(1);
    }

    private void HeavyAttackPressed()
    {
        RunTurn(2);
    }

    private void PreciseAttackPressed()
    {
        RunTurn(3);
    }

    private void HealPressed()
    {
        RunTurn(4);
    }

   private async void RunTurn(int selectedAction)
{
    if (playerOne == null || playerTwo == null)
    {
        GD.Print("Enter and confirm your name first.");
        return;
    }

    if (!playerOne.IsAlive || !playerTwo.IsAlive)
    {
        return;
    }

    SetActionButtonsDisabled(true);

    GD.Print($"------ Round {round} ------");

    // Player's turn
    int monsterHealthBefore = playerTwo.Health;

    playerOne.PerformAction(selectedAction, playerTwo);

    if (selectedAction != 4)
    {
        AnimateAttack();

        await ToSignal(
            GetTree().CreateTimer(0.25f),
            SceneTreeTimer.SignalName.Timeout
        );
    }

    if (playerTwo.Health < monsterHealthBefore)
    {
        AnimateMonsterHit();
    }

    monsterHealthBar.SetHealth(playerTwo.Health);

    await ToSignal(
        GetTree().CreateTimer(0.5f),
        SceneTreeTimer.SignalName.Timeout
    );

    // Monster's turn
    if (playerTwo.IsAlive)
    {
        int playerHealthBefore = playerOne.Health;

        int monsterAction =
            playerTwo.PerformRandomAction(playerOne);

        if (monsterAction != 4)
        {
            AnimateMonsterAttack();

            await ToSignal(
                GetTree().CreateTimer(0.25f),
                SceneTreeTimer.SignalName.Timeout
            );
        }

        if (playerOne.Health < playerHealthBefore)
        {
            AnimatePlayerHit();
        }

        playerHealthBar.SetHealth(playerOne.Health);
    }

    await ToSignal(
        GetTree().CreateTimer(0.4f),
        SceneTreeTimer.SignalName.Timeout
    );

    CheckForWinner();

    if (playerOne.IsAlive && playerTwo.IsAlive)
    {
        SetActionButtonsDisabled(false);
    }

    round++;
}

    private void CheckForWinner()
    {
        if (!playerOne.IsAlive)
        {
            GD.Print($"------ END ------");
            GD.Print($"{playerOne.Name} died. RIP.");
            SetActionButtonsDisabled(true);
            endScreen.Visible = true;
            endScreenLabel.Text = "Du hast verloren!".ToUpper();
        }
        else if (!playerTwo.IsAlive)
        {
            GD.Print($"------ END ------");
            GD.Print($"{playerTwo.Name} died. RIP.");
            SetActionButtonsDisabled(true);
            endScreen.Visible = true;
            endScreenLabel.Text = "Du hast gewonnen!".ToUpper();
        }
    }

    private void SetActionButtonsDisabled(bool disabled)
    {
        quickAttackButton.Disabled = disabled;
        heavyAttackButton.Disabled = disabled;
        preciseAttackButton.Disabled = disabled;
        healButton.Disabled = disabled;
    }
}

public class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public bool IsAlive { get; set; }
    public bool IsHuman { get; set; }

    public Attack QuickAttack { get; }
    public Attack HeavyAttack { get; }
    public Attack PreciseAttack { get; }

    private static readonly Random Rnd = new Random();

    public Player(string name, int health, bool isHuman)
    {
        Name = name;
        Health = health;
        IsAlive = health > 0;
        IsHuman = isHuman;

        QuickAttack = new Attack(
            name: "Quick Attack",
            damage: 10,
            hitChance: 95,
            criticalChance: 10
        );

        HeavyAttack = new Attack(
            name: "Heavy Attack",
            damage: 30,
            hitChance: 55,
            criticalChance: 20
        );

        PreciseAttack = new Attack(
            name: "Precise Attack",
            damage: 15,
            hitChance: 100,
            criticalChance: 5
        );
    }

    public void PerformAction(int action, Player opponent)
    {
        switch (action)
        {
            case 1:
                QuickAttack.Execute(this, opponent);
                break;

            case 2:
                HeavyAttack.Execute(this, opponent);
                break;

            case 3:
                PreciseAttack.Execute(this, opponent);
                break;

            case 4:
                Heal();
                break;

            default:
                GD.Print("Invalid action.");
                break;
        }
    }

public int PerformRandomAction(Player opponent)
{
    int randomAction = Rnd.Next(1, 5);
    PerformAction(randomAction, opponent);

    return randomAction;
}
    public void Heal()
    {
        int healAmount = Rnd.Next(10, 20);

        Health += healAmount;

        if (Health > 100)
        {
            Health = 100;
        }

        GD.Print($"{Name} healed for {healAmount} HP.");
        GD.Print($"{Name}: {Health} HP left.");
    }
}

public class Attack
{
    public string Name { get; }
    public int Damage { get; }
    public int HitChance { get; }
    public int CriticalChance { get; }

    public Attack(
        string name,
        int damage,
        int hitChance,
        int criticalChance
    )
    {
        Name = name;
        Damage = damage;
        HitChance = hitChance;
        CriticalChance = criticalChance;
    }

    public void Execute(Player attacker, Player target)
    {
        int hitRoll = Random.Shared.Next(1, 101);

        if (hitRoll > HitChance)
        {
            GD.Print($"{attacker.Name}'s {Name} missed!");
            return;
        }

        int finalDamage = Damage;
        int criticalRoll = Random.Shared.Next(1, 101);

        if (criticalRoll <= CriticalChance)
        {
            finalDamage *= 2;
            GD.Print("Critical hit!");
        }

        target.Health -= finalDamage;

        if (target.Health <= 0)
        {
            target.Health = 0;
            target.IsAlive = false;
        }

        GD.Print(
            $"{attacker.Name} used {Name} and dealt " +
            $"{finalDamage} damage to {target.Name}."
        );

        GD.Print($"{target.Name}: {target.Health} HP left.");
    }
}
