using System;
using System.Collections.Generic;

public static class AttackPatternRegistry
{
    public const int GunslingerAttack = 0;
    public const int BouncerAttack = 1;

    private static readonly Dictionary<int, Func<AttackPattern>> factories =
        new Dictionary<int, Func<AttackPattern>>
        {
            { GunslingerAttack, () => new GunslingerAttackPattern() },
            { BouncerAttack, () => new BouncerAttackPattern() },
            
        };

    public static AttackPattern FromID(int id)
    {
        if (factories.TryGetValue(id, out var factory))
            return factory();
        throw new ArgumentException($"Unknown AttackPattern type ID: {id}");
    }
}