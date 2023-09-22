using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Communications
{
    /// <summary>
    /// Author:    Alex Thurgood and Toby Armstrong
    /// Date:      March 30, 2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Toby Armstrong - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// Toby Armstrong and Alex Thurgood, certify that we wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// This project creates the networking and communication code of the solution. It uses a TCP client to establish connects, send and receive data streams. Some
    /// functionality of this project includes being able to start a server on the specified server name and port, wait for incoming messages and send them back 
    /// out to every other connected client after the message passes our networking protocol. Our network is also able to shut down and close the connection to 
    /// every client connected.
    /// 
    /// </summary>
    /// 
    public class Networking
    {

        private ILogger _logger;

        private ReportMessageArrived _messageHandler;

        private ReportDisconnect _disconnectHandler;

        private ReportConnectionEstablished _connectHandler;

        private TcpClient? _tcpClient { get; set; }

        private TcpListener? networkListener;

        private CancellationTokenSource? _waitForCancellation;

        private readonly char _terminationCharacter;

        public string? ID { get; set; }

        public delegate void ReportMessageArrived(Networking channel, string message);
        public delegate void ReportDisconnect(Networking channel);
        public delegate void ReportConnectionEstablished(Networking channel);


        public Networking(ILogger logger, ReportConnectionEstablished onConnect,
            ReportDisconnect onDisconnect,
            ReportMessageArrived onMessage,
            char terminationCharacter)
        {
            _logger = logger;
            _messageHandler = onMessage;
            _disconnectHandler = onDisconnect;
            _connectHandler = onConnect;
            _terminationCharacter = terminationCharacter;
            _tcpClient = new();
        }

        /// <summary>
        /// creates and stores a TCP client object connected to the given host / port
        /// </summary>
        /// <param name="host"> The host to connect to, ex: localhost</param>
        /// <param name="port"> The port to connect to, ex: 11000</param>
        public void Connect(string host, int port)
        {
            try
            {
                if (_tcpClient.Connected)
                    return;
                _tcpClient = new TcpClient(host, port);
                _connectHandler(this);
            }
            catch (Exception e)
            {
                return;
            }

        }

        /// <summary>
        /// waits for new messages, parses them into CheckForMessage method to split up by termination character to then send off.
        /// </summary>
        /// <param name="infinite">The server is always waiting for messages from clients</param>
        public async void AwaitMessagesAsync(bool infinite = true)
        {
            try
            {
                StringBuilder dataBacklog = new StringBuilder();
                byte[] buffer = new byte[4096];
                NetworkStream stream = _tcpClient.GetStream();
                if (stream == null)
                {
                    return;
                }
                while (true)
                {
                    int total = await stream.ReadAsync(buffer);

                    if (total == 0)
                    {
                        _tcpClient.Close();
                        _disconnectHandler(this);
                        return;
                    }

                    string current_data = Encoding.UTF8.GetString(buffer, 0, total);

                    dataBacklog.Append(current_data);

                    Console.WriteLine($"  Received {total} new bytes for a total of {dataBacklog.Length}.");

                    this.CheckForMessage(dataBacklog);

                    dataBacklog.Clear();
                }
            }
            catch (Exception)
            {
                _disconnectHandler(this);
            }

        }

        /// <summary>
        /// private helper method to loop through to look for termination characters
        /// </summary>
        /// <param name="dataBacklog"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CheckForMessage(StringBuilder dataBacklog)
        {
            string allData = dataBacklog.ToString();
            int terminator_position = allData.IndexOf("\n");
            bool foundOneMessage = false;

            while (terminator_position >= 0)
            {
                foundOneMessage = true;

                string message = allData.Substring(0, terminator_position + 1);
                dataBacklog.Remove(0, terminator_position + 1);

                _messageHandler(this, message);

                allData = dataBacklog.ToString();
                terminator_position = allData.IndexOf("\n");
            }

            if (!foundOneMessage)
            {
                Console.WriteLine($"  Message NOT found");
            }
            else
            {
                Console.WriteLine(
                    $"  --------------------------------------------------------------------------------\n" +
                    $"  After Message: {dataBacklog.Length} bytes unprocessed.\n" +
                    $"  --------------------------------------------------------------------------------\n");
            }
        }



        /// <summary>
        /// Waits and handles client connections to a server
        /// </summary>
        /// <param name="port">The port that server is waiting on for clients to join.</param>
        /// <param name="infinite"> If true, infinitely await for clients to connect </param>
        public async void WaitForClients(int port, bool infinite)
        {
            networkListener = new TcpListener(System.Net.IPAddress.Any, port);
            try
            {
                _waitForCancellation = new();
                networkListener.Start();
                while (true)
                {
                    TcpClient connection = await networkListener.AcceptTcpClientAsync(_waitForCancellation.Token);
                    Networking newClient = new(_logger, _connectHandler, _disconnectHandler, _messageHandler, _terminationCharacter);
                    newClient._tcpClient = connection;
                    newClient.ID = newClient._tcpClient.Client.RemoteEndPoint.ToString();

                    _connectHandler(newClient);

                }
            }
            catch (Exception)
            {
                networkListener.Stop();
            }
        }


        /// <summary>
        /// Stops server from waiting for more clients to connect
        /// </summary>
        public void StopWaitingForClients()
        {
            _waitForCancellation.Cancel();
        }

        /// <summary>
        /// Sends the given string to the remote end point
        /// </summary>
        /// <param name="text"> the message to be sent</param>
        public async void Send(string text)
        {
            try
            {
                if (_tcpClient.Connected)
                {
                    if (text.Contains(_terminationCharacter))
                        text = text.Replace(_terminationCharacter, '\r') + _terminationCharacter;

                    byte[] messageBytes = Encoding.UTF8.GetBytes(text);
                    await _tcpClient.GetStream().WriteAsync(messageBytes);
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                _disconnectHandler(this);
            }


        }

        /// <summary>
        /// disconnects the tcp client and networking object
        /// </summary>
        public void Disconnect()
        {
            _tcpClient.Close();
        }
    }


}
