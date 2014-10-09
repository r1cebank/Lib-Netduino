using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace ApertureLabs
{
    /// <summary>
    /// Enumeration for Temperature unit
    /// </summary>
    public enum TEMP
    {
        CELSIUS,
        FAHRENHEIT,
        KELVIN

    }
    /// <summary>
    /// Enumerations for response type
    /// </summary>
    public enum RESPONSETYPE
    {
        HTTP100 = 100,
        HTTP101,
        HTTP102,
        HTTP122 = 122,
        HTTP200 = 200,
        HTTP201,
        HTTP202,
        HTTP203,
        HTTP204,
        HTTP205,
        HTTP206,
        HTTP207,
        HTTP226 = 226,
        HTTP300 = 300,
        HTTP301,
        HTTP302,
        HTTP303,
        HTTP304,
        HTTP305,
        HTTP306,
        HTTP307,
        HTTP400 = 400,
        HTTP401,
        HTTP402,
        HTTP403,
        HTTP404,
        HTTP500 = 500,
        HTTP501,
        HTTP502,
        HTTP503,
        HTTP504
    }
    /// <summary>
    /// Event type for ConsoleLog
    /// </summary>
    public enum EVENT
    {
        LOG,
        ERROR,
        TIMEOUT,
        SENSOR,
        NETWORK,
        NETREQ,
        NETRES,
        EXCEPTION,
        APP,
        PROGRAM,
        NONE
    }
    /// <summary>
    /// Enumeration for PATH Builder
    /// </summary>
    public enum PATH
    {
        ROOT,
        BOOT,
        CONFIG,
        DATA,
        LOG,
        PROGRAM,
        RESOURCE,
        TEMP,
        SENSOR,
        APP,
        ERROR
    }
    namespace Netduino
    {
        class LED
        {
            /// <summary>
            /// Port for LED
            /// </summary>
            private OutputPort led;
            /// <summary>
            /// Pin Location of the LED
            /// </summary>
            private Cpu.Pin ledLocation;
            /// <summary>
            /// InitialState of the LED, used for reset
            /// </summary>
            private bool initialState;
            /// <summary>
            /// Thread for LED pattern method
            /// </summary>
            private Thread ledThread;
            /// <summary>
            /// Thread for LED Morse Code thread
            /// </summary>
            private Thread morseThread;
            /// <summary>
            /// Thread for Status LED
            /// </summary>
            private Thread statusThread;

            /// <summary>
            /// Create the LED Pin
            /// </summary>
            /// <param name="ledPin">The pin lication of the LED</param>
            /// <param name="initial">Initial state of the LED</param>
            /// <example>LED(Pins.ONBOARD_LED, false);</example>
            public LED(Cpu.Pin ledPin, bool initial)
            {
                led = new OutputPort(ledPin, initial);
                initialState = initial;
                ledLocation = ledPin;
            }
            /// <summary>
            /// Turn LED on
            /// </summary>
            /// <example>On();</example>
            public void On()
            {
                AbortAll();
                led.Write(true);
            }
            /// <summary>
            /// Turn LED off
            /// </summary>
            /// <example>Off();</example>
            public void Off()
            {
                AbortAll();
                led.Write(false);
            }
            /// <summary>
            /// Write LED State
            /// </summary>
            /// <param name="state">The state of LED to write</param>
            /// <example>write(false);</example>
            public void Write(bool state)
            {
                AbortAll();
                led.Write(state);
            }
            /// <summary>
            /// Reset LED to the initial state
            /// </summary>
            /// <example>reset();</example>
            public void Reset()
            {
                AbortAll();
                led.Write(initialState);
            }
            /// <summary>
            /// Get the pin location of the LED
            /// </summary>
            /// <returns>The pin location</returns>
            public Cpu.Pin GetLocation()
            {
                return ledLocation;
            }
            /// <summary>
            /// Get current LED state
            /// </summary>
            /// <returns>The LED state, TRUE for on, FALSE for off</returns>
            /// <example>bool ledState = get();</example>
            public bool Get()
            {
                return led.Read();
            }
            /// <summary>
            /// Get current LED state
            /// </summary>
            /// <returns>The LED state, TRUE for on, FALSE for off</returns>
            public bool Read()
            {
                return led.Read();
            }
            /// <summary>
            /// Display simple LED blinking pattern
            /// </summary>
            /// <param name="wait">Wait time in millisecond</param>
            /// <param name="iter">Number of iterations</param>
            /// <param name="thread">True for running the sequence in a thread</param>
            /// <example>basicSequence(1000,3);</example>
            /// <remarks>If number of iterations is large, main thread will sleep</remarks>
            public void BasicSequence(int wait, int iter, bool thread)
            {
                AbortAll();
                if (thread)
                {
                    ledThread = new Thread(() => Sequence(wait,iter));
                    ledThread.Start();
                }
                else
                {
                    Sequence(wait, iter);
                }
            }
            /// <summary>
            /// Display status light in a seperate thread
            /// </summary>
            public void StatusLight()
            {
                AbortAll();
                statusThread = new Thread(() => Sequence(1000));
                statusThread.Start();
            }
            /// <summary>
            /// Abort status light on current port
            /// </summary>
            public void StatusAbort()
            {
                if (statusThread != null)
                {
                    if (statusThread.IsAlive)
                    {
                        statusThread.Abort();
                    }
                }
                else
                    ConsoleLog.LogEvent("Status Light is not on.",EVENT.NONE,"");
            }
            public void AbortAll()
            {
                if (statusThread != null)
                {
                    if (statusThread.IsAlive)
                        statusThread.Abort();
                }
                if (ledThread != null)
                {
                    if (ledThread.IsAlive)
                        ledThread.Abort();
                }
                if (morseThread != null)
                {
                    if (morseThread.IsAlive)
                        morseThread.Abort();
                }
            }
            /// <summary>
            /// Display sequence in loop
            /// </summary>
            /// <param name="wait">Wait time in millisecond</param>
            private void Sequence(int wait)
            {
                while(1 == 1)
                {
                    led.Write(!led.Read());
                    Thread.Sleep(wait);
                    led.Write(!led.Read());
                    Thread.Sleep(wait);
                }
            }
            /// <summary>
            /// Basic Sequence for method basicSequence
            /// </summary>
            /// <param name="wait">Wait time in millisecond</param>
            /// <param name="iter">Number of iterations</param>
            private void Sequence(int wait, int iter)
            {
                for (int i = 0; i <= iter; i++)
                {
                    led.Write(!led.Read());
                    Thread.Sleep(wait);
                    led.Write(!led.Read());
                    Thread.Sleep(wait);
                }
            }
            /// <summary>
            /// Send morse code message
            /// </summary>
            /// <param name="message">The message you want to send</param>
            /// <param name="thread">True for running in a seperate thread</param>
            /// <example>moreseMessage("Hello World");</example>
            public void MorseMessage(string message, bool thread)
            {
                AbortAll();
                message = message.ToUpper();
                if (thread)
                {
                    morseThread = new Thread(() => SendMorse(message));
                    morseThread.Start();
                }
                else
                {
                    SendMorse(message);
                }
            }
            /// <summary>
            /// Send morse message, for morseMessage
            /// </summary>
            /// <param name="message">Message in string format</param>
            private void SendMorse(string message)
            {
                for (int i = 0; i < message.Length; i++)
                {
                    char[] messagechr = message.ToCharArray(i, 1);
                    char chr = messagechr[0];
                    MorseChar(chr);
                }
            }
            /// <summary>
            /// This method convert the character and convert them into morse codemessage
            /// </summary>
            /// <param name="message">The message you want to convert</param>
            private void MorseChar(char message)
            {
                int morse_dot = 200;                     // Dot length in milliseconds
                int morse_dash = morse_dot * 3;          // International Morse Code timing defintions ...
                int morse_intraletter = morse_dot;
                int morse_interletter = morse_dot * 3;
                int morse_interword = morse_dot * 4;     // Seven dots minus the interletter distance
                // International Morse Code symbol defintions A-Z0-9[space][full stop] from ASCII values
                uint[][] morse = new uint[255][];
                morse[65] = new uint[] { 0, 1 };
                morse[66] = new uint[] { 1, 0, 0, 0 };
                morse[67] = new uint[] { 1, 0, 1, 0 };
                morse[68] = new uint[] { 1, 0, 0 };
                morse[69] = new uint[] { 0 };
                morse[70] = new uint[] { 0, 0, 1, 0 };
                morse[71] = new uint[] { 1, 1, 0 };
                morse[72] = new uint[] { 0, 0, 0, 0 };
                morse[73] = new uint[] { 0, 0 };
                morse[74] = new uint[] { 0, 1, 1, 1 };
                morse[75] = new uint[] { 1, 0, 1 };
                morse[76] = new uint[] { 0, 1, 0, 0 };
                morse[77] = new uint[] { 1, 1 };
                morse[78] = new uint[] { 1, 0 };
                morse[79] = new uint[] { 1, 1, 1 };
                morse[80] = new uint[] { 0, 1, 1, 0 };
                morse[81] = new uint[] { 1, 1, 0, 1 };
                morse[82] = new uint[] { 0, 1, 0 };
                morse[83] = new uint[] { 0, 0, 0 };
                morse[84] = new uint[] { 1 };
                morse[85] = new uint[] { 0, 0, 1 };
                morse[86] = new uint[] { 0, 0, 0, 1 };
                morse[87] = new uint[] { 0, 1, 1 };
                morse[88] = new uint[] { 1, 0, 0, 1 };
                morse[89] = new uint[] { 1, 0, 1, 1 };
                morse[90] = new uint[] { 1, 1, 0, 0 };
                morse[48] = new uint[] { 1, 1, 1, 1, 1 };
                morse[49] = new uint[] { 0, 1, 1, 1, 1 };
                morse[50] = new uint[] { 0, 0, 1, 1, 1 };
                morse[51] = new uint[] { 0, 0, 0, 1, 1 };
                morse[52] = new uint[] { 0, 0, 0, 0, 1 };
                morse[53] = new uint[] { 0, 0, 0, 0, 0 };
                morse[54] = new uint[] { 1, 0, 0, 0, 0 };
                morse[55] = new uint[] { 1, 1, 0, 0, 0 };
                morse[56] = new uint[] { 1, 1, 1, 0, 0 };
                morse[57] = new uint[] { 1, 1, 1, 1, 0 };
                morse[46] = new uint[] { 0, 1, 0, 1, 0, 1 };
                morse[32] = new uint[] { 0, 0, 0, 0 };
                for (int i = 0; i < morse[message].Length; i++)
                {
                    if (morse[message][i] == 0) // Dot
                    {
                        led.Write(true);
                        Thread.Sleep(morse_dot);
                        led.Write(false);
                        Thread.Sleep(morse_intraletter);
                    }
                    else  // Dash
                    {
                        led.Write(true);
                        Thread.Sleep(morse_dash);
                        led.Write(false);
                        Thread.Sleep(morse_intraletter);
                    }
                }
            }
        }
        class TempSensor
        {
            /// <summary>
            /// Analog Input for sensor reading
            /// </summary>
            private AnalogInput tempSensor;
            /// <summary>
            /// Pin Location of the sensor
            /// </summary>
            private Cpu.Pin sensorLocation;
            /// <summary>
            /// Initialize a tempSensor
            /// </summary>
            /// <param name="pinLocation">The location of the tempSensor</param>
            public TempSensor(Cpu.Pin pinLocation)
            {
                sensorLocation = pinLocation;
                tempSensor = new AnalogInput(sensorLocation);
            }
            /// <summary>
            /// Get the current temperature from the temperature sensor
            /// </summary>
            /// <param name="tempMode">The temperature unit</param>
            /// <returns>temperature in given unit</returns>
            public Double GetTemp(TEMP tempMode)
            {
                Double temp = 0;
                switch (tempMode)
                {
                    case TEMP.CELSIUS:
                        temp = (GetRawTemperature() - 0.5) * 100;
                        break;
                    case TEMP.FAHRENHEIT:
                        temp = ((GetRawTemperature() - 0.5) * 100) * (9 / 5) + 32;
                        break;
                    case TEMP.KELVIN:
                        temp = ((GetRawTemperature() - 0.5) * 100) + 273.15;
                        break;
                }
                return temp;
            }
            /// <summary>
            /// Get raw reading from temperature sensor
            /// </summary>
            /// <returns>raw value from temperature sensor</returns>
            public Double GetRawTemperature()
            {
                return tempSensor.Read() * 3.3 / 1024;
            }
        }
        class LightSensor
        {
            /// <summary>
            /// Ligh sensor
            /// </summary>
            private AnalogInput lightSensor;
            /// <summary>
            /// pin location of the analog pin
            /// </summary>
            private Cpu.Pin pinLocation;
            /// <summary>
            /// Initialize the lightSensor
            /// </summary>
            /// <param name="pin">Pin Location of the sensor, analog input</param>
            public LightSensor(Cpu.Pin pin)
            {
                pinLocation = pin;
                lightSensor = new AnalogInput(pin);
            }
        }
        public static class ByteExtensionMethods
		{
			/// <summary>
			/// Convert byte data from BCD - Binary Coded Decimal - to binary.
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static byte FromBCD(this byte value)
			{
				var lo = value & 0x0f;
				var hi = (value & 0xf0) >> 4;
				var retValue = (byte)(hi * 10 + lo);
				return retValue;
			}

			/// <summary>
			/// Convert byte data from binary to BCD - Binary Coded Decimal
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public static byte ToBCD(this byte value)
			{
				return (byte)((value / 10 << 4) + value % 10);
			}
		}
		public static class TimespanExtensionMethods
		{
			/// <summary>
			/// Compute the total number of milliseconds represented by a TimeSpan.
			/// </summary>
			/// <param name="span"></param>
			/// <returns></returns>
			public static double TotalMilliseconds(this TimeSpan span)
			{
				double num = span.Ticks / TimeSpan.TicksPerMillisecond;

				if (num > double.MaxValue)
					return double.MaxValue;

				if (num < double.MinValue)
					return double.MinValue;
				
				return num;
			}
		}
		/// <summary>
		/// This class interfaces with the DS1307 real time clock chip via the I2C bus.
		/// To wire the chip to a netduino board, wire as follows:
		/// SDA -> Analog Pin 4
		/// SCL -> Analog Pin 5
		/// GND -> GND
		/// 5V  -> 5V
		/// </summary>
		public class DS1307RealTimeClock : IDisposable
		{
			private const int I2CAddress = 0x68;
			private const int I2CTimeout = 1000;
			private const int I2CClockRateKhz = 100;

			public const int UserDataAddress = 8;
			public const int UserDataLength = 56;

			private I2CDevice clock = new I2CDevice(new I2CDevice.Configuration(I2CAddress, I2CClockRateKhz));

			/// <summary>
			/// Set the local .NET time from the RTC board. You can do this on startup then call
			/// DateTime.Now during program execution.
			/// </summary>
			public void SetLocalTimeFromRTC()
			{
				var dt = Now();
				Utility.SetLocalTime(dt);
			}


			/// <summary>
			/// This method sets the real time clock. The current implementation does not take into account control
			/// registers on the DS1307. They can be easily added if needed.
			/// </summary>
			/// <param name="year"></param>
			/// <param name="month"></param>
			/// <param name="day"></param>
			/// <param name="hour"></param>
			/// <param name="minute"></param>
			/// <param name="second"></param>
			/// <param name="dayofWeek"></param>
			/// <returns></returns>
			public int SetClock(byte year, byte month, byte day, byte hour, byte minute, byte second, DayOfWeek dayofWeek)
			{
				// Set the time
				var buffer = new byte[] 
				{
					0x00, // Address
					(byte)second.ToBCD(),
					(byte)minute.ToBCD(),
					(byte)hour.ToBCD(),
					((byte)(dayofWeek +1)).ToBCD(),
					(byte)day.ToBCD(),
					(byte)month.ToBCD(),
					(byte)year.ToBCD()
				};

				var transaction = new I2CDevice.I2CTransaction[] 
				{
					I2CDevice.CreateWriteTransaction(buffer)
				};

				return clock.Execute(transaction, I2CTimeout);
			}

			/// <summary>
			/// Reads data from the DS1307 clock registers and returns it as a .NET DateTime.
			/// </summary>
			/// <returns></returns>
			public DateTime Now()
			{
				var data = new byte[7];
				int result = Read(0, data);

				//TODO: Add exception handling if result == 0
                DateTime dt = DateTime.Now;
                try
                {
                    dt = new DateTime(
                        2000 + data[6].FromBCD(),               // Year
                        data[5].FromBCD(),                      // Month
                        data[4].FromBCD(),                      // Day
                        ((byte)(data[2] & 0x3f)).FromBCD(),     // Hour
                        data[1].FromBCD(),                      // Minute
                        ((byte)(data[0] & 0x7f)).FromBCD()      // Second
                        );
                }
                catch (System.ArgumentOutOfRangeException ex)
                {
                    ConsoleLog.LogEvent("Clock was incorrectly connected.", EVENT.ERROR, "CLOCK");
                    ConsoleLog.LogEvent(ex.ToString(), EVENT.EXCEPTION, "CLOCK");
                }

				return dt;
			}


			/// <summary>
			/// Write data to the clock memory. Normally, this will be used for writing to the user data area.
			/// </summary>
			/// <param name="address"></param>
			/// <param name="data"></param>
			/// <returns></returns>
			public int Write(byte address, byte[] data)
			{
				byte[] buffer = new byte[57];
				buffer[0] = address;
				data.CopyTo(buffer, 1);

				var transaction = new I2CDevice.I2CTransaction[]
				{
					I2CDevice.CreateWriteTransaction(buffer)
				};

				return clock.Execute(transaction, I2CTimeout);
			}

			/// <summary>
			/// Read data from the clock memory. Normally this will be used for reading data from the user memory area.
			/// </summary>
			/// <param name="address"></param>
			/// <param name="data"></param>
			/// <returns></returns>
			public int Read(byte address, byte[] data)
			{
				var transaction = new I2CDevice.I2CTransaction[]
				{
					I2CDevice.CreateWriteTransaction(new byte[] {address}),     // Go to address
					I2CDevice.CreateReadTransaction(data)                       // Read the clock registers
				};

				return clock.Execute(transaction, I2CTimeout);
			}


			#region IDisposable Members
			// The skeleton for this implementaion of IDisposable is taken directly from MSDN.
			// I have left the MSDN comments in place for reference.

			// Track whether Dispose has been called.
			private bool disposed = false;

			// Implement IDisposable.
			public void Dispose()
			{
				Dispose(true);
				// This object will be cleaned up by the Dispose method.
				// Therefore, you should call GC.SupressFinalize to
				// take this object off the finalization queue
				// and prevent finalization code for this object
				// from executing a second time.
				GC.SuppressFinalize(this);
			}

			// Dispose(bool disposing) executes in two distinct scenarios.
			// If disposing equals true, the method has been called directly
			// or indirectly by a user's code. Managed and unmanaged resources
			// can be disposed.
			// If disposing equals false, the method has been called by the
			// runtime from inside the finalizer and you should not reference
			// other objects. Only unmanaged resources can be disposed.
			private void Dispose(bool disposing)
			{
				// Check to see if Dispose has already been called.
				if (!this.disposed)
				{
					// If disposing equals true, dispose all managed
					// and unmanaged resources.
					if (disposing)
					{
						// Dispose managed resources.
						if (clock != null)
							clock.Dispose();
					}

					// Call the appropriate methods to clean up
					// unmanaged resources here.
					// If disposing is false,
					// only the following code is executed.

					/* Empty */

					// Note disposing has been done.
					disposed = true;

				}
			}

			#endregion
		}
        /// <summary>
        /// Console Log Library
        /// </summary>
		class ConsoleLog
        {
            /// <summary>
            /// Log Stream for file logging.
            /// </summary>
            private static StreamWriter log;
            /// <summary>
            /// SD Flag
            /// </summary>
            private static bool SD = false;
            /// <summary>
            /// Clock Flag
            /// </summary>
            private static bool clock = false;
            /// <summary>
            /// Clock Module
            /// </summary>
            private static DS1307RealTimeClock clockModule;
            /// <summary>
            /// Set Devices for logging, 
            /// if this method is not been called, the logger 
            /// will not log and won't keep track of time
            /// </summary>
            /// <param name="isSD">Is SD Connected? (<=2Gb Netduino Plus Only)</param>
            /// <param name="isClock">Is Clock Connected? (DS1307)</param>
            public static void SetDevices(bool isSD, bool isClock)
            {
                SD = isSD;
                clock = isClock;
                if (clock)
                {
                    clockModule = new DS1307RealTimeClock();
                    clockModule.SetLocalTimeFromRTC();
                }
            }
            /// <summary>
            /// Build Event string from user
            /// </summary>
            /// <param name="clockString">Clock Time</param>
            /// <param name="eventString">Event string Specified by the user</param>
            /// <param name="eventType">Event type specified by user</param>
            /// <param name="extra">Extra tags</param>
            /// <returns>Formated Console Event string for logging</returns>
            private static string BuildEventString(string clockString, string eventString, string eventType, string extra)
            {
                string logString = "";
                logString = "[" + extra + "]\n";
                logString = logString + "[" + clockString + "]";
                logString = logString + "["+ eventType +"]: " + eventString;
                logString = logString + "\n" + "[/" + extra + "]\n";
                return logString;
            }
            /// <summary>
            /// Log Event Into File on SD Card
            /// </summary>
            /// <param name="logString">Log string</param>
            /// <param name="dir">Dir Name</param>
            /// <param name="dirPath">Paremt Path of Dir</param>
            /// <param name="fileName">Filename</param>
            /// <param name="filePath">Filepath</param>
            /// <param name="append">If append or overwrite</param>
            private static void LogFiles(string logString, string dir, PATH dirPath, string fileName, PATH filePath, bool append)
            {
                if (!IO.ApertureIO.DirExist(dir, dirPath))
                {
                    IO.ApertureIO.CreateDir(dir, dirPath);
                }
                log = new StreamWriter(IO.PathBuilder.BuildPath(@"\" + fileName, filePath), append);
                log.WriteLine(logString);
                log.Close();
            }
            /// <summary>
            /// Log event to on device SD Card
            /// </summary>
            /// <param name="eventString">The event string</param>
            /// <param name="eventType">The event type</param>
            /// <param name="extra">Extra Data for logging</param>
            public static void LogEvent(string eventString, EVENT eventType, string extra)
            {
                string logString = "";
                string clockString = "";
                clockString = DateTime.Now.ToString();
                switch (eventType)
                {
                    case EVENT.ERROR:
                        logString = BuildEventString(clockString, eventString, "ERROR", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.LOG:
                        logString = BuildEventString(clockString, eventString, "LOG", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.NETREQ:
                        logString = BuildEventString(clockString, eventString, "NETREQ", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.NETRES:
                        logString = BuildEventString(clockString, eventString, "NETRES", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.EXCEPTION:
                        logString = BuildEventString(clockString, eventString, "EXCEPTION", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.NETWORK:
                        logString = BuildEventString(clockString, eventString, "NETWORK", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.SENSOR:
                        logString = BuildEventString(clockString, eventString, "SENSOR", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.TIMEOUT:
                        logString = BuildEventString(clockString, eventString, "TIMEOUT", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.APP:
                        logString = BuildEventString(clockString, eventString, "APP", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    case EVENT.NONE:
                        logString = BuildEventString(clockString, eventString, "INFO", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                    default:
                        logString = BuildEventString(clockString, eventString, "NONE", extra);
                        Debug.Print(logString);
                        Log(logString, eventType);
                        break;
                }
            }
            /// <summary>
            /// Log event into file on SD Card
            /// </summary>
            /// <param name="logString">Log string</param>
            /// <param name="eventType">Event Type</param>
            private static void Log(string logString, EVENT eventType)
            {
                if (SD)
                {
                    switch (eventType)
                    {
                        case EVENT.ERROR:
                            LogFiles(logString, "ERROR", PATH.ROOT, "ERROR.LOG", PATH.ERROR, true);
                            break;
                        case EVENT.LOG:
                            LogFiles(logString, "LOG", PATH.ROOT, "LOG.LOG", PATH.LOG, true);
                            break;
                        case EVENT.NETREQ:
                            LogFiles(logString, "LOG", PATH.ROOT, "NETREQ.LOG", PATH.LOG, true);
                            break;
                        case EVENT.NETRES:
                            LogFiles(logString, "LOG", PATH.ROOT, "NETRES.LOG", PATH.LOG, true);
                            break;
                        case EVENT.EXCEPTION:
                            LogFiles(logString, "ERROR", PATH.ROOT, "EXCEPTION.LOG", PATH.ERROR, true);
                            break;
                        case EVENT.NETWORK:
                            LogFiles(logString, "LOG", PATH.ROOT, "NET.LOG", PATH.LOG, true);
                            break;
                        case EVENT.SENSOR:
                            LogFiles(logString, "SENSOR", PATH.ROOT, "SENSOR.LOG", PATH.SENSOR, true);
                            break;
                        case EVENT.TIMEOUT:
                            LogFiles(logString, "LOG", PATH.ROOT, "TIMEOUT.LOG", PATH.LOG, true);
                            break;
                        case EVENT.NONE:
                            LogFiles(logString, "LOG", PATH.ROOT, "NONE.LOG", PATH.LOG, true);
                            break;
                        case EVENT.APP:
                            LogFiles(logString, "LOG", PATH.ROOT, "APP.LOG", PATH.APP, true);
                            break;
                        default:
                            LogFiles(logString, "TEMP", PATH.ROOT, "TEMP.LOG", PATH.TEMP, true);
                            break;
                    }
                }
            }
        }
        namespace WebServer
        {
			/// <summary>
			/// Delegate for Processing Request
			/// </summary>
			/// <param name="request"></param>
			public delegate string ProcessRequestDelegate(string request);
            /// <summary>
            /// WebServer Class
            /// </summary>
			public class WebServer : IDisposable
			{
				/// <summary>
				/// The thread that handles this webserver
				/// </summary>
				private Thread webServerThread;
				/// <summary>
				/// Creates the socket that will do all the connection
				/// </summary>
				private Socket socket = null;   
				/// <summary>
				/// LED Object that will handle the status light.
				/// </summary>
				private OutputPort led = null;
				/// <summary>
				/// Request string
				/// </summary>
				private string request = "";
				/// <summary>
				/// Response string
				/// </summary>
				private string response = "";
				/// <summary>
				/// Web server constructor, creats necessary objects and outputs the ip address
				/// </summary>
				public WebServer()
				{
					//Initialize Socket class
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					//Request and bind to an IP from DHCP server
					socket.Bind(new IPEndPoint(IPAddress.Any, 80));
					//Debug print our IP address
					ConsoleLog.LogEvent("Current IP Address: " + Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress,EVENT.NETWORK,"NETWORK");
					//Start listen for web requests
					socket.Listen(10);
				}

				/// <summary>
				/// Start the web server
				/// </summary>
				public void StartServer(ProcessRequestDelegate ProcessRequest)
				{
					webServerThread = new Thread(() => ListenForRequest(ProcessRequest));
					webServerThread.Start();
				}

				/// <summary>
				/// Abort current Server thread, will exit with excepetion thrown
				/// </summary>
				public void StopServer()
				{
					webServerThread.Abort();
				}
				/// <summary>
				/// Set status light LED
				/// </summary>
				/// <param name="pinLocation">Pin location of the LED</param>
				public void SetStatusLED(Cpu.Pin pinLocation)
				{
					led = new OutputPort(pinLocation, false);
				}
				/// <summary>
				/// Blink the Status Light
				/// </summary>
				public void BlinkStatusLight()
				{
					if (led != null)
					{
						//Blink the status LED
						led.Write(true);
						Thread.Sleep(150);
						led.Write(false);
					}
				}

				public void ListenForRequest(ProcessRequestDelegate ProcessRequest)
				{
					while (true)
					{
						using (Socket clientSocket = socket.Accept())
						{
							//Get clients IP
							IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
							EndPoint clientEndPoint = clientSocket.RemoteEndPoint;
							//int byteCount = cSocket.Available;
							int bytesReceived = clientSocket.Available;
							if (bytesReceived > 0)
							{
								//Get request
								byte[] buffer = new byte[bytesReceived];
								int byteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
								request = new string(Encoding.UTF8.GetChars(buffer));
								response = ProcessRequest(request);
								//Compose a response
								string header = "HTTP/1.0 200 OK\r\nServer: ApertureLabs/v1.0 (GLaDOS)\r\nContent-Type: text/html; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
								clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
								clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
								BlinkStatusLight();
							}
						}
					}
				}
				#region IDisposable Members
				~WebServer()
				{
					Dispose();
				}
				public void Dispose()
				{
					if (socket != null)
						socket.Close();
				}
				#endregion
			}
        }
		namespace GLaDOS
		{
            /// <summary>
            /// Display Core that uses MicroLiquidCrystal to print characters on LCD
            /// </summary>
            class DisplayCore
            {
                /// <summary>
                /// LCD Provider for declaring LCD
                /// </summary>
                static MicroLiquidCrystal.GpioLcdTransferProvider lcdProvider = null;
                /// <summary>
                /// LCD Instance that handles everything
                /// </summary>
                static MicroLiquidCrystal.Lcd lcd = null;
                /// <summary>
                /// RealTime Clock Module, One Live at a time
                /// </summary>
                static DS1307RealTimeClock clock = null;
                /// <summary>
                /// Battery at analog 0
                /// </summary>
                static AnalogInput battery = null;
                /// <summary>
                /// X Axis of Accelerometer
                /// </summary>
                static AnalogInput X = null;
                /// <summary>
                /// Y Axis of Accelerometer
                /// </summary>
                static AnalogInput Y = null;
                /// <summary>
                /// Z Axis of Accelerometer
                /// </summary>
                static AnalogInput Z = null;
                /// <summary>
                /// Core Temp Module
                /// </summary>
                static TempSensor coreTemp = null;
                /// <summary>
                /// Thread for refreshing Display
                /// </summary>
                static Thread displayRefresh = null;
                /// <summary>
                /// Free memory
                /// </summary>
                static int freeMem = 0;
                /// <summary>
                /// Number of request recieved
                /// </summary>
                static int numReq = 0;
                /// <summary>
                /// Last Request recieved
                /// </summary>
                static string lastReq = "";
                /// <summary>
                /// Last real command executed
                /// </summary>
                static string lastExc = "";
                /// <summary>
                /// Refresh the screen in a Infinite Loop
                /// </summary>
                private static void Refresh()
                {
                    while (true)
                    {
                        PrintGeneralStatus();
                        Thread.Sleep(4000);
                        PrintStatusWTime();
                        Thread.Sleep(4000);
                        PrintNetworkStatus();
                        Thread.Sleep(4000);
                        PrintSensorValue();
                        Thread.Sleep(4000);
                    }
                }
                private static void RefreshMotion()
                {
                    while (true)
                    {
                        printMotionValue();
                        Thread.Sleep(1000);
                    }
                }
                /// <summary>
                /// Start refreshing the screen
                /// </summary>
                /// <param name="thread">Using a thread, will not block current thread</param>
                public static void StartRefresh(bool thread)
                {
                    if (!thread)
                    {
                        Refresh();
                    }
                    else
                    {
                        displayRefresh = new Thread(Refresh);
                        displayRefresh.Start();
                    }
                }
                /// <summary>
                /// Add a request to the Instance
                /// </summary>
                public static void AddReq()
                {
                    numReq++;
                }
                /// <summary>
                /// Set up clock for real time monitoring
                /// </summary>
                public static void SetClock()
                {
                    clock = new DS1307RealTimeClock();
                    clock.SetLocalTimeFromRTC();
                }
                /// <summary>
                /// Set the last executed command
                /// </summary>
                /// <param name="data">Command</param>
                public static void SetLastExc(string data)
                {
                    lastExc = data;
                }
                /// <summary>
                /// Trim the string for LCD Display
                /// </summary>
                /// <param name="data">Data to trim</param>
                /// <param name="length">Length of string needed</param>
                /// <returns>New Trimmed string</returns>
                public static string Trim(string data, int length)
                {
                    if (data.Length > 20)
                    {
                        return data.Substring(0, length);
                    }
                    else
                    {
                        return data;
                    }
                }
                /// <summary>
                /// Set the previous request
                /// </summary>
                /// <param name="req">Request</param>
                public static void SetPrvReq(string req)
                {
                    lastReq = req;
                }
                /// <summary>
                /// Initialize the LCD Display using fewer pins
                /// </summary>
                /// <param name="rs">RS</param>
                /// <param name="en">EN</param>
                /// <param name="D4">D4</param>
                /// <param name="D5">D5</param>
                /// <param name="D6">D6</param>
                /// <param name="D7">D7</param>
                public static void InitDisplayCore(Cpu.Pin rs, Cpu.Pin en, Cpu.Pin D4, Cpu.Pin D5, Cpu.Pin D6, Cpu.Pin D7)
                {
                    coreTemp = new TempSensor(Pins.GPIO_PIN_A0);
                    X = new AnalogInput(Pins.GPIO_PIN_A3);
                    Y = new AnalogInput(Pins.GPIO_PIN_A2);
                    Z = new AnalogInput(Pins.GPIO_PIN_A1);
                    lcdProvider = new MicroLiquidCrystal.GpioLcdTransferProvider(rs, en, D4, D5, D6, D7);
                    lcd = new MicroLiquidCrystal.Lcd(lcdProvider);
                }
                /// <summary>
                /// Set up a LCD using all Pins
                /// </summary>
                /// <param name="rs">RS</param>
                /// <param name="en">EN</param>
                /// <param name="D0">D0</param>
                /// <param name="D1">D1</param>
                /// <param name="D2">D2</param>
                /// <param name="D3">D3</param>
                /// <param name="D4">D4</param>
                /// <param name="D5">D5</param>
                /// <param name="D6">D6</param>
                /// <param name="D7">D7</param>
                public static void InitDisplayCore(Cpu.Pin rs, Cpu.Pin en, Cpu.Pin D0, Cpu.Pin D1, Cpu.Pin D2, Cpu.Pin D3, Cpu.Pin D4, Cpu.Pin D5, Cpu.Pin D6, Cpu.Pin D7)
                {
                    coreTemp = new TempSensor(Pins.GPIO_PIN_A0);
                    X = new AnalogInput(Pins.GPIO_PIN_A3);
                    Y = new AnalogInput(Pins.GPIO_PIN_A2);
                    Z = new AnalogInput(Pins.GPIO_PIN_A1);
                    lcdProvider = new MicroLiquidCrystal.GpioLcdTransferProvider(rs, en, D0, D1, D2, D3, D4, D5, D6, D7);
                    lcd = new MicroLiquidCrystal.Lcd(lcdProvider);
                }
                /// <summary>
                /// Print the general Status on LCD
                /// </summary>
                public static void PrintGeneralStatus()
                {
                    lcd.Begin(20, 4);
                    lcd.Write("#== GLaDOS  V1.0 ==#");
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(Trim("Num Req:" + numReq, 20));
                    lcd.SetCursorPosition(0, 2);
                    lcd.Write(Trim("Prv Req:" + lastReq, 20));
                    lcd.SetCursorPosition(0, 3);
                    lcd.Write("Up Time:" + (Utility.GetMachineTime().Ticks / 10000 / 1000 / 60).ToString() + "mins");
                }
                /// <summary>
                /// Print LCD Network Status on LCD
                /// </summary>
                public static void PrintNetworkStatus()
                {
                    lcd.Begin(20, 4);
                    lcd.Write("#== GLaDOS  V1.0 ==#");
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(Trim("IP:" + Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress.ToString(), 20));
                    lcd.SetCursorPosition(0, 2);
                    lcd.Write(Trim("MAC: 5C864A000821", 20));
                    lcd.SetCursorPosition(0, 3);
                    lcd.Write("Mask:" + Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].SubnetMask.ToString());
                }
                /// <summary>
                /// Print Freemem, Time and LastExc on LCD
                /// </summary>
                public static void PrintStatusWTime()
                {
                    freeMem = (int)Debug.GC(true);
                    if (freeMem < 2000)
                    {
                        lcd.Clear();
                        lcd.Write("#== GLaDOS  V1.0 ==#");
                        lcd.SetCursorPosition(0, 1);
                        lcd.Write("System will reboot");
                        lcd.SetCursorPosition(0, 2);
                        lcd.Write("due to insufficient");
                        lcd.Write("memory");
                        Thread.Sleep(10000);
                        PowerState.RebootDevice(false);
                    }
                    lcd.Begin(20, 4);
                    lcd.Write("#== GLaDOS  V1.0 ==#");
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(Trim("FreeMem:" + freeMem + " bytes", 20));
                    lcd.SetCursorPosition(0, 2);
                    lcd.Write(Trim("LastExc:" + lastExc, 20));
                    lcd.SetCursorPosition(0, 3);
                    lcd.Write(DateTime.Now.ToString());
                }
                /// <summary>
                /// Print the sensor values, core temperature, battery voltage
                /// </summary>
                public static void PrintSensorValue()
                {
                    lcd.Begin(20, 4);
                    lcd.Write("#== GLaDOS  V1.0 ==#");
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(Trim("Voltage:" + "5" + "V", 20));
                    lcd.SetCursorPosition(0, 2);
                    Debug.Print("Core Temp:" + (coreTemp.GetTemp(TEMP.CELSIUS).ToString("F3")));
                    lcd.Write(Trim("Core Temp:" + (coreTemp.GetTemp(TEMP.CELSIUS).ToString("F3")), 20));
                    lcd.SetCursorPosition(0, 3);
                    lcd.Write(Trim("something", 20));
                }
                public static void printMotionValue()
                {
                    lcd.Begin(20, 4);
                    lcd.Write("#== GLaDOS  V1.0 ==#");
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(Trim("X:" + (X.Read()), 20));
                    lcd.SetCursorPosition(0, 2);
                    lcd.Write(Trim("Y:" + (Y.Read()), 20));
                    lcd.SetCursorPosition(0, 3);
                    lcd.Write(Trim("Z:" + (Z.Read()), 20));
                }
                /// <summary>
                /// Print Customized Data on Screen
                /// </summary>
                /// <param name="line1">Line1</param>
                /// <param name="line2">Line2</param>
                /// <param name="line3">Line3</param>
                /// <param name="line4">Line4</param>
                /// <param name="length">Length of screen</param>
                public static void Print(string line1, string line2, string line3, string line4, int length)
                {
                    lcd.Begin(20, 4);
                    lcd.Write(Trim(line1, length));
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(Trim(line2, length));
                    lcd.SetCursorPosition(0, 2);
                    lcd.Write(Trim(line3, length));
                    lcd.SetCursorPosition(0, 3);
                    lcd.Write(Trim(line4, length));
                }
                /// <summary>
                /// Print Customized Data on Screen
                /// </summary>
                /// <param name="line1">Line1</param>
                /// <param name="line2">Line2</param>
                static void Print(string line1, string line2)
                {
                    lcd.Begin(16, 2);
                    lcd.Write(line1);
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write(line2);
                }
            }
            class ResponseCore
            {
                /// <summary>
                /// Password for login
                /// </summary>
                private static string password = "Aperture";
                /// <summary>
                /// If the Response Core is authorized
                /// </summary>
                private static bool authorized = false;
                /// <summary>
                /// Hashtable for request logging
                /// </summary>
                private static Hashtable request = new Hashtable();
                /// <summary>
                /// Current Queue for locating the current queue
                /// </summary>
                private static int currentQueue = 0;
                /// <summary>
                /// System Onboard LED
                /// </summary>
                private static LED led = new LED(Pins.GPIO_PIN_D8,false);
                /// <summary>
                /// Add Request to the queue
                /// </summary>
                /// <param name="req">Request</param>
                /// <returns>Command Execution returns</returns>
                public static string AddRequestStack(string req)
                {
                    request.Add(currentQueue++, req);
                    return executeReq(req);
                }
                /// <summary>
                /// Execute the command.
                /// </summary>
                /// <param name="req">Request send to GLaDOS</param>
                private static string executeReq(string req)
                {
                    if (req == "/")
                        return "";
                    Debug.Print("[DEBUG]" + req + "[DEBUG]");
                    if (req.Split('/')[1] == "AUTH")
                    {
                        if (req.Split('/')[2] == password)
                        {
                            authorized = true;
                            return "[LOGIN SUCCESS]";
                        }
                        else
                            return "[LOGIN FAILED]";
                    }
                    else if (req.Split('/')[1] == "LOGOUT")
                    {
                        authorized = false;
                        return "[LOGOUT SUCCESS]";
                    }
                    else
                    {
                        if (!authorized)
                            return "[NOT AUTHORIZED]";
                        else
                            return executeOtherCommand(req);
                    }
                }
                private static string executeOtherCommand(string req)
                {
                    if (req == "/")
                        return "";
                    if (req.Split('/')[1] == "LED")
                    {
                        if (req.Split('/')[2] == "GET")
                        {
                            return "[LED STATUS: " + led.Read() + "]";
                        }
                        else if (req.Split('/')[2] == "SET")
                        {
                            if (req.Split('/').Length <= 3)
                            {
                                return "[Missing Arguments]";
                            }
                            else
                            {
                                if (req.Split('/')[3] == "TRUE" ||
                                   req.Split('/')[3] == "ON" ||
                                   req.Split('/')[3] == "1")
                                {
                                    led.On();
                                    return "[COMMAND EXECUTED]";
                                }
                                else if (req.Split('/')[3] == "FALSE" ||
                                         req.Split('/')[3] == "OFF" ||
                                         req.Split('/')[3] == "0")
                                {
                                    led.Off();
                                    return "[COMMAND EXECUTED]";
                                }
                                else
                                {
                                    return "[INVALID ARGUMENTs]";
                                }
                            }
                        }
                        else if (req.Split('/')[2] == "MORSE")
                        {
                            if (req.Split('/').Length <= 3)
                            {
                                return "[Missing Arguments]";
                            }
                            else
                            {
                                led.MorseMessage(req.Split(' ')[3], true);
                                return "[COMMAND EXECUTED]";
                            }
                        }
                        else if (req.Split('/')[2] == "STATUSLIGHT")
                        {
                            if (req.Split('/').Length <= 3)
                            {
                                return "[Missing Arguments]";
                            }
                            else
                            {
                                if (req.Split('/')[3] == "ON")
                                {
                                    led.AbortAll();
                                    led.StatusLight();
                                    return "[COMMAND EXECUTED]";
                                }
                                else if (req.Split(' ')[3] == "OFF")
                                {
                                    led.AbortAll();
                                    return "[COMMAND EXECUTED]";
                                }
                                else
                                {
                                    return "[INVALID ARGUMENTs]";
                                }
                            }
                        }
                    }
                    return "";
                }
            }
            class RequestCore
            {
            }
            class SensorCore
            {
            }
            class IOCore
            {
            }
		}
        namespace IO
        {
            class PathBuilder
            {
                /// <summary>
                /// Build the real path on the netduino plus
                /// </summary>
                /// <param name="data">the string data, could be a path or filename</param>
                /// <param name="path">path enum, for easy path build</param>
                /// <returns></returns>
                public static string BuildPath(string data, PATH path)
                {
                    string sdPath = @"SD\";
                    string realPath = "";
                    switch (path)
                    {
                        case PATH.BOOT:
                            realPath = sdPath + "BOOT" + data;
                            break;
                        case PATH.CONFIG:
                            realPath = sdPath + "CONFIG" + data;
                            break;
                        case PATH.DATA:
                            realPath = sdPath + "DATA" + data;
                            break;
                        case PATH.LOG:
                            realPath = sdPath + "LOG" + data;
                            break;
                        case PATH.PROGRAM:
                            realPath = sdPath + "PROGRAM" + data;
                            break;
                        case PATH.RESOURCE:
                            realPath = sdPath + "RESOURCE" + data;
                            break;
                        case PATH.ROOT:
                            realPath = sdPath + data;
                            break;
                        case PATH.TEMP:
                            realPath = sdPath + "TEMP" + data;
                            break;
                        case PATH.ERROR:
                            realPath = sdPath + "ERROR" + data;
                            break;
                        case PATH.SENSOR:
                            realPath = sdPath + "SENSOR" + data;
                            break;
                        case PATH.APP:
                            realPath = sdPath + "APPLOG" + data;
                            break;
                        default:
                            realPath = sdPath + data;
                            break;
                    }
                    return realPath;
                }
            }
            class ApertureIO
            {
                /// <summary>
                /// Dir for directory creating
                /// </summary>
                public static DirectoryInfo dir;
                /// <summary>
                /// Stream for file creation
                /// </summary>
                public static StreamWriter fileStream;
                /// <summary>
                /// Reader for file reading
                /// </summary>
                public static StreamReader reader;
                /// <summary>
                /// SuperCreate Creates a folder set from the string provided
                /// </summary>
                /// <param name="path">Path for creating</param>
                /// <example>SuperCreate(@"Config/TD/Config.inf");</example>
                public static void SuperCreate(string path)
                {
                    SuperCreate(path, false);
                }
                /// <summary>
                /// SuperCreate Creates a folder set from the string provided
                /// </summary>
                /// <param name="data">Path for creating</param>
                /// <param name="lastForFile">True for Last section for file, default is false</param>
                public static void SuperCreate(string data, bool lastForFile)
                {
                    data = PathBuilder.BuildPath(data, PATH.ROOT);
                    string finalPath = "";
                    string[] path = data.Split('/');
                    string[] file;
                    for (int i = 0; i < path.Length; i++)
                    {
                        if (path[i] != "")
                        {
                            if ((i == (path.Length - 1)) && (lastForFile))
                            {
                                finalPath += (path[i]);
                                fileStream = new StreamWriter(finalPath, true);
                                fileStream.Close();
                            }
                            else
                            {
                                file = path[i].Split('.');
                                if (file.Length == 1)
                                {
                                    finalPath += (path[i] + @"\");
                                    dir = Directory.CreateDirectory(finalPath);
                                }
                                else
                                {
                                    finalPath += (path[i]);
                                    fileStream = new StreamWriter(finalPath, true);
                                    fileStream.Close();
                                }
                            }
                        }
                    }
                }
                /// <summary>
                /// Create a directory
                /// </summary>
                /// <param name="dirName">Directory path</param>
                /// <param name="path">Low-Level Path</param>
                public static void CreateDir(string dirName, PATH path)
                {
                    dirName = PathBuilder.BuildPath(dirName, path);
                    dir = Directory.CreateDirectory(dirName);
                }
                /// <summary>
                /// Creating a file
                /// </summary>
                /// <param name="filePath">Path and name of the file</param>
                /// <param name="path">Low-Level Path</param>
                public static void CreateFile(string filePath, PATH path)
                {
                    filePath = PathBuilder.BuildPath(filePath, path);
                    fileStream = new StreamWriter(filePath, true);
                    fileStream.Close();
                }
                /// <summary>
                /// Check if a file exists
                /// </summary>
                /// <param name="filePath">Path of the file</param>
                /// <param name="path">Low-Level Path</param>
                /// <returns>True for exist, false for not exist</returns>
                public static bool FileExist(string filePath, PATH path)
                {
                    filePath = PathBuilder.BuildPath(filePath, path);
                    if (File.Exists(filePath))
                    {
                        return true;
                    }
                    {
                        return false;
                    }
                }
                /// <summary>
                /// Check if a file exists
                /// </summary>
                /// <param name="dirPath">File path</param>
                /// <param name="path">Low-Level Path</param>
                /// <returns>True for exist, false for not exist</returns>
                public static bool DirExist(string dirPath, PATH path)
                {
                    dirPath = PathBuilder.BuildPath(dirPath, path);
                    if (Directory.Exists(dirPath))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                /// <summary>
                /// Read entire file and return in  array
                /// </summary>
                /// <param name="filePath">Path of the file</param>
                /// <param name="path">Low-Level Path</param>
                /// <returns>Entire file in string array</returns>
                /// <remarks>Please do not have "`" in the file</remarks>
                public static string[] ReadFile(string filePath, PATH path)
                {
                    filePath = PathBuilder.BuildPath(filePath, path);
                    reader = new StreamReader(filePath);
                    string line;
                    string lines = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines += (line + "`");
                    }
                    return lines.Split('`');
                }
                /// <summary>
                /// Set the attributes of a file
                /// </summary>
                /// <param name="fileName">filename or path</param>
                /// <param name="path">Low-Level Path</param>
                /// <param name="attr">File Attributes</param>
                public static void SetAttrib(string fileName, PATH path, FileAttributes attr)
                {
                    File.SetAttributes(PathBuilder.BuildPath(fileName, path), attr);
                }
                /// <summary>
                /// Clear the file attribute
                /// </summary>
                /// <param name="filename">Filename or the path</param>
                /// <param name="path">Low-Level Path</param>
                public static void ClearAttrib(string filename, PATH path)
                {
                    File.SetAttributes(PathBuilder.BuildPath(filename, path), FileAttributes.Normal);
                }
            }
        }

    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    sealed class ExtensionAttribute : Attribute
    {
    }
}

//Special Thanks to the folks that developed MicroLiquidCrystal
// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 
namespace MicroLiquidCrystal
{
    public class GpioLcdTransferProvider : ILcdTransferProvider, IDisposable
    {
        private readonly OutputPort _rsPort;
        private readonly OutputPort _rwPort;
        private readonly OutputPort _enablePort;
        private readonly OutputPort[] _dataPorts;
        private readonly bool _fourBitMode;
        private bool _disposed;

        public GpioLcdTransferProvider(Cpu.Pin rs, Cpu.Pin enable, Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
            : this(true, rs, Cpu.Pin.GPIO_NONE, enable, Cpu.Pin.GPIO_NONE, Cpu.Pin.GPIO_NONE, Cpu.Pin.GPIO_NONE, Cpu.Pin.GPIO_NONE, d4, d5, d6, d7)
        { }

        public GpioLcdTransferProvider(Cpu.Pin rs, Cpu.Pin rw, Cpu.Pin enable, Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
            : this(true, rs, rw, enable, Cpu.Pin.GPIO_NONE, Cpu.Pin.GPIO_NONE, Cpu.Pin.GPIO_NONE, Cpu.Pin.GPIO_NONE, d4, d5, d6, d7)
        { }

        public GpioLcdTransferProvider(Cpu.Pin rs, Cpu.Pin enable, Cpu.Pin d0, Cpu.Pin d1, Cpu.Pin d2, Cpu.Pin d3, Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
            : this(false, rs, Cpu.Pin.GPIO_NONE, enable, d0, d1, d2, d3, d4, d5, d6, d7)
        { }

        public GpioLcdTransferProvider(Cpu.Pin rs, Cpu.Pin rw, Cpu.Pin enable, Cpu.Pin d0, Cpu.Pin d1, Cpu.Pin d2, Cpu.Pin d3, Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
            : this(false, rs, rw, enable, d0, d1, d2, d3, d4, d5, d6, d7)
        { }

        /// <summary>
        /// Creates a variable of type LiquidCrystal. The display can be controlled using 4 or 8 data lines. If the former, omit the pin numbers for d0 to d3 and leave those lines unconnected. The RW pin can be tied to ground instead of connected to a pin on the Arduino; if so, omit it from this function's parameters. 
        /// </summary>
        /// <param name="fourBitMode"></param>
        /// <param name="rs">The number of the CPU pin that is connected to the RS (register select) pin on the LCD.</param>
        /// <param name="rw">The number of the CPU pin that is connected to the RW (Read/Write) pin on the LCD (optional).</param>
        /// <param name="enable">the number of the CPU pin that is connected to the enable pin on the LCD.</param>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="d4"></param>
        /// <param name="d5"></param>
        /// <param name="d6"></param>
        /// <param name="d7"></param>
        public GpioLcdTransferProvider(bool fourBitMode, Cpu.Pin rs, Cpu.Pin rw, Cpu.Pin enable, 
                                                 Cpu.Pin d0, Cpu.Pin d1, Cpu.Pin d2, Cpu.Pin d3, 
                                                 Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
        {
            _fourBitMode = fourBitMode;

            if (rs == Cpu.Pin.GPIO_NONE) throw new ArgumentException("rs");
            _rsPort = new OutputPort(rs, false);

            // we can save 1 pin by not using RW. Indicate by passing Cpu.Pin.GPIO_NONE instead of pin#
            if (rw != Cpu.Pin.GPIO_NONE) // (RW is optional)
                _rwPort = new OutputPort(rw, false);

            if (enable == Cpu.Pin.GPIO_NONE) throw new ArgumentException("enable");
            _enablePort = new OutputPort(enable, false);

            var dataPins = new[] { d0, d1, d2, d3, d4, d5, d6, d7};
            _dataPorts = new OutputPort[8];
            for (int i = 0; i < 8; i++)
            {
                if (dataPins[i] != Cpu.Pin.GPIO_NONE)
                    _dataPorts[i] = new OutputPort(dataPins[i], false);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~GpioLcdTransferProvider()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _rsPort.Dispose();
                _rwPort.Dispose();
                _enablePort.Dispose();

                for (int i = 0; i < 8; i++)
                {
                    if (_dataPorts[i] != null)
                        _dataPorts[i].Dispose();
                }
                _disposed = true;
            }
            
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public bool FourBitMode
        {
            get { return _fourBitMode; }
        }

        /// <summary>
        /// Write either command or data, with automatic 4/8-bit selection
        /// </summary>
        /// <param name="value">value to write</param>
        /// <param name="mode">Mode for RS (register select) pin.</param>
        /// <param name="backlight">Backlight state.</param>
        public void Send(byte value, bool mode, bool backlight)
        {
            if (_disposed)
                throw new ObjectDisposedException();

            //TODO: set backlight

            _rsPort.Write(mode);

            // if there is a RW pin indicated, set it low to Write
            if (_rwPort != null)
            {
                _rwPort.Write(false);
            }

            if (!_fourBitMode)
            {
                Write8Bits(value);
            }
            else
            {
                Write4Bits((byte) (value >> 4));
                Write4Bits(value);
            }
        }

        private void Write8Bits(byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                _dataPorts[i].Write(((value >> i) & 0x01) == 0x01);
            }

            PulseEnable();
        }

        private void Write4Bits(byte value)
        {
            for (int i = 0; i < 4; i++)
            {
                _dataPorts[4+i].Write(((value >> i) & 0x01) == 0x01);
            }

            PulseEnable();
        }

        private void PulseEnable()
        {
            _enablePort.Write(false);
            _enablePort.Write(true);  // enable pulse must be >450ns
            _enablePort.Write(false); // commands need > 37us to settle
        }
    }
}
// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 
namespace MicroLiquidCrystal
{
    public interface ILcdTransferProvider
    {
        void Send(byte data, bool mode, bool backlight);

        /// <summary>
        /// Specify if the provider works in 4-bit mode; 8-bit mode is used otherwise.
        /// </summary>
        bool FourBitMode { get; }
    }
}
// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 
namespace MicroLiquidCrystal
{
    public class Lcd
    {
        private static readonly byte[] RowOffsets = new byte[] { 0x00, 0x40, 0x14, 0x54 };

        private readonly ILcdTransferProvider _provider;
        private bool _showCursor;
        private bool _blinkCursor;
        private bool _visible = true;
        private bool _autoScroll;
        private bool _backlight = true;

        private byte _numLines;
        private byte _numColumns;
        private byte _currLine;
        private byte _displayFunction;

        #region LCD Flags
        // ReSharper disable InconsistentNaming

        // commands
        private const byte LCD_CLEARDISPLAY = 0x01;
        private const byte LCD_RETURNHOME = 0x02;
        private const byte LCD_ENTRYMODESET = 0x04;
        private const byte LCD_DISPLAYCONTROL = 0x08;
        private const byte LCD_CURSORSHIFT = 0x10;
        private const byte LCD_FUNCTIONSET = 0x20;
        private const byte LCD_SETCGRAMADDR = 0x40;
        private const byte LCD_SETDDRAMADDR = 0x80;

        // flags for display entry mode
        private const byte LCD_ENTRYRIGHT = 0x00;
        private const byte LCD_ENTRYLEFT = 0x02;
        private const byte LCD_ENTRYSHIFTINCREMENT = 0x01;
        private const byte LCD_ENTRYSHIFTDECREMENT = 0x00;

        // flags for display on/off control
        private const byte LCD_DISPLAYON = 0x04;
        private const byte LCD_DISPLAYOFF = 0x00;
        private const byte LCD_CURSORON = 0x02;
        private const byte LCD_CURSOROFF = 0x00;
        private const byte LCD_BLINKON = 0x01;
        private const byte LCD_BLINKOFF = 0x00;

        // flags for display/cursor shift
        private const byte LCD_DISPLAYMOVE = 0x08;
        private const byte LCD_CURSORMOVE = 0x00;
        private const byte LCD_MOVERIGHT = 0x04;
        private const byte LCD_MOVELEFT = 0x00;

        // flags for function set
        private const byte LCD_4BITMODE  = 0x00;
        private const byte LCD_8BITMODE  = 0x10;
        private const byte LCD_1LINE     = 0x00;
        private const byte LCD_2LINE     = 0x08;
        private const byte LCD_5x8DOTS   = 0x00;
        private const byte LCD_5x10DOTS  = 0x04;

        // ReSharper restore InconsistentNaming
        #endregion

        public Lcd(ILcdTransferProvider provider)
        {
            Encoding = Encoding.UTF8;

            if (provider == null) throw new ArgumentNullException("provider");
            _provider = provider;

            if (_provider.FourBitMode)
                _displayFunction = LCD_4BITMODE | LCD_1LINE | LCD_5x8DOTS;
            else
                _displayFunction = LCD_8BITMODE | LCD_1LINE | LCD_5x8DOTS;

            Begin(16, 1);
        }

        protected ILcdTransferProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Display or hide the LCD cursor: an underscore (line) at the position to which the next character will be written. 
        /// </summary>
        public bool ShowCursor
        {
            get { return _showCursor; }
            set {
                if (_showCursor != value)
                {
                    _showCursor = value;
                    UpdateDisplayControl();
                }
            }
        }

        /// <summary>
        /// Display or hide the blinking block cursor at the position to which the next character will be written.
        /// </summary>
        public bool BlinkCursor
        {
            get { return _blinkCursor; }
            set
            {
                if (_blinkCursor != value)
                {
                    _blinkCursor = value;
                    UpdateDisplayControl();
                }
            }
        }

        /// <summary>
        /// Turns the LCD display on or off. This will restore the text (and cursor) that was on the display. 
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    UpdateDisplayControl();
                }
            }
        }


        /// <summary>
        /// Turns the LCD backlight on or off.
        /// </summary>
        public bool Backlight
        {
            get { return _backlight; }
            set
            {
                if (_backlight != value)
                {
                    _backlight = value;
                    UpdateDisplayControl();
                }
            }
        }

/*       
        /// <summary>
        /// Turns on automatic scrolling of the LCD. This causes each character output to the display to push previous characters 
        /// over by one space. If the current text direction is left-to-right (the default), the display scrolls to the left; 
        /// if the current direction is right-to-left, the display scrolls to the right. 
        /// This has the effect of outputting each new character to the same location on the LCD. 
        /// </summary>
        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                //TODO:
            }
        }*/

        /// <summary>
        /// Get or set the encoding used to map the string into bytes codes that are sent LCD. 
        /// UTF8 is used by default.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Use this method to initialize the LCD. Specifies the dimensions (width and height) of the display. 
        /// </summary>
        /// <param name="columns">The number of columns that the display has.</param>
        /// <param name="lines">The number of rows that the display has.</param>
        public void Begin(byte columns, byte lines)
        {
            Begin(columns, lines, true, false);
        }

        public void Begin(byte columns, byte lines, bool leftToRight, bool dotSize)
        {
            if (lines > 1)
            {
                _displayFunction |= LCD_2LINE;
            }
            _currLine = 0;
            _numLines = lines;
            _numColumns = columns;

            // for some 1 line displays you can select a 10 pixel high font
            if (dotSize && (lines == 1))
            {
                _displayFunction |= LCD_5x10DOTS;
            }

            Thread.Sleep(50);      // LCD controller needs some warm-up time
            // rs, rw, and enable should be low by default

            if (Provider.FourBitMode)
            {
                // this is according to the hitachi HD44780 datasheet
                // figure 24, pg 46

                // we start in 8bit mode, try to set 4 bit mode
                SendCommand(0x03);
                Thread.Sleep(5);   // wait min 4.1ms

                SendCommand(0x03);
                Thread.Sleep(5);    // wait min 4.1ms

                // third go!
                SendCommand(0x03);
                Thread.Sleep(5);

                // finally, set to 4-bit interface
                SendCommand(0x02);
            }
            else
            {
                // this is according to the hitachi HD44780 datasheet
                // page 45 figure 23

                // Send function set command sequence
                SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));
                Thread.Sleep(5); // wait more than 4.1ms

                // second try
                SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));
                Thread.Sleep(1);

                // third go
                SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));
            }

            // finally, set # lines, font size, etc.
            SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));

            // turn the display on with no cursor or blinking default
            _visible = true;
            _showCursor = false;
            _blinkCursor = false;
            _backlight = true;
            UpdateDisplayControl();

            // clear it off
            Clear();

            // set the entry mode
            var displayMode = leftToRight ? LCD_ENTRYLEFT : LCD_ENTRYRIGHT;
            displayMode |= LCD_ENTRYSHIFTDECREMENT;
            SendCommand((byte)(LCD_ENTRYMODESET | displayMode));
        }

        /// <summary>
        /// Clears the LCD screen and positions the cursor in the upper-left corner.
        /// </summary>
        public void Clear()
        {
            SendCommand(LCD_CLEARDISPLAY);
            Thread.Sleep(2); // this command takes a long time!
        }

        /// <summary>
        /// Positions the cursor in the upper-left of the LCD. 
        /// That is, use that location in outputting subsequent text to the display. 
        /// To also clear the display, use the <see cref="Clear"/> method instead. 
        /// </summary>
        public void Home()
        {
            SendCommand(LCD_RETURNHOME);
            Thread.Sleep(2); // this command takes a long time!
        }

        /// <summary>
        /// Position the LCD cursor; that is, set the location at which subsequent text written to the LCD will be displayed
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        public void SetCursorPosition(int column, int row)
        {
            if (row > _numLines)
                row = _numLines - 1;
            
            int address = column + RowOffsets[row];
            SendCommand((byte) (LCD_SETDDRAMADDR | address));
        }

        /// <summary>
        /// Scrolls the contents of the display (text and cursor) one space to the left. 
        /// </summary>
        public void ScrollDisplayLeft()
        {
            //TODO: test
            SendCommand(0x18 | 0x00);
        }

        /// <summary>
        /// Scrolls the contents of the display (text and cursor) one space to the right. 
        /// </summary>
        public void ScrollDisplayRight()
        {
            //TODO: test
            SendCommand(0x18 | 0x04);
        }

        /// <summary>
        /// Moves cursor left or right.
        /// </summary>
        /// <param name="right">true to move cursor right.</param>
        public void MoveCursor(bool right)
        {
            //TODO: verify this instruction
            SendCommand((byte)(0x10 | ((right) ? 0x04 : 0x00)));
        }

        /// <summary>
        /// Writes a text to the LCD.
        /// </summary>
        /// <param name="text">The string to write.</param>
        public void Write(string text)
        {
            byte[] buffer = Encoding.GetBytes(text);
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a specified number of bytes to the LCD using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains data to write to display.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to display.</param>
        /// <param name="count">The number of bytes to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            int len = offset + count;
            for (int i = offset; i < len; i++)
            {
                WriteByte(buffer[i]);
            }
        }

        /// <summary>
        /// Sends one data byte to the display.
        /// </summary>
        /// <param name="data">The data byte to send.</param>
        public void WriteByte(byte data)
        {
            Provider.Send(data, true, _backlight);
        }

        /// <summary>
        /// Sends HD44780 lcd interface command.
        /// </summary>
        /// <param name="data">The byte command to send.</param>
        public void SendCommand(byte data)
        {
            Provider.Send(data, false, _backlight);
        }

        /// <summary>
        /// Create a custom character (gylph) for use on the LCD.
        /// </summary>
        /// <remarks>
        /// Up to eight characters of 5x8 pixels are supported (numbered 0 to 7). 
        /// The appearance of each custom character is specified by an array of eight bytes, one for each row.
        /// The five least significant bits of each byte determine the pixels in that row. 
        /// To display a custom character on the screen, call WriteByte() and pass its number. 
        /// </remarks>
        /// <param name="location">Which character to create (0 to 7) </param>
        /// <param name="charmap">The character's pixel data </param>
        /// <param name="offset">Offset in the charmap wher character data is found </param>
        public void CreateChar(int location, byte[] charmap, int offset)
        {
            location &= 0x7; // we only have 8 locations 0-7
            SendCommand((byte) (LCD_SETCGRAMADDR | (location << 3)));
            for (int i = 0; i < 8; i++)
            {
                WriteByte(charmap[offset+i]);
            }
        }

        public void CreateChar(int location, byte[] charmap)
        {
            CreateChar(location, charmap, 0);
        }

        /// <summary>
        /// Method is called when any of the display control properties are changed.
        /// </summary>
        protected void UpdateDisplayControl()
        {
            int command = LCD_DISPLAYCONTROL;
            command |= (_visible) ? LCD_DISPLAYON : LCD_DISPLAYOFF;
            command |= (_showCursor) ? LCD_CURSORON : LCD_CURSOROFF;
            command |= (_blinkCursor) ? LCD_BLINKON : LCD_BLINKOFF;
            
            //NOTE: backlight is updated with each command
            SendCommand((byte)command); 
        }
    }
}