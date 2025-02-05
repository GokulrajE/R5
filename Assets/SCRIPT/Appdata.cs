using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime;
using System.IO;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Random = UnityEngine.Random;
using UnityEditor.Rendering;
using System.Globalization;
using JetBrains.Annotations;
using UnityEditor;


static class JediDataFormat
{
    static public string dformat { get; private set; }
    static public char[] dataTypes { get; private set; }
    static public int dataSize = -1;
    static public string[] dataLabels { get; private set; }
    static public readonly char[] FormatChars = new char[] { 'b', 'i', 'f' };
    static public readonly int[] FormatCharSize = new int[] { 1, 2, 4 };

    static public void ReadSetJediDataFormat(string filename)
    {
        dformat = "";
        // Open the file to read from.
        string[] allformatlines = File.ReadAllLines(filename);
        List<char> _dtypes = new List<char>();
        List<string> _dlabels = new List<string>();
        dataSize = 0;
        foreach (string line in allformatlines)
        {
            if (line.Length > 0)
            {
                // Split line by spaces
                string[] linecomps = line.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (linecomps[0].Length == 1 && Array.Exists(JediDataFormat.FormatChars, chr => chr == linecomps[0][0]))
                {
                    dformat += linecomps[0];
                    _dtypes.Add(linecomps[0][0]);
                    _dlabels.Add(String.Join(" ", linecomps).Substring(2));
                    dataSize += FormatCharSize[Array.IndexOf(FormatChars, linecomps[0][0])];
                }
                else
                {
                    dformat = null;
                    dataTypes = null;
                    dataLabels = null;
                    dataSize = -1;
                    return;
                }
            }
        }
        dataTypes = _dtypes.ToArray();
        dataLabels = _dlabels.ToArray();
        
    }
    
    static public bool IsValidFormatLine(string line)
    {
        
        return false;
    }

    static public float GetFloatValue(int datainx, object dataval)
    {
        
        switch (dataTypes[datainx])
        {
            case 'b':
                return (float)((byte)dataval);
            case 'i':
                return (float)((UInt16)dataval);
            default:
                return (float)dataval;
        }
    }
}

static class JediSerialPayload
{
    static public uint count;
    static public int plSz = 0;
    static public byte status = 0;
    //static public byte[] errorval = new byte[] { 0, 0x00 };
    static public int[] payload = new int[256];
    static public byte[] payloadBytes = new byte[256];
    static public List<object> data = new List<object>();

    static private bool IsFormatStringCorrect(string dataformat)
    {
        foreach (char c in dataformat)
        {
            if (Array.Exists(JediDataFormat.FormatChars, chr => chr == c) == false)
            {
                return false;
            }
        }
        return true;
    }

    static public bool updateData()
    {
        
        if ((plSz - 1 == JediDataFormat.dataSize) )
        {
           // Debug.Log(JediDataFormat.dformat.Length && JediDataFormat.dformat.Length < 16);
            int byteArrayInx = 0;
            data.Clear();
            for (int i = 0; i <2; i++)
            {
                switch (JediDataFormat.dataTypes[i])
                {
                    case 'b':
                        data.Add(JediSerialPayload.payloadBytes[byteArrayInx]);
                        break;
                    case 'i':
                        data.Add(System.BitConverter.ToUInt16(JediSerialPayload.payloadBytes, byteArrayInx));
                        break;
                    case 'f':
                        data.Add(System.BitConverter.ToSingle(JediSerialPayload.payloadBytes, byteArrayInx));
                        break;
                }
                byteArrayInx += JediDataFormat.FormatCharSize[Array.IndexOf(JediDataFormat.FormatChars, JediDataFormat.dataTypes[i])];
            }

            foreach (object o in data)
            {
               
            }
           
            return true;
        }
        else
        {
            Debug.Log("\r no data coming in.");
            return false;
        }
    }


    static public string GetFormatedData(List<object> data)
    {
        string _dstring = AppData.CurrentTime().ToString("G17");
        for (int i = 0; i < data.Count; i++)
        {
            switch (JediDataFormat.dataTypes[i])
            {
                case 'b':
                    _dstring += "," + ((byte)data[i]).ToString();
                    break;
                case 'i':
                    _dstring += "," + ((UInt16)data[i]).ToString();
                    break;
                case 'f':
                    _dstring += "," + ((float)data[i]).ToString("G17");
                    break;
            }

        }
        //_dstring += "\n";
        return _dstring;
    }

    static public string GetFormatedDataWithLabels(List<object> data)
    {
        StringBuilder _strbldr = new StringBuilder();
        _strbldr.AppendLine($"    Time\t: {AppData.CurrentTime()}");
        for (int i = 0; i < data.Count; i++)
        {
            _strbldr.AppendLine($"[{JediDataFormat.dataTypes[i]}] {JediDataFormat.dataLabels[i]}\t: {data[i]}");

        }
        return _strbldr.ToString();
    }
}

