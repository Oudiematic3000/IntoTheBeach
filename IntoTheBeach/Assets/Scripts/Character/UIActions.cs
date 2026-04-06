using System;
using UnityEngine;

public class UIActions : MonoBehaviour
{
    public static event Action OnMovement;
    public static event Action OnAttack;



    public void MoveButtonPressed() 
    {
        OnMovement?.Invoke();
    }
}
