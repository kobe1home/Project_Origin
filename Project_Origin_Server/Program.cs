using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Project_Origin_Server
{
    //TODO: Just show example how to serialize and deserialize class, remove in future
    class dummyClass
    {
        public dummyClass()
        {
            ;
        }
    }

    class Program
    {
        struct PlayerInfo
        {
            public PlayerInfo(Vector3 pos, float ori)
            {
                position = pos;
                orientation = ori;
            }
            public Vector3 position;
            public float orientation;
        }

        static Dictionary<IPEndPoint, PlayerInfo> playerInfoDict = new Dictionary<IPEndPoint, PlayerInfo>(); //key: senderEndpoint value: a list of position and orientation
        
        public enum PlayerId
        {
            Green,
            Red
        }

        public enum OutgoingMessageType
        {
            CommandChangeStatusToPlan,
            CommandChangeStatusToCommit,
            
            DataSelfPlayerInfo,
            DataOtherPlayerInfo,
        }

        public enum IncomingMessageType
        {
            CommandFinishPlan,
            
            DataPlayerInfo,

            DummyObjectData
        }

        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("projectorigin");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

            // Create and start server
            NetServer server = new NetServer(config);
            server.Start();
            Console.WriteLine("Game Server starts");

            //Schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;

            //Generate a map and wait client to connect
            //InternalMap map = GenerateMap();
            int mapSeed = 1000;
            Random rand = new Random();
            mapSeed = rand.Next();
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
                            NetOutgoingMessage com = server.CreateMessage();
                            //byte[] mapData = Serializer<InternalMap>.SerializeObject(map);
                            switch (server.ConnectionsCount)
                            {
                                case 0:
                                    com.Write((int)PlayerId.Green);
                                    com.Write((int)mapSeed); //Write map seed
                                    break;
                                case 1:
                                    com.Write((int)PlayerId.Red);
                                    com.Write((int)mapSeed); //Write map seed
                                    break;
                            }
                            server.SendDiscoveryResponse(com, msg.SenderEndpoint);
                            Console.WriteLine("Connect to: " + msg.SenderEndpoint);
                            break;
                        case NetIncomingMessageType.Data:
                            //
                            // The client sent input to the server
                            //
                            IncomingMessageType imt = (IncomingMessageType)msg.ReadByte();
                            if (imt == IncomingMessageType.CommandFinishPlan)
                            {

                            }
                            else if (imt == IncomingMessageType.DataPlayerInfo)
                            {
                                Vector3 tempPos;
                                tempPos.X = msg.ReadFloat();
                                tempPos.Y = msg.ReadFloat();
                                tempPos.Z = msg.ReadFloat();

                                float tempOrientation;
                                tempOrientation = msg.ReadFloat();

                                playerInfoDict[msg.SenderEndpoint] = new PlayerInfo(tempPos, tempOrientation);
                            }
                            else if (imt == IncomingMessageType.DummyObjectData)
                            {
                                //TODO: Just an example to show deserialize object, remove this in future
                                int objDataSize = msg.ReadInt32();
                                byte[] objData = new byte[objDataSize];
                                dummyClass obj = Serializer<dummyClass>.DeserializeObject<dummyClass>(objData);
                            }
                            break;

                            
                            //int xinput = msg.ReadInt32();
                            //int yinput = msg.ReadInt32();

                            //int[] pos = msg.SenderConnection.Tag as int[];

                            //pos[0] = xinput;
                            //pos[1] = yinput;
                            //break;
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
                            if (!playerInfoDict.ContainsKey(player.RemoteEndpoint))
                                playerInfoDict[player.RemoteEndpoint] = new PlayerInfo(new Vector3(0, 0, 0), 0);
                        }
                        foreach (NetConnection player in server.Connections)
                        {
                            //send information about every other player(not including self)
                            foreach(NetConnection otherPlayer in server.Connections)
                            {
                                if (player.RemoteEndpoint == otherPlayer.RemoteEndpoint)
                                    continue;

                                // send position update about 'otherPlayer' to 'player'
                                NetOutgoingMessage om = server.CreateMessage();
                                om.Write((byte)OutgoingMessageType.DataOtherPlayerInfo);
                                //Write who this position is for
                                //om.Write(otherPlayer.RemoteUniqueIdentifier);

                                //if(otherPlayer.Tag == null)
                                //    otherPlayer.Tag = new int[2];

                                //int[] pos = otherPlayer.Tag as int[];
                                //om.Write(pos[0]);
                                //om.Write(pos[1]);
                                PlayerInfo playerInfo = playerInfoDict[otherPlayer.RemoteEndpoint];
                                om.Write(playerInfo.position.X);
                                om.Write(playerInfo.position.Y);
                                om.Write(playerInfo.position.Z);
                                om.Write(playerInfo.orientation);

                                //send message
                                server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered);
                            }

                        }
                        //schedule next update
                        nextSendUpdates += (1.0 / 10.0);

                    }
                }
                //sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            server.Shutdown("server exiting");
        }
        /*
        static InternalMap GenerateMap()
        {
            InternalMap internalMap = new InternalMap(160, 80, 8, 8);
            internalMap.GenerateRandomMap();
            //internalMap.convertMapNodes();
            //internalMap.printMaps();
            return internalMap;
        }
         * */
    }
}
