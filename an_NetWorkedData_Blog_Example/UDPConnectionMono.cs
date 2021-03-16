using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

using NetWorkedData;
using UnityEngine.UI;
using NWEMiniJSON;

[Serializable]
public class DEMOPlayer : NWDPlayer
{
    public string Demo;
}
[Serializable]
public class DEMOArea : NWDArea
{
    public string Demo;
}
[Serializable]
public class DEMOAreaObject : NWDAreaObject
{
    public string Demo;
}
[Serializable]
public class DEMOArenaBattle : NWDArenaBattle<DEMOArea, DEMOPlayer, DEMOAreaObject>
{
    private NWDSplashscreenController SplashscreenController;
    public void SetSplashscreenController(NWDSplashscreenController sSplashscreenController)
    {
        SplashscreenController = sSplashscreenController;
    }
    public override void DefineServer()
    {
        IP = "37.187.114.161";
        PortOut = 7777;
        PortIn = 7777;
    }
    public override void ReceivedFromServer(string sMessage, long tTimeRequest)
    {
        string tReturn = "receiving data from UDP (" + tTimeRequest.ToString() + " ms) : " + sMessage;
        if (SplashscreenController != null)
        {
            UnityEngine.Debug.Log(tReturn);
            SplashscreenController.LastSyncResultLog = tReturn;
        }
    }
    public override void Closed()
    {
    }
}

public class UDPConnectionMono : MonoBehaviour
{
    DEMOArenaBattle BattleData = new DEMOArenaBattle();
    public NWDSplashscreenController SplashscreenController;
    public Toggle CommunToggle;
    public Toggle CommunToggleAutoUpdate;
    private DEMOAreaObject CommunToggleData;
    int Counter;
    void Start()
    {
        if (BattleData != null)
        {
            BattleData.SetSplashscreenController(SplashscreenController);
            BattleData.Ref = "123456789";
            BattleData.Area.Ref = "6454567aze-erea";
        }
        CommunToggleData = new DEMOAreaObject();
        CommunToggleData.Ref = "TTT141414";
    }
    private void OnApplicationPause(bool pause)
    {
        if (BattleData != null)
        {
            BattleData.CloseServer();
        }
    }
    private void OnApplicationQuit()
    {
        if (BattleData != null)
        {
            BattleData.CloseServer();
        }
    }
    public void ToggleAction()
    {
        UnityEngine.Debug.Log("ToggleAction()");
        if (CommunToggle.isOn == true)
        {
            CommunToggleData.Demo = "yes";
        }
        else
        {
            CommunToggleData.Demo = "no";
        }
        if (BattleData.ChangedObjects.Contains(CommunToggleData) == false)
        {
            BattleData.ChangedObjects.Add(CommunToggleData);
        }
        if (CommunToggleAutoUpdate.isOn == false)
        {
            SendUDPMessage();
        }
    }
    public void SendUDPMessage()
    {
        if (BattleData != null)
        {
            BattleData.Player.Demo = "Hello " + (Counter++).ToString();
            BattleData.SendToServer();
        }
    }
    private void Update()
    {
        if (CommunToggleAutoUpdate != null)
        {
            if (CommunToggleAutoUpdate.isOn)
            {
                SendUDPMessage();
            }
        }
        //UnityEngine.Debug.Log("analyze "+ BattleData.AllObjects.Count+ "object(s) ");
        foreach (DEMOAreaObject tObject in BattleData.AllObjects)
        {
            //UnityEngine.Debug.Log("analyze object " + tObject.Ref + " with value "+ tObject.Demo);
            if (CommunToggleData != null)
            {
                if (tObject.Ref == CommunToggleData.Ref)
                {
                    bool tWant = (tObject.Demo == "yes");
                    if (tWant != CommunToggle.isOn)
                    {
                        Toggle.ToggleEvent tOriginalEvent = CommunToggle.onValueChanged;
                        CommunToggle.onValueChanged = new Toggle.ToggleEvent();
                        if (tObject.Demo == "yes")
                        {
                            CommunToggle.isOn = true;
                        }
                        else
                        {
                            CommunToggle.isOn = false;
                        }
                        CommunToggle.onValueChanged = tOriginalEvent;
                    }
                }
            }
        }
    }
}






