// Author Nicolas Ostermann

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
// using System.IO.Ports; // You might need a different library depending on  Bluetooth setup

// This Class should get the data from the raspberryPI (FPS, Temperature, Distance, Distance left)
public class DataComponent : MonoBehaviour
{
   
    [SerializeField] public TMP_Text fps = null;
    [SerializeField] public TMP_Text temprature = null;
    [SerializeField] public TMP_Text distance = null;
    [SerializeField] public TMP_Text distanceLeft = null;
    // Example for you and to test UI 
    private void Start()
    {
        // Start is called before the first frame update
        // Used here to start the UpdateData coroutine
        StartCoroutine(UpdateData());
    }
    // CoRoutine to Update Data
    public IEnumerator UpdateData()
    {
        for (int i = 0; i < 20; i++)
        {
            setFps(i.ToString());
            setTemprature(i.ToString());
            setDistance(i.ToString());
            setDistanceLeft(i.ToString());
            yield return new WaitForSeconds(1); // Wait for 1 second
        }
    }
    // Method to set the fps text
    // inputFps is the string that will be displayed in the TMP_Text component
    public void setFps(string inputFps)
    {
        fps.text = inputFps;
    }
    public void setTemprature(string inputTemprature)
    {
        temprature.text = inputTemprature;
    }
    public void setDistance(string inputDistance)
    {
        distance.text = inputDistance;
    }
    public void setDistanceLeft(string inputDistanceLeft)
    {
        distanceLeft.text = inputDistanceLeft;
    }





















    /*
    private SerialPort serialPort; // For Bluetooth connection
    private string receivedData; // To store received data

    void Start()
    {
        // Initialize and open your Bluetooth connection here
        // Example: serialPort = new SerialPort("COM3", 9600);
        // serialPort.Open();
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            // Read data from the Bluetooth connection
            receivedData = serialPort.ReadLine();
            ProcessReceivedData(receivedData);
        }
    }

    void ProcessReceivedData(string data)
    {
        // Parse your data here
        // Example: Splitting the received string into different parts
        // and converting them to the respective data types
        var dataParts = data.Split(',');
        float fps = float.Parse(dataParts[0]);
        float temperature = float.Parse(dataParts[1]);
        float distance = float.Parse(dataParts[2]);
        float distanceLeft = float.Parse(dataParts[3]);

        // Use this data as needed
    }

    void OnDestroy()
    {
        // Close the serial port when the object is destroyed
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }*/


}
