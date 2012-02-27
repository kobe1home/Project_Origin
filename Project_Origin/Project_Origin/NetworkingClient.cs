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
        NetClient client;
        public Dictionary<long, Vector2> positions = new Dictionary<long, Vector2>();

        public NetworkingClient(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
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
                om.Write(10);
                om.Write(20);
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
                        break;
                    case NetIncomingMessageType.Data:
                        //server sent a position update
                        long who = msg.ReadInt64();
                        int x = msg.ReadInt32();
                        int y = msg.ReadInt32();
                        positions[who] = new Vector2(x, y);
                        break;
                }
            }
            base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            client.Shutdown("bye");
            base.UnloadContent();
        }
    }
}
