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
/// Methods that update the ChatClient GUI. Some of these methods include, grabbing and sending the message from the client over to the server, 
/// displaying messages that the client sent in the client chat-log, displaying whether they have connected or disconnected from the server. 
/// The client can also send command messages that can update the current clients name and can display a list of connected clients 
/// </summary>

using System.Text;
using Communications;
using Microsoft.Extensions.Logging;

namespace ChatClient;

public partial class MainPage : ContentPage
{
    private readonly char _terminationCharacter = '\n';

    private readonly ILogger<MainPage> _logger;

    private Networking clientNetwork;

    public MainPage(ILogger<MainPage> logger)
    {
        clientNetwork = new Networking(_logger, ClientConnectHandler, ClientDisconnectHandler, ClientMessageHandler, _terminationCharacter);

        _logger = logger;
        InitializeComponent();

        serverConnectionButton.Clicked += ConnectOrDisconnect;
        participantButton.Clicked += RetrieveParticipants;
        entryEntry.Completed += sendMessage;
        _logger.LogInformation("Main Page Constructor");
    }

    /// <summary>
    /// Event handler for clicking the "Retrieve Participants" button to request participants
    /// </summary>
    /// <param name="sender">the server that this message is going to</param>
    /// <param name="e">Event handler for clicking the Retrieve Participants button</param>
    private void RetrieveParticipants(object sender, EventArgs e)
    {
        clientNetwork.Send($"Command Participants{_terminationCharacter}");
    }

    /// <summary>
    /// grabs the text from the "say something" label and sends it from the client over to the server 
    /// </summary>
    /// <param name="sender">the current client</param>
    /// <param name="e">the event handlet to send a message</param>
    private void sendMessage(object sender, EventArgs e)
    {
        clientNetwork.Send(entryEntry.Text + _terminationCharacter);
    }


    /// <summary>
    /// Event handler for clicking the connect to server button
    /// </summary>
    /// <param name="sender">the current client</param>
    /// <param name="e">the event handler for the connect to server button</param>
    private void ConnectOrDisconnect(object sender, EventArgs e)
    {
        if (serverConnectionButton.Text == "Connect To Server" && locationEntry.Text.Length != 0)
        {

            string host = locationEntry.Text;
            clientNetwork.Connect(host, 11000);
            if (nameEntry.Text != null)
                clientNetwork.Send($"Command Name [{nameEntry.Text}]{_terminationCharacter}");
            else
                clientNetwork.Send($"Command Name [Anonymous User]{_terminationCharacter}");
            clientNetwork.AwaitMessagesAsync();
            Dispatcher.Dispatch(() => serverConnectionButton.Text = "Disconnect From Server");
        }
        else
        {
            clientNetwork.Disconnect();
        }
    }

    /// <summary>
    /// displays the message the client sent on the client chatlog
    /// </summary>
    /// <param name="channel">the client that is sending the message </param>
    /// <param name="message">the message that was sent</param>
    private void ClientMessageHandler(Networking channel, string message)
    {
        if (this.HandleMessage(message, out string line))
        {
            Dispatcher.Dispatch(() => chatLog.Text += line);
        }
        else
        {
            if (line != "")
                Dispatcher.Dispatch(() => participantLabel.Text = line);
        }
    }

    /// <summary>
    /// connecting a client to the server and notify the client side that they connected
    /// </summary>
    /// <param name="channel"> the client that is connecting</param>
    private void ClientConnectHandler(Networking channel)
    {
        if (nameEntry.Text != null)
        {
            channel.ID = nameEntry.Text;
            Dispatcher.Dispatch(() => chatLog.Text += channel.ID + " connected to the server!\n");
            _logger.LogDebug($"{channel.ID} is now connected");
        }
        else
            Dispatcher.Dispatch(() => chatLog.Text += "Anonymous User connected to the server!\n");
        channel.AwaitMessagesAsync();

    }

    /// <summary>
    ///  disconnects the current client from the server and displays a message that the disconnected
    /// </summary>
    /// <param name="channel"> the client to be disconnected</param>
    private void ClientDisconnectHandler(Networking channel)
    {
        Dispatcher.Dispatch(() => serverConnectionButton.Text = "Connect To Server");
        Dispatcher.Dispatch(() => entryEntry.Text = "");
        Dispatcher.Dispatch(() => nameEntry.Text = "");
        Dispatcher.Dispatch(() => locationEntry.Text = "");
        Dispatcher.Dispatch(() => participantLabel.Text = "");
        Dispatcher.Dispatch(() => chatLog.Text = "Disconnected from server, Goodbye!\n");
    }

    /// <summary>
    /// Determines whether the incoming message is a command message or a normal message. 
    /// There are different protocols depending on which case it is.
    /// </summary>
    /// <param name="message"> The incoming message</param>
    /// <param name="toShow"> What message is outputted depending on the condition</param>
    /// <returns> returns true if the message is a 'normal' message, false otherwise</returns>
    public bool HandleMessage(string message, out string toShow)
    {
        if (message.StartsWith("Command Participants"))
        {
            toShow = FormatParticipants(message.Substring(message.IndexOf("[")));
            return false;
        }
        else if ((message.StartsWith("Command Name")))
        {
            toShow = "";
            return false;
        }
        else
        {
            toShow = FormatMessage(message);
            return true;
        }
    }

    /// <summary>
    /// If the incoming input is a normal message, it just returns the message
    /// </summary>
    /// <param name="input"> the inputted message </param>
    /// <returns> the formatted message</returns>
    private string FormatMessage(string input)
    {
        return input;
    }

    /// <summary>
    /// If the inputted message starts with "Command Participants", this method gets called, 
    /// it loops over the names of the participants and returns them in a string builder.
    /// </summary>
    /// <param name="input"> the inputted message</param>
    /// <returns>the string builder objects ToString method</returns>
    private string FormatParticipants(string input)
    {
        string[] names = input.Split(',');
        StringBuilder sb = new();
        foreach (string name in names)
        {
            int endOfNameIndex = name.IndexOf("]");
            sb.Append(name.Substring(1, endOfNameIndex - 1) + '\n');
            _logger.LogDebug($"The connected clients are{ sb.ToString()}");
        }
        return sb.ToString();
    }
}

