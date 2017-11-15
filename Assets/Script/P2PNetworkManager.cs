using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class P2PNetworkManager : MonoBehaviour, IDisposable
{

    private int PORTNUM;
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private IPAddress address;
    private IPEndPoint endPoint;
    private bool connect = false;
    Thread thread;
    UdpClient server;
    SpeexScript speex;

    string serverAddress;

    public bool GetConnect()
    {
        return connect;
    }

    public void CreateP2PNetworkManager(string _address,int _port)
    {
        PORTNUM = _port;
        connect = true;
        serverAddress = _address.Substring(7);
        thread = new Thread(DoWork);
        thread.IsBackground = true;
        thread.Start();
        speex = new SpeexScript();
    }

    void OnApplicationQuit()
    {
        Dispose();
    }


    public void Send(float[] _audio)
    {
        speex.CreateDecoder();
        speex.CreateEncoder();
        byte[] datagram = speex.AudioEncode(_audio, _audio.Length);

        //float[] outbuffer = speex.AudioDecode(buffer, buffer.Length);
        ////Debug.Log(outbuffer.Length);
        //// (1) UdpClient 객체 성성
        UdpClient client = new UdpClient();

        //// (2) 데이타 송신
        client.Send(datagram, datagram.Length, serverAddress, PORTNUM);

        //// (4) UdpClient 객체 닫기
        client.Close();
    }

    void DoWork()
    {
        try
        {
            server = new UdpClient(PORTNUM);
        }
        catch(Exception e)
        {
            //Debug.Log(e.Message);
        }
        // 클라이언트 IP를 담을 변수
        while (true)
        {
            Thread.Sleep(100);

            try
            {
                // (2) 데이타 수신
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] datagram = server.Receive(ref remoteEP);
                float[] outbuffer = speex.AudioDecode(datagram, datagram.Length);
                //NetworkManager.getInstance().OtherPlayerVoice(outbuffer);
            }
            catch (Exception e)
            {
                //Debug.Log(e.Message);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (thread != null)
        {
            connect = false;
            if(server!=null)
                server.Close();
            thread.Interrupt();
            thread.Abort();
            thread = null;
        }
    }
}
