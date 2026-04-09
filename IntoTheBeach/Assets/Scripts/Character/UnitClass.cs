using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="NewClass",menuName ="UnitClass")]
public class UnitClass : ScriptableObject
{
    public int moveRange = 2;
    public int health=5;
    public int damage = 1;
    [SerializeReference, SubclassSelector] public AttackPattern attackPattern;
    public GameObject unitGhost;

}
