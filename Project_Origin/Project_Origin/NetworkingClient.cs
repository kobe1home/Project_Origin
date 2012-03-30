using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Lidgren.Network;

namespace Project_Origin
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class NetworkingClient : Microsoft.Xna.Framework.DrawableGameComponent
    {


        public enum IncomingMessageType
        {
            CommandChangeStatusToPlan,
            CommandChangeStatusToCommit,

            DataSelfPlayerInfo,
            DataOtherPlayerInfo
        }

        public enum OutgoingMessageType
        {
            CommandFinishPlan,

            DataPlayerInfo
        }

        public struct PlayerInfo
        {
            public PlayerInfo(Vector3 pos, float ori)
            {
                position = pos;
                orientation = ori;
            }
            public Vector3 position;
            public float orientation;
        }
        public PlayerInfo otherPlayerInfo = new PlayerInfo(new Vector3(0, 0, 0), 0);

        private List<WayPoint> opponentWaypoint = new List<WayPoint>();

        Shooter game;

        NetClient client;

        KeyboardState prevKeyboardState;


        //Below is the column that save player information
        Project_Origin.Player.PlayerId playerId = Project_Origin.Player.PlayerId.Red;
        InternalMap map;


        public NetworkingClient(Game game)
            : base(game)
        {
            this.game = (Shooter)game;
            this.prevKeyboardState = Keyboard.GetState();

            NetPeerConfiguration config = new NetPeerConfiguration("projectorigin");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            client.DiscoverLocalPeers(14242);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Enter) && 
                prevKeyboardState.IsKeyUp(Keys.Enter) &&
                this.game.GetGameStatus() == Shooter.GameStatus.Simulation && 
                this.game.GetGameStatus() != Shooter.GameStatus.Sending && 
                this.game.GetGameStatus() != Shooter.GameStatus.Receive)
            {
                this.game.SetGameStatus(Shooter.GameStatus.Sending);
            }
            this.prevKeyboardState = keyboard;


            if (this.game.GetGameStatus() == Shooter.GameStatus.Sending)
            {
                if (this.game.bMapIsReady)
                {
                    //
                    // If there's input; send it to server
                    //
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write((byte)OutgoingMessageType.DataPlayerInfo);
                    om.Write(this.game.path.GetWayPoints().Count);
                    foreach (WayPoint point in this.game.path.GetWayPoints())
                    { 
                        Vector3 tempPos = point.CenterPos;//this.game.gamePlayer.GetPlayerPosition();
                        float tempOri = this.game.gamePlayer.GetPlayerOrientation();
                        om.Write(tempPos.X);
                        om.Write(tempPos.Y);
                        om.Write(tempPos.Z);
                        om.Write(tempOri);
                    }
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);

                }
                this.game.SetGameStatus(Shooter.GameStatus.Receive);
            }

            if (this.game.GetGameStatus() == Shooter.GameStatus.Receive || 
                this.game.GetGameStatus() == Shooter.GameStatus.MainMenu)
            {
                //read message
                NetIncomingMessage msg;
                while ((msg = client.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            //just connect to first server discovered
                            client.Connect(msg.SenderEndpoint);
                            playerId = (Project_Origin.Player.PlayerId)(msg.ReadInt32());
                            int mapSeed = msg.ReadInt32();
                            this.game.bMapIsReady = true;
                            this.game.BuildGameComponents(mapSeed);
                            this.game.gamePlayer.SetPlayId();
                            break;
                        case NetIncomingMessageType.Data:
                            //server sent a position update
                            IncomingMessageType imt = (IncomingMessageType)msg.ReadByte();
                            if (imt == IncomingMessageType.DataOtherPlayerInfo)
                            {
                                opponentWaypoint.Clear();
                                int wayPointCounter = msg.ReadInt32();
                                for (int i = 1; i <= wayPointCounter; ++i)
                                {
                                    otherPlayerInfo.position.X = msg.ReadFloat();
                                    otherPlayerInfo.position.Y = msg.ReadFloat();
                                    otherPlayerInfo.position.Z = msg.ReadFloat();
                                    otherPlayerInfo.orientation = msg.ReadFloat();

                                    Console.WriteLine("{0} {1} {2}",otherPlayerInfo.position.X,otherPlayerInfo.position.Y,otherPlayerInfo.position.Z);
                                    opponentWaypoint.Add(new WayPoint(this.game.GraphicsDevice, new Vector3(otherPlayerInfo.position.X,
                                                                                                        otherPlayerInfo.position.Y,
                                                                                                        otherPlayerInfo.position.Z)));
                                }
                                
                                
                            }
                            this.game.gamePlayer.Path.OpponentWayPoints = opponentWaypoint;
                            this.game.SetGameStatus(Shooter.GameStatus.Start);
                            break;
                    }
                }
                //if (this.game.GetGameStatus() == Shooter.GameStatus.MainMenu)
                //{
                //    //this.game.SetGameStatus(Shooter.GameStatus.Simulation);
                //}
                //else
                //{
                //    this.game.SetGameStatus(Shooter.GameStatus.Start);
                //}
            }

            
            base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            client.Shutdown("bye");
            base.UnloadContent();
        }

        public Project_Origin.Player.PlayerId GetPlayerId()
        {
            return playerId;
        }
    }
}
