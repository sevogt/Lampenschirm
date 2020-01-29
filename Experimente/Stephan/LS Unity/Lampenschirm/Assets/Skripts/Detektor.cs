using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class Detektor : MonoBehaviour
{
    ConcurrentQueue<(float, float)> results = new ConcurrentQueue<(float, float)>();

    Thread backgroundUDP;
    bool stopUdpServ = false;
    object lockObj = new object();

    public string LADDR = "127.0.0.1";
    public int LPORT = 54321;

    void backgroundUDPCallback(IAsyncResult r) {
        var udpClient = (((UdpClient, IPEndPoint))(r.AsyncState)).Item1;
        var rSock = (((UdpClient, IPEndPoint))(r.AsyncState)).Item2;
        try {
            var recv_b = udpClient.EndReceive(r, ref rSock);
            var recv_s = Encoding.ASCII.GetString(recv_b).Split(' ');
      
            float x, y;
            if(recv_s.Length == 2 
                && float.TryParse(recv_s[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x) 
                && float.TryParse(recv_s[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y)) {
                results.Enqueue((x, y));
            }
        } catch(ObjectDisposedException) {
        } finally {
            lock(lockObj) {
                Monitor.PulseAll(lockObj);
            }
        }
    }

    void backgroundUDPProcess() {
        var lSock = new IPEndPoint(IPAddress.Parse(LADDR), LPORT);
        var rSock = new IPEndPoint(IPAddress.Any, 0);
        var udpClient = new UdpClient(lSock);
        while(!stopUdpServ) {
            udpClient.BeginReceive(new AsyncCallback(backgroundUDPCallback), (udpClient, rSock));
            lock(lockObj) {
                Monitor.Wait(lockObj);
            }
        }
        udpClient.Close();
    }

    void Start()
    {
        backgroundUDP = new Thread(backgroundUDPProcess);
        backgroundUDP.Start();
    }

    void Update()
    {
        (float, float) elem;
        while(results.TryDequeue(out elem)) {
            Debug.Log("A x: "+elem.Item1.ToString()+" y: "+elem.Item2.ToString());
        }
    }

    void OnDisable()
    {
        stopUdpServ=true;
        lock(lockObj) {
            Monitor.PulseAll(lockObj);
        }
        backgroundUDP.Join();
    }
}
