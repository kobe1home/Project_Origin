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

        static Dictionary<IPEndPoint, List<PlayerInfo>> playerInfoDict = new Dictionary<IPEndPoint, List<PlayerInfo>>(); //key: senderEndpoint value: a list of position and orientation
        
        public enum GameStatus
        {
            Initial,
            Receive,
            Sending
        }

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

            GameStatus gameStatus = GameStatus.Initial;
            int clientDoneCounter = 0;

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
                                    gameStatus = GameStatus.Receive;
                                    break;
                            }
                            server.SendDiscoveryResponse(com, msg.SenderEndpoint);
                            Console.WriteLine("Connect to: " + msg.SenderEndpoint);
                            break;
                        case NetIncomingMessageType.Data:
                            if (gameStatus == GameStatus.Receive)
                            {
                                //
                                // The client sent input to the server
                                //
                                IncomingMessageType imt = (IncomingMessageType)msg.ReadByte();
                                if (imt == IncomingMessageType.DataPlayerInfo)
                                {
                                    int wayPointCounter = msg.ReadInt32();

                                    List<PlayerInfo> wayPoints = new List<PlayerInfo>();
                                    for (int i = 1; i <= wayPointCounter; ++i)
                                    {
                                        Vector3 tempPos;
                                        tempPos.X = msg.ReadFloat();
                                        tempPos.Y = msg.ReadFloat();
                                        tempPos.Z = msg.ReadFloat();

                                        float tempOrientation;
                                        tempOrientation = msg.ReadFloat();

                                        PlayerInfo wayPoint = new PlayerInfo(tempPos, tempOrientation);
                                        wayPoints.Add(wayPoint);
                                    }
                                    playerInfoDict[msg.SenderEndpoint] = wayPoints;
                                    Console.WriteLine("Receive message from" + msg.SenderEndpoint);
                                    
                                    clientDoneCounter++;
                                    if (clientDoneCounter == 2)
                                        gameStatus = GameStatus.Sending;
                                    
                                }
                            }
                            break;
                    }

                    //
                    // send position updates 30 times per second
                    //
                    if (gameStatus == GameStatus.Sending)
                    {
                            // Yes, it's time to send position updates

                            // for each player...
                            foreach (NetConnection player in server.Connections)
                            {
                                if (!playerInfoDict.ContainsKey(player.RemoteEndpoint))
                                    playerInfoDict[player.RemoteEndpoint] = new List<PlayerInfo>();
                            }
                            foreach (NetConnection player in server.Connections)
                            {
                                //send information about every other player(not including self)
                                foreach (NetConnection otherPlayer in server.Connections)
                                {
                                    if (player.RemoteEndpoint == otherPlayer.RemoteEndpoint)
                                        continue;

                                    // send position update about 'otherPlayer' to 'player'
                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write((byte)OutgoingMessageType.DataOtherPlayerInfo);

                                    om.Write(playerInfoDict[otherPlayer.RemoteEndpoint].Count);

                                    foreach (PlayerInfo playerInfo in playerInfoDict[otherPlayer.RemoteEndpoint])
                                    {
                                        om.Write(playerInfo.position.X);
                                        om.Write(playerInfo.position.Y);
                                        om.Write(playerInfo.position.Z);
                                        om.Write(playerInfo.orientation);
                                    }
                                    //send message
                                    server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered);
                                }

                            }

                            gameStatus = GameStatus.Receive;
                            clientDoneCounter = 0;

                        }
                }
            }

            server.Shutdown("server exiting");
        }
    }
}
