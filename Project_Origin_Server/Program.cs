using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace Project_Origin_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("projectorigin");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

            // Create and start server
            NetServer server = new NetServer(config);
            server.Start();

            //Schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;
            
            //Run until escape is pressed
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                        //
                        // Server received a discovery request from a client; send a discovery response (with no extra data attached)
                        //
                            server.SendDiscoveryResponse(null, msg.SenderEndpoint);
                            Console.WriteLine("Connect to" + msg.SenderEndpoint);
                            break;
                        case NetIncomingMessageType.Data:
                        //
                        // The client sent input to the server
                        //
                            int xinput = msg.ReadInt32();
                            int yinput = msg.ReadInt32();

                            int[] pos = msg.SenderConnection.Tag as int[];

                            pos[0] = xinput;
							pos[1] = yinput;
                            break;
                    }

                    //
                    // send position updates 30 times per second
                    //
                    double now = NetTime.Now;
                    if (now > nextSendUpdates)
                    {
                        // Yes, it's time to send position updates

                        // for each player...

                        foreach (NetConnection player in server.Connections)
                        {
                            //send information about every other player(actually including self)
                            foreach(NetConnection otherPlayer in server.Connections)
                            {
                                // send position update about 'otherPlayer' to 'player'
                                NetOutgoingMessage om = server.CreateMessage();

                                //Write who this position is for
                                om.Write(otherPlayer.RemoteUniqueIdentifier);

                                if(otherPlayer.Tag == null)
                                    otherPlayer.Tag = new int[2];

                                int[] pos = otherPlayer.Tag as int[];
                                om.Write(pos[0]);
                                om.Write(pos[1]);

                                //send message
                                server.SendMessage(om, player, NetDeliveryMethod.Unreliable);
                            }

                        }
                        //schedule next update
                        nextSendUpdates += (1.0 / 30.0);

                    }
                }
                //sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            server.Shutdown("server exiting");
        }
    }
}
