using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class UDPReceiver : MonoBehaviour
{
    [SerializeField] int rxPort = 6500; // port to receive data from

    private UdpClient server; //Creating a UDP server object

    private IPEndPoint ipEndPointData; //Using this to connect to all IPs and specific port

    private object obj = null;
    private System.AsyncCallback AC; //Creating an Async Callback variable
    byte[] receivedBytes; //Variable to store the received bytes from UDP message

    void Start()
    {
        InitializeUDPListener();
    }
    public void InitializeUDPListener()
    {
        ipEndPointData = new IPEndPoint(IPAddress.Any, rxPort); //Initialize IP endpoint
        server = new UdpClient(); //Initialize UDP Client
        server.Client.Bind(ipEndPointData); //Bind the client to the IP endpoint
        AC = new System.AsyncCallback(ReceivedUDPPacket); //Initialize the callback variable
        server.BeginReceive(AC, obj); //Start receiving the message on client using the callback
        Debug.Log("UDP - Start Receiving.."); //Show that message receiving has begun
    }

    void ReceivedUDPPacket(System.IAsyncResult result)
    {
        receivedBytes = server.EndReceive(result, ref ipEndPointData); //store received message in variable

        ParsePacket(); //Do something with the received data

        server.BeginReceive(AC, obj); //start the receiving step again
    } // ReceiveCallBack

    void ParsePacket()
    {
        // work with receivedBytes
        Debug.Log(Encoding.ASCII.GetString(receivedBytes)); //Print the data in console
    }

    void OnDestroy() //To close the server when program is closed
    {
        if (server != null)
        {
            server.Close();
        }
    }
}
