/// <summary>
/// Author:    Alex Thurgood and Toby Armstrong
/// Date:      March 30, 2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Toby Armstrong - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Toby Armstrong and Alex Thurgood, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// Methods that update the ChatServer GUI. Some of these methods include, starting the server and waiting for clients to join,
/// grabbing and sending the message from the client over to the server,
/// displaying messages that each client has sent in the server chat log, displaying whether clients have connected or disconnected from the server. 
/// The server can also give a list of the ID's from all other connected clients if a client presses the "retrieve participants" button.
/// </summary>

using System.Text;
using Communications;
using Microsoft.Extensions.Logging;

namespace ChatServer;

public partial class MainPage : ContentPage
{
    private readonly char _terminationCharacter = '\n';

    private readonly ILogger<MainPage> _logger;

    private Networking serverNetwork;

    private List<Networking> _clients;

    public MainPage(ILogger<MainPage> logger)
    {
        serverNetwork = new Networking(_logger, ServerConnectHandler, ServerDisconnectHandler, ServerMessageHandler, '\n');
        _clients = new List<Networking>();
        _logger = logger;
        InitializeComponent();
        _logger.LogInformation("Main Page Constructor");

        StartServerBtn.Clicked += StartAndStopServer;
    }

    /// <summary>
    ///  event handler that starts the server and waits for clients to connect
    /// </summary>
    /// <param name="sender">the current client</param>
    /// <param name="e"> The event handler for the start server button</param>
    private void StartAndStopServer(object sender, EventArgs e)
    {
        if (StartServerBtn.Text == "Start Server")
        {
            Dispatcher.Dispatch(() => chatLog.Text = "");
            Dispatcher.Dispatch(() => StartServerBtn.Text = "Shutdown Server");
            Dispatcher.Dispatch(() => chatLog.Text += "Setting up server...\n");
            serverNetwork.WaitForClients(11000, true);
            Dispatcher.Dispatch(() => chatLog.Text += $"{nameEntry.Text} server started!\n");
        }
        else
        {
            foreach (Networking client in _clients)
            {
                client.Disconnect();
            }
            Dispatcher.Dispatch(() => StartServerBtn.Text = "Start Server");
            serverNetwork.StopWaitingForClients();
            Dispatcher.Dispatch(() => chatLog.Text = "");
        }
    }

    /// <summary>
    ///  displays the message from the client
    /// </summary>
    /// <param name="channel"> the client that the message is coming from</param>
    /// <param name="message"> the message that was sent</param>
    private void ServerMessageHandler(Networking channel, string message)
    {
        // condition if the command message is "Command Name"

        if (message.StartsWith("Command Name"))
        {
            //Update channel ID
            int startIndex = message.IndexOf("[") + 1;
            channel.ID = message.Substring(startIndex, message.IndexOf("]") - startIndex);

            //Update server GUI
            Dispatcher.Dispatch(() => participantLabel.Text += channel.ID + "\n");
            Dispatcher.Dispatch(() => chatLog.Text += channel.ID + " connected to the server!" + "\n");
            return;
        }
        // condition if the command message is "Command Participants"
        else if (message.StartsWith("Command Participants"))
        {
            StringBuilder commandMessage = new("Command Participants");
            foreach (Networking client in _clients)
            {
                commandMessage.Append($",[{client.ID}]");
            }
            commandMessage.Append(_terminationCharacter);
            channel.Send(commandMessage.ToString());
        }
        // if the message is a 'normal' message
        else
        {
            Dispatcher.Dispatch(() => chatLog.Text += $"{channel.ID} - {message}");
        }

        List<Networking> toRemove = new();
        List<Networking> toSendTo = new();

        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        // this adds every connected client to a list
        lock (_clients)
        {
            foreach (var client in _clients)
            {
                toSendTo.Add(client);
            }
        }

       _logger.LogDebug($"  Sending a message of size ({message.Length}) to {toSendTo.Count} clients");

        // this sends the message, properly formatted, out to every client
        foreach (Networking client in toSendTo)
        {
            try
            {
                message = message.Trim() + _terminationCharacter;
                client.Send($"{channel.ID} - {message}");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"    Client Disconnected: {client.ID} - {ex.Message}");
                toRemove.Add(client);
            }
        }

        // this send the disconnect message out and removes said client
        lock (_clients)
        {
            foreach (Networking client in toRemove)
            {
                channel.Send($"{client.ID} disconnected from the server!");
                _clients.Remove(client);
            }
        }

        toSendTo.Clear();
        toRemove.Clear();
    }

    /// <summary>
    ///  connects the given client to the server, adds the client to a list of clients, and displays a message about the connection
    /// </summary>
    /// <param name="channel"> the client that is connecting to the server</param>
    private void ServerConnectHandler(Networking channel)
    {
        channel.AwaitMessagesAsync();
        _clients.Add(channel);
    }

    /// <summary>
    /// disconnects the client from the server 
    /// </summary>
    /// <param name="channel">the client to disconnect</param>
    private void ServerDisconnectHandler(Networking channel)
    {
        Dispatcher.Dispatch(() => participantLabel.Text = "");
        lock (_clients)
        {
            _clients.Remove(channel);

            foreach (Networking client in _clients)
            {
                Dispatcher.Dispatch(() => participantLabel.Text += client.ID + "\n");
                client.Send($"{channel.ID} disconnected from the server!\n");
            }
        }
        Dispatcher.Dispatch(() => chatLog.Text += $"{channel.ID} disconnected from the server!\n");
    }
}


