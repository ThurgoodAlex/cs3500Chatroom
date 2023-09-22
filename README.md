```
Author:     Tobias R. Armstrong and Alex Thurgood
Start Date: 26-March-2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  tobybobarm, ThurgoodAlex
Repo:       https://github.com/uofu-cs3500-spring23/assignment-seven---chatting-ms_mongoose
Commit Date: 6th-April-2023 11:00pm
Solution:   Networking_and_Logging
Copyright:  CS 3500, Tobias R. Armstrong and Alex Thurgood - This work may not be copied for use in Academic Coursework.
``
#Overview of Functionality
    
    This solution creates a chat program, a Networking library and a file logger class. The chat program creates a Server that allows for multiple connected clients. This allows the clients to talk to each other over the server and have the server display the messages back to the clients. The networking class hold the logic that creates this connection. The GUI's react properly whether a client connects or disconnects, if the server shutdowns or if the client requests a list of participants. 

    One design implementation we decided to change is the default ID. The API tells us that the default ID is the clients IP address but we decided that it was not smart to display the IP address to the entire chat room. We decided to change that to "Anonymous User" instead.

    We were having problems trying to implement the file logger class. How we were trying to implement it broke our functionality of our chat room. One example of this was when we first implemented our logger, it made us have to click every GUI button twice for the GUI to respond.

    Another problem is that when running on a windows, it is not allows to have multiple clients connected. I'm not sure if this is a MAUI bug, a problem with Alex's computer or something else, but if a second instance of Visual Studio that is running the ChatClient GUI, it disconnects the old Client and replaces it. As far as we know, this is not a problem on Toby's laptop (mac user) so if there is testing for multiple clients, it might be better to try and run it on a mac book.

    Based on our understanding of the command messages, we decided not to let the clients be able to send these messages. We do in fact have them show up in the chat room just to notify other clients what they are trying to send. For example, when a client presses the participant button, it send that message on the backend to the server but displays the entire message to the chat room. 

    When starting a server, and trying to connect a client to it, the text that is in the Server IP address entry on the server GUI needs to be in the Server Name/Address entry on the Chat Client side.

#Partnership Information

    All code in this solution was completed using pair programming techniques. This includes many in person and zoom sessions throughout the assignment time line. 

#Branching

    Due to all of the code being worked on together, no branches were made. the only branch within this repo is the master branch.

#Testing

    This solution was tested using the individual GUI interfaces in combination with the debugger. Many trials were run in order to ensure that the program acted as expected in as many cases as we could think of. Some examples of tests were connecting multiple clients to the same server, sending messages, retrieving participants, etc...

#Time Tracking (Personal Software Practice)

    ESTIMATED TIME: 15 HRS
    TRACKED TIME: (all time estimates are representative of time spent pair programming)
        Effective time spent: 10 HRS
        Debugging time spent: 8 - 9 HRS
        Learning time spent:  6 - 7 HRS

   Our time estimates are generally getting better the more assignments we complete. We estimated that this assignment would take longer than usual because of its difficult concepts, but we did not foresee just how long it would take. It was interesting to see how much time we spent "ineffectively", that is time spent debugging and trying to learn concepts. This stemmed from the many new and challenging topics that were thrown at us during this assignment. We took time in class to go over these topics but even then they were still unclear. I think the reason it took so long to get a grasp of all of these concepts (i.e. tcpClients, await, async, etc...) is that they were all interconnected, so failure to understand one would result in failure to understand the others and how they interact. In general, we are getting better at recognizing which assignments are going to be more difficult and thus take more time, but there are usually some unforeseen challenges that add time to our original estimates.