[Serializable]
public enum NWDPlayerStatut : int
{
    NotInArea = 0,

    Ready = 1,
    Pause = 2,
    Dead = 3,

    WillQuit = 9,
}
[Serializable]
public enum NWDAreaStatut : int
{
    None = 0,
}
public enum NWDArenaStatut : int
{
    WaitingPlayers,
    CountDownFive,
    CountDownFour,
    CountDownThree,
    CountDownTwo,
    CountDownOne,
    Go,
    Finish,
    LootMode,
    Close,
}
[Serializable]
public class NWDPlayer
{
    public string Ref;
    public NWDPlayerStatut Statut;
}
[Serializable]
public class NWDArea
{
    public string Ref;
    public NWDAreaStatut Statut = NWDAreaStatut.None;
}
[Serializable]
public class NWDAreaObject
{
    public string Ref;
}
[Serializable]
public class NWDArena
{
    protected const string K_CLOSED_ON_SERVER = "closed";

    protected string IP = "";
    protected int PortOut = 123;
    protected int PortIn = 321;

    public string Ref;
    public int Frequency;
    public int CountLocal;
    public int CountServer;

    public NWDArenaStatut Statut = NWDArenaStatut.WaitingPlayers;
    public int CountDown = 10;

    public virtual void DefineServer()
    {
        IP = "37.187.114.161";
        PortOut = 7777;
        PortIn = 7777;
    }
    public virtual void ReceivedFromServer(string sMessage, long tTimeRequest)
    {
    }
    public virtual void Closed()
    {
    }
}
public delegate void NWDArenaBattleDelegate(string sPayload, long tTimeRequest);
[Serializable]
public class NWDArenaBattle<A, P, O> : NWDArena where A : NWDArea, new() where P : NWDPlayer, new() where O : NWDAreaObject, new()
{
    [Serializable]
    private class PlayerSend
    {
        public string R;
        public NWDPlayerStatut S;
        public string W;
    }
    [Serializable]
    private class AreaSend
    {
        public string R;
        public NWDAreaStatut S = NWDAreaStatut.None;
        public string W;
    }
    [Serializable]
    private class AreaObjectSend
    {
        public string R;
        public string W;
    }
    [Serializable]
    private class BattleSend
    {
        public string R; // battle reference
        public NWDArenaStatut S = NWDArenaStatut.WaitingPlayers;
        public PlayerSend P = new PlayerSend(); //Player which send
        public AreaSend A = new AreaSend(); // Area object
        public List<string> C = new List<string>(); // full by all Object changed
        public List<string> E = new List<string>(); // empty when send, full by all players when return
        public List<string> O = new List<string>(); // empty when send, full by all Object when return
        public int I; // Server Count
        public int Y; // Client Count
        public string D; // Server Log
    }
    private BattleSend ToSend = new BattleSend();
    public P Player = new P();
    public List<P> AllPlayers = new List<P>();
    public A Area = new A();
    public List<O> ChangedObjects = new List<O>();
    public List<O> AllObjects = new List<O>();
    private UdpConnection UDPClient;
    public void SendToServer()
    {
        if (UDPClient == null)
        {
            DefineServer();
            UDPClient = new UdpConnection();
        }
        CountLocal++;
        UDPClient.StartConnection(IP, PortOut, PortIn);
        Player.Ref = NWDAccount.CurrentReference();
        ToSend.R = Ref;
        ToSend.E.Clear();
        ToSend.O.Clear();
        ToSend.C.Clear();
        ToSend.S = Statut;
        ToSend.Y = CountLocal;
        ToSend.P.R = Player.Ref;
        ToSend.P.S = Player.Statut;
        ToSend.P.W = JsonUtility.ToJson(Player);
        foreach (O tObjectChanged in ChangedObjects)
        {
            AreaObjectSend tObject = new AreaObjectSend();
            tObject.R = tObjectChanged.Ref;
            tObject.W = JsonUtility.ToJson(tObjectChanged);
            ToSend.C.Add(JsonUtility.ToJson(tObject));
        }
        ToSend.A.R = Area.Ref;
        ToSend.A.S = Area.Statut;
        ToSend.A.W = JsonUtility.ToJson(Area);
        // ready to send
        string tJSON = JsonUtility.ToJson(ToSend);
        ChangedObjects.Clear();
        UDPClient.Send(tJSON, delegate (string sPayload, long tTimeRequest)
        {
            if (sPayload != K_CLOSED_ON_SERVER)
            {
                JsonUtility.FromJsonOverwrite(sPayload, ToSend);
                if (CountServer < ToSend.I)
                {
                    if (CountLocal == ToSend.Y)
                    {
                        Statut = ToSend.S;
                        CountServer = ToSend.I;

                        A tA = new A();
                        JsonUtility.FromJsonOverwrite(ToSend.A.W, tA);

                        AllPlayers.Clear();
                        foreach (string tPW in ToSend.E)
                        {
                            //UnityEngine.Debug.Log("tPW = " + tPW);
                            PlayerSend tOr = new PlayerSend();
                            JsonUtility.FromJsonOverwrite(tPW, tOr);
                            P tP = new P();
                            JsonUtility.FromJsonOverwrite(tOr.W, tP);
                            AllPlayers.Add(tP);
                        }

                        AllObjects.Clear();
                        foreach (string tOW in ToSend.O)
                        {
                            //UnityEngine.Debug.Log("tOW = " + tOW);
                            AreaObjectSend tOr = new AreaObjectSend();
                            JsonUtility.FromJsonOverwrite(tOW, tOr);
                            O tO = new O();
                            JsonUtility.FromJsonOverwrite(tOr.W, tO);
                            AllObjects.Add(tO);
                        }

                        ReceivedFromServer(sPayload, tTimeRequest);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Not response of my demand");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Old reponse of server");
                }
            }
            else
            {
                Closed();
            }
        });
    }
    public void CloseServer()
    {
        if (UDPClient != null)
        {
            UDPClient.Stop();
        }
    }
}

