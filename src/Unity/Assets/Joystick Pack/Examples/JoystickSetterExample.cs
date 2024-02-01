using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using TMPro;

public class JoystickSetterExample : MonoBehaviour
{
    [Header("Joystick Variables")]
    public VariableJoystick variableJoystick;
    public Text valueText;
    public Image background;
    public Sprite[] axisSprites;

    [Header("AR Events")]
    public GameObject tornado;
    public GameObject monster;
    public GameObject storm;
    public GameObject joystick;

    [Header("Debugger")]
    public TMP_Text debugger;

    private string serverIP = null;
    private IPAddress serverIPAddress = null;

    private int serverPort = 5000;

    private string endChar = "\n";
    private string intervalChar = "#";
    private string CMD_M_MOTOR = "CMD_M_MOTOR";
    private TcpClient client;
    private NetworkStream stream;
    private Socket socketClient;

    private bool carIsStopped = false;

    // Event Movement
    private float tornadoMovementX = 0.8f;
    private float tornadoMovementY = 0.7f;

    private float dragonMovementY = -0.8f;

    private float stormMovementY = 0.8f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        serverIP = PlayerPrefs.GetString("raspiIpAddress");
        serverIPAddress = IPAddress.Parse(PlayerPrefs.GetString("raspiIpAddress"));

        ConnectToServerStream();
    }

    private void Update()
    {
        if (tornado.active)
        {
            debugger.text = "Tornado: Active";
            valueText.text = "Current Value: (" + tornadoMovementX + ", " + tornadoMovementY + ")";
            joystick.SetActive(false);
            Move(tornadoMovementX, tornadoMovementY);
        }
        else if (monster.active){
            debugger.text = "Dragon: Active";
            valueText.text = "Current Value: (" + 0.0 + ", " + dragonMovementY + ")";
            Move(0, dragonMovementY);
        }
        else if (storm.active)
        {
            valueText.text = "Current Value: (" + 0.0 + ", " + stormMovementY + ")";
            debugger.text = "Storm: Active";
            Move(0, stormMovementY);
        }
        else
        {
            debugger.text = "Events: Inactive";
            joystick.SetActive(true);
            valueText.text = "Current Value: (" + variableJoystick.Direction.x + ", " + variableJoystick.Direction.y + ")";
            if (variableJoystick.Direction.magnitude > 0.1f)
            {
                Move(variableJoystick.Direction.x, variableJoystick.Direction.y);
                Thread.Sleep(100);
            }
            else
            {
                StopMovement();
                Thread.Sleep(100);
            }
        }
    }

    public void ModeChanged(int index)
    {
        switch(index)
        {
            case 0:
                variableJoystick.SetMode(JoystickType.Fixed);
                break;
            case 1:
                variableJoystick.SetMode(JoystickType.Floating);
                break;
            case 2:
                variableJoystick.SetMode(JoystickType.Dynamic);
                break;
            default:
                break;
        }     
    }

    public void AxisChanged(int index)
    {
        switch (index)
        {
            case 0:
                variableJoystick.AxisOptions = AxisOptions.Both;
                background.sprite = axisSprites[index];
                break;
            case 1:
                variableJoystick.AxisOptions = AxisOptions.Horizontal;
                background.sprite = axisSprites[index];
                break;
            case 2:
                variableJoystick.AxisOptions = AxisOptions.Vertical;
                background.sprite = axisSprites[index];
                break;
            default:
                break;
        }
    }

    public void SnapX(bool value)
    {
        variableJoystick.SnapX = value;
    }

    public void SnapY(bool value)
    {
        variableJoystick.SnapY = value;
    }

    private byte[] MakeMotorCommand(string commandString)
    {
        byte[] commandBytes = Encoding.ASCII.GetBytes(CMD_M_MOTOR + commandString);
        return commandBytes;

    }

    public void Move(float x, float y)
    {
        // plus 300 to get above threshold of 600
        int thrustY = Convert.ToInt16(y * 1000);
        int thrustX = Convert.ToInt16(x * 1000);
        Debug.Log($"the thrust in x direction is: {thrustX}");
        Debug.Log($"the thrust in y direction is: {thrustY}");
        int Turn = x > 0 ? -90 : 90;
        thrustX = Math.Abs(thrustX);
        string moveForwardCode = $"{intervalChar}0{intervalChar}{thrustY}{intervalChar}{Turn}{intervalChar}{thrustX}{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveForwardCode);
        SendData(commandBytes);
        carIsStopped = false;
    }

    public void DriveForward(double y)
    {
        // plus 300 to get above threshold of 600
        int thrust = Convert.ToInt16(y * 1000) + 300;
        Debug.Log($"the thrust is: {thrust}");
        string moveForwardCode = $"{intervalChar}0{intervalChar}{thrust}{intervalChar}0{intervalChar}0{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveForwardCode);
        SendData(commandBytes);
        carIsStopped = false;
    }
    public void DriveBackward(double y)
    {
        // minus 300 to get above threshold of -600
        int thrust = Convert.ToInt16(y * 1000) - 300;
        Debug.Log($"the thrust is: {thrust}");
        string moveForwardCode = $"{intervalChar}0{intervalChar}{thrust}{intervalChar}0{intervalChar}0{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveForwardCode);
        SendData(commandBytes);
        carIsStopped = false;
    }
    public void TurnLeft()
    {
        string moveLeftCode = $"{intervalChar}0{intervalChar}0{intervalChar}90{intervalChar}1500{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveLeftCode);
        SendData(commandBytes);
        carIsStopped = false;
    }
    public void DriveLeft()
    {
        string moveLeftCode = $"{intervalChar}0{intervalChar}500{intervalChar}90{intervalChar}1000{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveLeftCode);
        SendData(commandBytes);
        carIsStopped = false;
    }
    public void TurnRight()
    {
        string moveRightCode = $"{intervalChar}0{intervalChar}0{intervalChar}-90{intervalChar}1500{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveRightCode);
        SendData(commandBytes);
        carIsStopped = false;
    }

    public void DriveRight()
    {
        string moveRightCode = $"{intervalChar}0{intervalChar}500{intervalChar}-90{intervalChar}1000{endChar}";
        byte[] commandBytes = MakeMotorCommand(moveRightCode);
        SendData(commandBytes);
        carIsStopped = false;
    }

    public void StopMovement()
    {
        if (carIsStopped)
        {
            return;
        }
        string stopMovementCode = $"{intervalChar}0{intervalChar}0{intervalChar}0{intervalChar}0{endChar}";
        byte[] commandBytes = MakeMotorCommand(stopMovementCode);
        SendData(commandBytes);
        carIsStopped = true;
    }

    private void SendData(byte[] bytes)
    {
        // choose whatever works
        try
        {
            stream.Write(bytes);
            Debug.Log("Bytes Written");
        }
        catch (Exception e)
        {
            return;
        }
        //socketClient.Send(bytes);
    }

    private void ConnectToServerStream()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server.");
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