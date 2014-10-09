using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using ApertureLabs.Netduino;
using ApertureLabs.Netduino.IO;
using ApertureLabs.Netduino.GLaDOS;
using ApertureLabs.Netduino.WebServer;

namespace SLibDuino
{
    public class Program
    {
        public static void Main()
        {
            //// write your code here
            //Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableDhcp();
            //LED onBoard = new LED(Pins.ONBOARD_LED, false);
            //onBoard.StatusLight();
            //ConsoleLog.SetDevices(false, false);
            //DisplayCore.InitDisplayCore(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, Pins.GPIO_PIN_D2, Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D5);
            //DisplayCore.SetClock();
            //DisplayCore.StartRefresh(true);
            //WebServer webserver = new WebServer();
            //webserver.StartServer(new ProcessRequestDelegate(processRequest));
            //Thread.Sleep(Timeout.Infinite);
        }
        //private static string processRequest(string request)
        //{
        //    string response = "";
        //    string firstLine = request.Substring(0, request.IndexOf('\n'));
        //    DisplayCore.SetPrvReq(firstLine.Split(' ')[1]);
        //    DisplayCore.AddReq();
        //    response = ResponseCore.AddRequestStack(firstLine.Split(' ')[1]);
        //    ConsoleLog.LogEvent(firstLine.Split(' ')[1], ApertureLabs.EVENT.NETREQ, "NETWORK");
        //    ConsoleLog.LogEvent(request, ApertureLabs.EVENT.NETWORK, "REQUEST");
        //    ConsoleLog.LogEvent(response, ApertureLabs.EVENT.NETRES, "RESPONSE");
        //    return response;
        //}

    }
}
