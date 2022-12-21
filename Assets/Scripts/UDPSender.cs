using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;

public class UDPSender : MonoBehaviour
{
    public string dataToSend = "Hello from Unity"; //String data to send across via UDP

    [SerializeField] string IP = "127.0.0.1"; // local host
    [SerializeField] int txPort = 8001; // port to send data to

    //164.11.73.69 - joseph
    //164.11.72.79 - me

    // Create necessary UdpClient object
    UdpClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = new UdpClient(txPort); //assign so that there is no null reference later
        client.Connect(IP, txPort); //Connect to the IP and port
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) //Check when spacebar is pressed
        {
            UDPSend(dataToSend); //Call function to send the data
        }
    }

    void UDPSend(string message)
    {
        try // Try block to try sending data
        {
            byte[] sendBytes = Encoding.ASCII.GetBytes(message); //convert string data to bytes
            client.Send(sendBytes, sendBytes.Length); //Send the data
        }
        catch (Exception e)
        {
            Debug.Log("Exception thrown " + e.ToString()); // Print error message
        }
    }
}
