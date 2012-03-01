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


        Shooter game;

        NetClient client;
        


        //Below is the column that save player information
        Project_Origin.Player.PlayerId playerId = Project_Origin.Player.PlayerId.Red;


        public NetworkingClient(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game = (Shooter)game;
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
            // TODO: Add your update code here
            
            if(true)
            {
                //
                // If there's input; send it to server
                //
                NetOutgoingMessage om = client.CreateMessage();
                om.Write((byte)OutgoingMessageType.DataPlayerInfo);
                Vector3 tempPos = this.game.gamePlayer.GetPlayerPosition();
                float tempOri = this.game.gamePlayer.GetPlayerOrientation();
                om.Write(tempPos.X);
                om.Write(tempPos.Y);
                om.Write(tempPos.Z);
                om.Write(tempOri);
                client.SendMessage(om, NetDeliveryMethod.Unreliable);

            }

            //read message
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch(msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        //just connect to first server discovered
                        client.Connect(msg.SenderEndpoint);
                        playerId = (Project_Origin.Player.PlayerId)(msg.ReadInt32());
                        this.game.gamePlayer.SetPlayId();
                        break;
                    case NetIncomingMessageType.Data:
                        //server sent a position update
                        IncomingMessageType imt = (IncomingMessageType)msg.ReadByte();
                        if (imt == IncomingMessageType.DataOtherPlayerInfo)
                        {
                            otherPlayerInfo.position.X = msg.ReadFloat();
                            otherPlayerInfo.position.Y = msg.ReadFloat();
                            otherPlayerInfo.position.Z = msg.ReadFloat();
                            otherPlayerInfo.orientation = msg.ReadFloat();
                        }
                        break;
                        /*long who = msg.ReadInt64();
                        int x = msg.ReadInt32();
                        int y = msg.ReadInt32();
                        positions[who] = new Vector2(x, y);
                        break;*/
                }
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