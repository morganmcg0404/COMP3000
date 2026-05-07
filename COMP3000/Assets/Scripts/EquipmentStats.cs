using UnityEngine;

[System.Serializable]
public struct EquipmentStats
{
    [SerializeField] private int accuracy;
    [SerializeField] private int defence;
    [SerializeField] private int strength;
    [SerializeField] private int attackSpeed;

    public int Accuracy => accuracy;
    public int Defence => defence;
    public int Strength => strength;
    public int AttackSpeed => attackSpeed;

    public EquipmentStats(int accuracy, int defence, int strength, int attackSpeed = 0)
    {
        this.accuracy = accuracy;
        this.defence = defence;
        this.strength = strength;
        this.attackSpeed = attackSpeed;
    }

    public static EquipmentStats operator +(EquipmentStats a, EquipmentStats b)
    {
        return new EquipmentStats(
            a.accuracy + b.accuracy,
            a.defence + b.defence,
            a.strength + b.strength,
            a.attackSpeed + b.attackSpeed
        );
    }

    public static EquipmentStats operator -(EquipmentStats a, EquipmentStats b)
    {
        return new EquipmentStats(
            a.accuracy - b.accuracy,
            a.defence - b.defence,
            a.strength - b.strength,
            a.attackSpeed - b.attackSpeed
        );
    }

    public static EquipmentStats Zero => new EquipmentStats(0, 0, 0);

    public static bool operator ==(EquipmentStats a, EquipmentStats b)
    {
        return a.accuracy == b.accuracy && a.defence == b.defence && a.strength == b.strength && a.attackSpeed == b.attackSpeed;
    }

    public static bool operator !=(EquipmentStats a, EquipmentStats b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is EquipmentStats other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + accuracy.GetHashCode();
            hash = hash * 23 + defence.GetHashCode();
            hash = hash * 23 + strength.GetHashCode();
            hash = hash * 23 + attackSpeed.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return $"Accuracy: {accuracy}, Defence: {defence}, Strength: {strength}";
    }
}