public class DataLogger
{
 

    private string _currfilename;
    public string curr_fname
    {
        get { return _currfilename; }
    }
    public StringBuilder _filedata;
    public bool stillLogging
    {
        get { return (_filedata != null); }
    }

    public DataLogger(string filename, string header)
    {
        _currfilename = filename;
        _filedata = new StringBuilder(header);
    }

    public void stopDataLog(bool log = true)
    {
        if (log)
        {
            File.AppendAllText(_currfilename, _filedata.ToString());
        }
        _currfilename = "";
        _filedata = null;
    }

    public void logData(string data)
    {
        if (_filedata != null)
        {
            _filedata.Append(data);
        }
    }
}

public class DataLogger_afterbreak
{


    private string _currfilename1;
    public string curr_fname1
    {
        get { return _currfilename1; }
    }
    public StringBuilder _nextfile;
    public bool stillLogging
    {
        get { return (_nextfile != null); }
    }

    public DataLogger_afterbreak(string filename, string header)
    {
        _currfilename1 = filename;
        _nextfile = new StringBuilder(header);
    }

    public void stopDataLog_afterbreak(bool log = true)
    {
        if (log)
        {
            File.AppendAllText(_currfilename1, _nextfile.ToString());
        }
        _currfilename1 = "";
        _nextfile = null;
    }

    public void logData_afterbreak(string data)
    {
        if (_nextfile != null)
        {
            _nextfile.Append(data);
        }
    }
}
public static class MarsDefs
{
    public static readonly string[] Movements = new string[] { "SFE", "SABDU", "ELFE" };

    public static int getMovementIndex(string Movement)
    {
        return Array.IndexOf(Movements, Movement);
    }
}
static class AppData
{
    static public string comPort = "COM12";
    static public string jdfFilename = "Assets\\jeditextformat.txt";
 
    static public int PrescribedTime = 60; //minutes
    static public double nanosecPerTick = 1.0 / Stopwatch.Frequency;
    static public Stopwatch stp_watch = new Stopwatch();

    
    public static bool IsR5Connected;


    // Arduino related variables
    static public JediSerialCom jediClient;

    // Incoming data format
    static public string dataFormat;

    // JEDI data buffer
    static public JediDataBuffers dataTime;
    static public JediDataBuffers dataBuffers;

    // Data Logger
    static public DataLogger dlogger;
    static public DataLogger_afterbreak afterLogger;

    public static void initializeRobot()
    {
        JediDataFormat.ReadSetJediDataFormat(AppData.jdfFilename);
        jediClient = new JediSerialCom(comPort);
        jediClient.ConnectToArduino();
        
    }
    public static void Quit()
    {
        jediClient.DisconnectArduino();
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Stop play mode if in editor
        #endif

    }
    static public double CurrentTime()
    {
        return stp_watch.ElapsedTicks * nanosecPerTick;

    }
    public static class UserData
    {
        public static bool isExceeded { get; private set; }
        
        public static DataTable dTableSession = null;
        
        public static int MovetimeCurr;
        public static DaySummary[] daySummary;
      
 
        // Function to read all the user data.
        public static void readAllUserData()
        {
            string sessionFilepath = Path.Join(circleclass.circlePath, "Sessions.csv");
           
            dTableSession = DataManager.loadCSV(sessionFilepath);
          
            if (dTableSession != null)
            {
                daySummary = SessionDataHandler.CalculateMoveTimePerDay();
                SessionDataHandler.parseMoveTimecurr();
            }
            
        }
       
    }

}

public class JediSerialCom
{
    static public JediSerialCom jediClient;
    public bool stop;
    public bool pause;
    public SerialPort serPort;
    private Thread reader;
    private uint _count;
    public uint count

    {
        get { return _count; }
    }
    public bool newData;
    public int annotation;

    public JediSerialCom(string port)
    {
        serPort = new SerialPort();
        // Allow the user to set the appropriate properties.
        serPort.PortName = port;
        serPort.BaudRate = 115200;
        serPort.Parity = Parity.None;
        serPort.DataBits = 8;
        serPort.StopBits = StopBits.One;
        serPort.Handshake = Handshake.None;
        serPort.DtrEnable = true;

        // Set the read/write timeouts
        serPort.ReadTimeout = 1500;
        serPort.WriteTimeout = 1500;
        //reader = new Thread(Serialreaderthread);
     
    }
    public void ConnectToArduino()
    {
        if (serPort.IsOpen)
        {
            Debug.LogWarning("Port already open! Closing and reopening...");
            serPort.Close();
            Thread.Sleep(100); // Give the OS time to release the port
        }

        try
        {
            serPort.Open();
            serPort.DiscardInBuffer(); // Clear old data AFTER opening

            Debug.Log("Successfully connected to Arduino.");

            reader = new Thread(Serialreaderthread);
            reader.Start();
        }
        catch (UnauthorizedAccessException)
        {
            Debug.LogError("Access denied! Make sure no other program is using the port.");
        }
        catch (System.IO.IOException)
        {
            Debug.LogError("Port not found. Is the Arduino connected?");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect: " + e.Message);
        }
    }

