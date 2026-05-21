using UnityEngine;

public class Compendium : MonoBehaviour
{
   [SerializeField] private GameObject compendium;
    [SerializeField] private GameObject gunSlinger;
    [SerializeField] private GameObject gunSlingerButton;
    [SerializeField] private GameObject drunkard;
    [SerializeField] private GameObject drunkardButton;
    [SerializeField] private GameObject bouncer;
    [SerializeField] private GameObject bouncerButton;


    // Update is called once per frame
    void Update()
    {
        if (gunSlinger.activeInHierarchy)
        {
            ResetButtons();
            gunSlingerButton.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(-600, gunSlingerButton.GetComponent<RectTransform>().anchoredPosition.y);
        }

        if (drunkard.activeInHierarchy)
        {
            ResetButtons();
            drunkardButton.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(-600, drunkardButton.GetComponent<RectTransform>().anchoredPosition.y);
        }

        if (bouncer.activeInHierarchy)
        {
            ResetButtons();
            bouncerButton.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(-600, bouncerButton.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
   
    private void ResetButtons() 
    {
        gunSlingerButton.GetComponent<RectTransform>().anchoredPosition =
              new Vector2(-560, gunSlingerButton.GetComponent<RectTransform>().anchoredPosition.y);
        drunkardButton.GetComponent<RectTransform>().anchoredPosition =
               new Vector2(-560, drunkardButton.GetComponent<RectTransform>().anchoredPosition.y);
        bouncerButton.GetComponent<RectTransform>().anchoredPosition =
               new Vector2(-560, bouncerButton.GetComponent<RectTransform>().anchoredPosition.y);
        
        
    }
    public void GunslingerSelection() 
    {
        ResetButtons();
        gunSlinger.SetActive(true);
        drunkard.SetActive(false);
        bouncer.SetActive(false);
    }
    public void DrunkardSelection()
    {
        ResetButtons();
        drunkard.SetActive(true);
        gunSlinger.SetActive(false);
        bouncer.SetActive(false);

    }
    public void BouncerSelection()
    {
        ResetButtons();
        bouncer.SetActive(true);
        drunkard.SetActive(false);
        gunSlinger.SetActive(false);

    }
}
