using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System;

using System.Net;
using System.Net.Sockets;



public class iainHands : MonoBehaviour
{   
    public GameObject gElbowL;

    public GameObject gWriL;

    public GameObject gPinL0;
    public GameObject gPinL1;
    public GameObject gPinL2;
    public GameObject gPinL3;
    public GameObject gPinL4;

    public GameObject gRinL0;
    public GameObject gRinL1;
    public GameObject gRinL2;
    public GameObject gRinL3;
    public GameObject gRinL4;

    public GameObject gMidL0;
    public GameObject gMidL1;
    public GameObject gMidL2;
    public GameObject gMidL3;
    public GameObject gMidL4;

    public GameObject gIndL0;
    public GameObject gIndL1;
    public GameObject gIndL2;
    public GameObject gIndL3;
    public GameObject gIndL4;

    public GameObject gThuL0;
    public GameObject gThuL1;
    public GameObject gThuL2;
    public GameObject gThuL3;
    public GameObject gThuL4;

    public GameObject gElbowR;

    public GameObject gWriR;

    public GameObject gPinR0;
    public GameObject gPinR1;
    public GameObject gPinR2;
    public GameObject gPinR3;
    public GameObject gPinR4;

    public GameObject gRinR0;
    public GameObject gRinR1;
    public GameObject gRinR2;
    public GameObject gRinR3;
    public GameObject gRinR4;

    public GameObject gMidR0;
    public GameObject gMidR1;
    public GameObject gMidR2;
    public GameObject gMidR3;
    public GameObject gMidR4;

    public GameObject gIndR0;
    public GameObject gIndR1;
    public GameObject gIndR2;
    public GameObject gIndR3;
    public GameObject gIndR4;

    public GameObject gThuR0;
    public GameObject gThuR1;
    public GameObject gThuR2;
    public GameObject gThuR3;
    public GameObject gThuR4;


    short[] xPos = new short[52];
    short[] yPos = new short[52];
    short[] zPos = new short[52];

    byte [] coordsBuffer;

    public TextMeshProUGUI tmpText;
    private GameObject[] joint = new GameObject[52];
    int fc = 0;

       string remoteIPAddress = "192.168.8.100"; //MSI
   // string remoteIPAddress = "10.56.135.147"; //alienware
    //string remoteIPAddress = "172.20.10.12"; //alienware


    // string remoteIPAddress = "127.0.0.1";

    int remotePort = 9880;
    UdpClient udpClient = new UdpClient();
    void Start()
    {
        Debug.Log("Made Change to test github");

        tmpText.text = "Hello World";

        for (int i = 0; i<52; i++)
        {
            joint[i] = transform.GetChild(i).gameObject;  // Gets the first child
            Debug.Log("First child attached: " + joint[i].name);

        }

        

    }

