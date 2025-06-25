using Verse;

namespace DrugPolicyFix;

public class DrugPolicyFixComponent : GameComponent
{
    private bool fixedthisplay;

    public DrugPolicyFixComponent(Game game)
    {
    }

    public DrugPolicyFixComponent()
    {
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        if (!fixedthisplay)
        {
            DrugPolicyFix.CorrectPolicies();
            DrugPolicySort.SortPolicies();
        }

        fixedthisplay = true;
    }
}