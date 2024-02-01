using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;

namespace Assets
{
    public class CarDriveController: MonoBehaviour
    {

        //public static CarDriveController carDriveController = new CarDriveController();

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

        void Start()
        {
            serverIP = PlayerPrefs.GetString("raspiIpAddress");
            serverIPAddress = IPAddress.Parse(PlayerPrefs.GetString("raspiIpAddress"));
            
            ConnectToServerStream();
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
        public void DriveBackward(double y )
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
            catch(Exception e) 
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
}
