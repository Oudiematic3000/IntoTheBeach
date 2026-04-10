using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitAnimations", menuName = "Scriptable Objects/UnitAnimations")]
public class UnitAnimations : ScriptableObject
{
    public Sprite idleLeft, idleRight, idleTop, idleBottom;
    public Sprite walkLeft, walkRight, walkTop, walkBottom;
    public Sprite attackLeft, attackRight, attackTop, attackBottom;
    public enum AnimState 
    {
        None,
        Move,
        Attack
    }
    public Sprite GetSpriteAnim(AnimState state, int direct) 
    {
        if (state ==  AnimState.Move) 
        {
            switch (direct)
            {
                case 0: return walkLeft;
                case 1: return walkTop;
                case 2: return walkRight;
                case 3: return walkBottom;
            }
        }
        if (state == AnimState.Attack)
        {
            switch (direct)
            {
                case 0: return attackLeft;
                case 1: return attackTop;
                case 2: return attackRight;
                case 3: return attackBottom;
            }
        }
        if(state == AnimState.None) {
            switch (direct)
            {
                case 0: return idleLeft;
                case 1: return idleTop;
                case 2: return idleRight;
                case 3: return idleBottom;
            }
        }
        return null;
    }

   

}
