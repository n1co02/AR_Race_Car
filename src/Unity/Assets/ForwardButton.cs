using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ForwardButton : MonoBehaviour
{
    public string serverIP = "141.70.43.62"; 
    public IPAddress serverIPAddress = IPAddress.Parse("141.70.43.62");
    public int serverPort = 5000; 

    private string endChar = "\n";
    private string intervalChar = "#";
    private string CMD_M_MOTOR = "CMD_M_MOTOR";
    private TcpClient client;
    private NetworkStream stream;
    private Socket socketClient;
    // Start is called before the first frame update
    void Start()
    {
        // try out both by (un)commenting the lines in SendData()
        ConnectToServerStream();
        ConnectToServerSocket();
    }
    void Update()
    {
        // works for touch as well
        if(Input.GetMouseButton(0))
        {
            Debug.Log("Pressed left click.");
            OnBtnForward();
        }
        if (Input.GetMouseButtonUp(0)) 
        {
            Console.WriteLine("Released left click.");
            StopMovement();
        }
    }

    private byte[] MakeMotorCommand(string commandString)
    {
        byte[] commandBytes = Encoding.ASCII.GetBytes(CMD_M_MOTOR + commandString);
        return commandBytes;

    }

    public void OnBtnForward()
    {
        string moveForwardCode = $"{intervalChar}0{intervalChar}1500{intervalChar}0{intervalChar}0{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveForwardCode);
        SendData(commandBytes);
    }

    private void StopMovement()
    {
        string stopMovementCode = $"{intervalChar}0{intervalChar}0{intervalChar}0{intervalChar}0{endChar}";
        byte[] commandBytes = MakeMotorCommand(stopMovementCode);
        SendData(commandBytes);
    }

    private void SendData(byte[] bytes)
    {
        // choose whatever works
        stream.Write(bytes);
        //socketClient.Send(bytes);
    }

    private void ConnectToServerStream()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    private void ConnectToServerSocket()
    {
        IPEndPoint ipEndPoint = new(serverIPAddress, serverPort);
        socketClient = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
    }

}