    // Update is called once per frame
    void LateUpdate()
    {
        DateTime currentTime = DateTime.Now;
        long ticks = currentTime.Ticks;

        int k = 0;

        joint[0].transform.position = gElbowR.transform.position;

        joint[1].transform.position = gWriL.transform.position;

        joint[2].transform.position = gPinL0.transform.position;
        joint[3].transform.position = gPinL1.transform.position;
        joint[4].transform.position = gPinL2.transform.position;
        joint[5].transform.position = gPinL3.transform.position;
        joint[6].transform.position = gPinL4.transform.position;

        joint[7].transform.position = gRinL0.transform.position;
        joint[8].transform.position = gRinL1.transform.position;
        joint[9].transform.position = gRinL2.transform.position;
        joint[10].transform.position = gRinL3.transform.position;
        joint[11].transform.position = gRinL4.transform.position;

        joint[12].transform.position = gMidL0.transform.position;
        joint[13].transform.position = gMidL1.transform.position;
        joint[14].transform.position = gMidL2.transform.position;
        joint[15].transform.position = gMidL3.transform.position;
        joint[16].transform.position = gMidL4.transform.position;

        joint[17].transform.position = gIndL0.transform.position;
        joint[18].transform.position = gIndL1.transform.position;
        joint[19].transform.position = gIndL2.transform.position;
        joint[20].transform.position = gIndL3.transform.position;
        joint[21].transform.position = gIndL4.transform.position;

        joint[22].transform.position = gThuL0.transform.position;
        joint[23].transform.position = gThuL1.transform.position;
        joint[24].transform.position = gThuL2.transform.position;
        joint[25].transform.position = gThuL3.transform.position;
        joint[26].transform.position = gThuL4.transform.position;

        joint[27].transform.position = gElbowR.transform.position;

        joint[28].transform.position = gWriR.transform.position;

        joint[29].transform.position = gPinR0.transform.position;
        joint[30].transform.position = gPinR1.transform.position;
        joint[31].transform.position = gPinR2.transform.position;
        joint[32].transform.position = gPinR3.transform.position;
        joint[33].transform.position = gPinR4.transform.position;

        joint[34].transform.position = gRinR0.transform.position;
        joint[35].transform.position = gRinR1.transform.position;
        joint[36].transform.position = gRinR2.transform.position;
        joint[37].transform.position = gRinR3.transform.position;
        joint[38].transform.position = gRinR4.transform.position;

        joint[39].transform.position = gMidR0.transform.position;
        joint[40].transform.position = gMidR1.transform.position;
        joint[41].transform.position = gMidR2.transform.position;
        joint[42].transform.position = gMidR3.transform.position;
        joint[43].transform.position = gMidR4.transform.position;

        joint[44].transform.position = gIndR0.transform.position;
        joint[45].transform.position = gIndR1.transform.position;
        joint[46].transform.position = gIndR2.transform.position;
        joint[47].transform.position = gIndR3.transform.position;
        joint[48].transform.position = gIndR4.transform.position;

        joint[49].transform.position = gThuR0.transform.position;
        joint[50].transform.position = gThuR1.transform.position;
        joint[51].transform.position = gThuR2.transform.position;
        joint[52].transform.position = gThuR3.transform.position;
        joint[53].transform.position = gThuR4.transform.position;


        for (int i = 0; i < 54; i++)
        {
            xPos[i] = (short)(joint[i].transform.position.x * 1000);
            yPos[i] = (short)(joint[i].transform.position.y * 1000);
            zPos[i] = (short)(joint[i].transform.position.z * 1000);
        }
        byte[] byteTicks = BitConverter.GetBytes(ticks);
        byte[] byteArrayx = ConvertShortArrayToByteArray(xPos);
        byte[] byteArrayy = ConvertShortArrayToByteArray(yPos);
        byte[] byteArrayz = ConvertShortArrayToByteArray(zPos);
        byte[] combinedArray = CombineArrays(byteTicks, byteArrayx, byteArrayy, byteArrayz);

        tmpText.text = fc.ToString() + " " + k.ToString() + " x: " + joint[0].transform.position.x.ToString("0.000");
        tmpText.text += ticks.ToString() + "\r\n" ;
        tmpText.text += combinedArray.Length.ToString() + "\r\n" ;

        // Send the byte array to the remote host
        udpClient.Send(combinedArray, combinedArray.Length, remoteIPAddress, remotePort);
        tmpText.text += "Message sent via UDP v2!" + remoteIPAddress + " " + remotePort;

        


        fc++;
    }
    public static byte[] CombineArrays(byte[] array1, byte[] array2, byte[] array3,byte[] array4)
    {
        // Create a new array large enough to hold all three arrays
        byte[] combinedArray = new byte[array1.Length + array2.Length + array3.Length + array4.Length];

        // Copy the arrays into the new array
        Buffer.BlockCopy(array1, 0, combinedArray, 0, array1.Length);
        Buffer.BlockCopy(array2, 0, combinedArray, array1.Length, array2.Length);
        Buffer.BlockCopy(array3, 0, combinedArray, array1.Length + array2.Length, array3.Length);
        Buffer.BlockCopy(array4, 0, combinedArray, array1.Length + array2.Length + array3.Length, array4.Length);

        return combinedArray;
    }
    public static byte[] ConvertShortArrayToByteArray(short[] shortArray)
    {
        // Create a byte array with double the size of the short array (2 bytes per short)
        byte[] byteArray = new byte[shortArray.Length * 2];

        for (int i = 0; i < shortArray.Length; i++)
        {
            // Convert each short to bytes using BitConverter and copy to the byte array
            byte[] shortBytes = BitConverter.GetBytes(shortArray[i]);

            // Copy the 2 bytes for the short to the byte array
            Array.Copy(shortBytes, 0, byteArray, i * 2, 2);
        }

        return byteArray;
    }
}
