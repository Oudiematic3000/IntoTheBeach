using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public static LineGenerator Instance;
    public GameObject linePrefab;
    Line activeLine;
    public bool engaged=false;
    List<GameObject> drawnLines=new();
    private void Awake()
    {
        if(Instance==null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (engaged)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject newLine = Instantiate(linePrefab);
                activeLine = newLine.GetComponent<Line>();
                drawnLines.Add(newLine);
            }
            if(Input.GetMouseButtonDown(1))
            {
                for(int i = drawnLines.Count;i>0;i--) Destroy(drawnLines[i-1]);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            activeLine = null;
        }
        if (activeLine != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            activeLine.UpdateLine(mousePos);
        }

    }

 
}