public class UdpConnection
{
    private UdpClient udpClient;
    Stopwatch MyStopWatch;
    Thread receiveThread;
    long SendMessageTimestamp;
    private bool threadRunning = false;
    private string senderIp;
    private int senderPort;
    public NWDArenaBattleDelegate Delegation;
    bool started = false;
    public void StartConnection(string sendIp, int sendPort, int receivePort)
    {
        if (started == false)
        {
            MyStopWatch = new Stopwatch();
            MyStopWatch.Start();
            try
            {
                udpClient = new UdpClient(receivePort);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Failed to listen for UDP at port " + receivePort + ": " + e.Message);
                return;
            }
            started = true;
            UnityEngine.Debug.Log("Created receiving client at ip  and port " + receivePort);
            this.senderIp = sendIp;
            this.senderPort = sendPort;
            UnityEngine.Debug.Log("Set sendee at ip " + sendIp + " and port " + sendPort);
            StartReceiveThread();
        }
    }

    private void StartReceiveThread()
    {
        receiveThread = new Thread(() => ListenForMessages(udpClient));
        receiveThread.IsBackground = true;
        threadRunning = true;
        receiveThread.Start();
    }

    private void ListenForMessages(UdpClient client)
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (threadRunning)
        {
            try
            {
                Byte[] receiveBytes = client.Receive(ref remoteIpEndPoint); // Blocks until a message returns on this socket from a remote host.
                string returnData = Encoding.UTF8.GetString(receiveBytes);
                long tResult = MyStopWatch.ElapsedMilliseconds - SendMessageTimestamp;
                Delegation(returnData, tResult);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10004) UnityEngine.Debug.Log("Socket exception while receiving data from udp client: " + e.Message);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Error receiving data from udp client: " + e.Message);
            }
            Thread.Sleep(1);
        }
    }

    public void Send(string message, NWDArenaBattleDelegate sDelegateion)
    {
        if (started == true)
        {
            Delegation = sDelegateion;
            SendMessageTimestamp = MyStopWatch.ElapsedMilliseconds;
            UnityEngine.Debug.Log(String.Format("Send msg to ip:{0} port:{1} msg:{2}", senderIp, senderPort, message));
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
            Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(sendBytes, sendBytes.Length, serverEndpoint);
        }
    }

    public void Stop()
    {
        if (started == true)
        {
            started = false;
            threadRunning = false;
            receiveThread.Abort();
            udpClient.Close();
            MyStopWatch.Stop();
        }
    }
}