    //public void ConnectToArduino()
    //{
    //    if (serPort.IsOpen == false)
    //    {
    //        try
    //        {
    //            serPort.Open();
    //            //Debug.Log("Not open");
    //            serPort.DiscardInBuffer();
    //            reader = new Thread(Serialreaderthread);


    //            reader.Start();


    //        }
    //        // catch (Exception e)
    //        catch (Exception e)
    //        {
    //            Debug.Log("NOT CONNECTED"+e.Message);
    //        }


    //       // Debug.Log("open");
    //    }


    //}

    public void DisconnectArduino()
    {
        stop = true;
        reader.Abort();
        serPort.Close();
        
    }

    public void ResetCount()
    {
        _count = 0;
    }

    private void Serialreaderthread()
    {
        byte[] _floatbytes = new byte[4];

        // start stop watch.
        while (stop == false)
        {
           
            // Do nothing if in pause
            if (pause)
            {  
                continue;
            }
            try
            {
                
                // Read full packet.
                if (ReadFullSerialPacket())
                {
                    AppData.IsR5Connected = true;
                    //Debug.Log("working");
                    _count++;
                    JediSerialPayload.updateData();
                    newData = true;
                   
                    // Update data buffers.
                    if (AppData.dataBuffers != null)
                    {

                        AppData.dataTime.Add(new float[] { (float)AppData.CurrentTime() });
                        AppData.dataBuffers.Add(JediSerialPayload.data);
                    }
                  
                    if (AppData.dlogger != null)
                    {
                      
                        string traildata =  R5comm.timer_+JediSerialPayload.GetFormatedData(JediSerialPayload.data)+ "\n";
                        
                        AppData.dlogger.logData(traildata);
                       
                    }
                    else
                    {
                       
                        annotation = 0;
                    }
                    
                }
                else
                {
                    AppData.IsR5Connected = false;
                }
            }
            catch (TimeoutException)
            {
             
                continue;
            }
        }
        serPort.Close();
    }

    // Read a full serial packet.
    private bool ReadFullSerialPacket()
    {
        int chksum = 0;
        int _chksum;
        int rawbyte;
      //  Debug.Log(serPort.ReadByte().ToString());
        // Header bytes
        if ((serPort.ReadByte() == 0xFF) && (serPort.ReadByte() == 0xFF))
        {

            JediSerialPayload.count++;
            chksum = 255 + 255;
            //  No. of bytes
            JediSerialPayload.plSz = serPort.ReadByte();
            chksum += JediSerialPayload.plSz;
          
            // read payload
            for (int i = 0; i < JediSerialPayload.plSz - 1; i++)
            {
                rawbyte = serPort.ReadByte();
                chksum += rawbyte;
                JediSerialPayload.payload[i] = rawbyte;
                JediSerialPayload.payloadBytes[i] = (byte)rawbyte;
            }

           //    De.Log("ping");
            // ensure check sum is correct.
           // Debug.Log(JediSerialPayload.plSz);
            _chksum = serPort.ReadByte();
           // Debug.Log(_chksum == (chksum & 0xFF));
            return (_chksum == (chksum & 0xFF));

        }
        
        
        //    Debug.Log("Wrong Data");
            return false;
        
    }

}

public class JediDataBuffers
{
    static public readonly int maxBufferLength = 1000;
    public int noOfChannels { get; private set; }
    private float[,] _rawData;
    public float[,] rawData
    {
        get { return _rawData; }
    }
    public int head { get; private set; }
    public int tail { get; private set; }
    public int count { get; private set; }
    public string[] channelLabels { get; private set; }

    public JediDataBuffers(int noChannels, string[] chLbls)
    {
        noOfChannels = noChannels;
        _rawData = new float[noChannels, maxBufferLength];
        channelLabels = (string[])chLbls.Clone();
        head = maxBufferLength - 1;
        tail = 0;
        count = 0;
    }

    public void Add(float[] newdata)
    {
        // Update tail and start
        head = (head + 1) % maxBufferLength;
        for (int i = 0; i < newdata.Length; i++)
        {
            _rawData[i, head] = newdata[i];
        }
        if (count == maxBufferLength)
        {
            tail = (tail + 1) % maxBufferLength;
        }
        else
        {
            count++;
        }
    }

    public void Add(List<object> newdata)
    {
        // Update tail and start
        head = (head + 1) % maxBufferLength;
        for (int i = 0; i < newdata.Count; i++)
        {
            _rawData[i, head] = JediDataFormat.GetFloatValue(i, newdata[i]);
        }
        if (count == maxBufferLength)
        {
            tail = (tail + 1) % maxBufferLength;
        }
        else
        {
            count++;
        }

    }
  
}






