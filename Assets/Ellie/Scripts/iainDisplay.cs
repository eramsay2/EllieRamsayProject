using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class iainDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI tmpText;
    
    public GameObject gPinky0;
    public GameObject gPinky1;
    public GameObject gPinky2;
    public GameObject gPinky3;
    public GameObject gPinky4;
   

    public GameObject[] joint = new GameObject[10];


    int fc = 0; 
    void Start()
    {
        tmpText.text = "Hello World"; 
    }

    // Update is called once per frame
    void Update()
    {
        joint[0].transform.position = gPinky0.transform.position;
        joint[1].transform.position = gPinky1.transform.position;
        joint[2].transform.position = gPinky2.transform.position;
        joint[3].transform.position = gPinky3.transform.position;
        joint[4].transform.position = gPinky4.transform.position;

        tmpText.text = fc.ToString() + " x: " + joint[0].transform.position.x.ToString("0.000");
        fc++;
    }
}
