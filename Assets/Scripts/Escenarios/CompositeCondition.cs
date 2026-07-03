using System.Linq;

public sealed class CompositeCondition : IUnlockCondition
{
    private readonly IUnlockCondition[] _conditions;
    private readonly bool _requireAll;

    public CompositeCondition(IUnlockCondition[] conditions, bool requireAll)
    {
        _conditions = conditions;
        _requireAll = requireAll;
    }

    public bool IsMet(GameState state)
    {
        if (_requireAll)
            return _conditions.All(c => c.IsMet(state));

        return _conditions.Any(c => c.IsMet(state));
    }
}