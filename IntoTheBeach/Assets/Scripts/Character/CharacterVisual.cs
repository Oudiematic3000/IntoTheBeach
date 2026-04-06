using System;
using UnityEngine;


public class CharacterVisual : MonoBehaviour, Iinteractable
{
    public static event Action<CharacterVisual> OnClick;
   
    public void OnHover(Vector2 mousePos)
    {
        print("My mom");
    }

    public void OnPress(Vector2 mousePos)
    {
        OnClick?.Invoke(this);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


}
