// Author Nicolas Ostermann and Jakob Lingel
using System;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class CameraFeedReceiver : MonoBehaviour
{
    private string serverIP = null; // Replace with your Raspberry Pi's IP
    private int serverPort = 8000; // Replace with your streaming port

    private TcpClient client;
    public NetworkStream stream;
    private Thread receiveThread;
    public Texture2D camTexture;
    byte[] completeImageByte;

    public Material cameraMaterial;
    public Renderer displayRenderer; // Renderer to display the video feed
    private MemoryStream imageStream;
    public GameObject screenDisplay;
    public Image cameraImageHolder;

    public TMP_Text debugger = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        Debug.Log("Start Called");
        serverIP = PlayerPrefs.GetString("raspiIpAddress");
        camTexture = new Texture2D(640, 480);
        ConnectToServer();
        debugger.text = "Debugger Running";
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            debugger.text = "Client Set ";
            stream = client.GetStream();
            debugger.text = "Get Stream done";
            imageStream = new MemoryStream();
            debugger.text = "Image stream initialized";
            receiveThread = new Thread(new ThreadStart(ReceiveImage));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            debugger.text = "Receive Thread Running";
        }
        catch (System.Exception e)
        {
            debugger.text = "Error connecting to server: " + e.Message;
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }
    void Update()
    {
        try
        {
            // Load image data into the texture
            camTexture.LoadImage(completeImageByte);
            byte[] pngData = camTexture.EncodeToJPG();

            GetComponent<ARCameraBackground>().material.mainTexture = camTexture;

            Sprite tempSprite = Sprite.Create(camTexture, new Rect(0, 0, camTexture.width, camTexture.height), new Vector2(0, 0));

            cameraMaterial.mainTexture = camTexture;
            cameraImageHolder.sprite = tempSprite;
            debugger.text = debugger.text + "Image Set!";
        }
        catch(System.Exception e)
        {
            debugger.text = debugger.text + e.Message;
        }
    }

    private void ReceiveImage()
    {
        int bytesRead = 0;
        byte[] buffer = new byte[4]; // Adjust the buffer size as needed
        int byteArraySize = 1024;
        buffer = new byte[byteArraySize];
        bool processing = false;
        while (client.Connected)
        {
            if (stream != null && stream.DataAvailable)
            {
                buffer = new byte[byteArraySize];
                stream.Read(buffer, 0, byteArraySize);

                bytesRead += buffer.Length;

                int startOfImage = FindStartOfImage(buffer);
                int endOfImage = FindEndOfImage(buffer);

                if (startOfImage != -1 && endOfImage != -1 && startOfImage > endOfImage)
                {
                    // Write the last part of the image to the stream
                    imageStream.Write(buffer, 0, endOfImage + 2);
                    DisplayImage();
                    processing = false;

                    processing = true;
                    imageStream = new MemoryStream();
                    // Write the first part of the image to the stream
                    imageStream.Write(buffer, startOfImage, byteArraySize - startOfImage);
                }
                else if (startOfImage != -1)
                {
                    // Clear the MemoryStream for the next image
                    processing = true;
                    imageStream = new MemoryStream();
                    // Write the first part of the image to the stream
                    imageStream.Write(buffer, startOfImage, byteArraySize - startOfImage);
                }
                else if (endOfImage != -1)
                {
                    // Write the last part of the image to the stream
                    processing = false;
                    imageStream.Write(buffer, 0, endOfImage);
                    // Process the complete image
                    DisplayImage();
                }
                else if (processing)
                {
                    // Write the buffer to the stream
                    imageStream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }

    private int FindStartOfImage(byte[] buffer)
    {
        // Check if the first 10 bytes match the JPEG or Exif signature
        byte[] signature = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        // Get the start index of the signature in the buffer (Source: StackOverflow)
        var matchingIndexes = from index in Enumerable.Range(0, buffer.Length - signature.Length + 1)
                              where buffer.Skip(index).Take(signature.Length).SequenceEqual(signature)
                              select (int?)index;

        var matchingIndex = matchingIndexes.FirstOrDefault();

        return matchingIndex ?? -1;
    }

    private static byte[] TrimEnd(byte[] array)
    {
        int lastIndex = Array.FindLastIndex(array, b => b != 0);

        Array.Resize(ref array, lastIndex + 1);

        return array;
    }

    private void DisplayImage()
    {
        // Process the complete image
        completeImageByte = imageStream.ToArray();
    }

    int FindEndOfImage(byte[] buffer)
    {
        // Check if the image ends with the JPEG end-of-file marker (0xFF, 0xD9)
        for (int i = 0; i < buffer.Length - 2; i++)
        {
            if (buffer[i] == 0xFF && buffer[i + 1] == 0xD9)
            {
                return i;
            }
        }
        return -1;
    }
    public void CloseSocket()
    {
        client.Close();
    }
}