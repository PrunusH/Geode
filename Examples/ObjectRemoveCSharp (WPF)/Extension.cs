using Geode.Extension;
using Geode.Network;
using Geode.Network.Protocol;
using System;

namespace ConsoleBotCSharp
{
    [Module("ObjectRemove", "Lilith", "Geode examples.", true, false)]
    public class Extension : GService
    {
        public bool RemoveEnabled = false;
        public MainWindow MainWindowParent;
        public ConsoleBot ConsoleBot;

        public Extension(MainWindow MainWindowParent)
        {
            this.MainWindowParent = MainWindowParent; // Set main window.
            //Add extension event handlers
            OnDataInterceptEvent += Extension_OnDataInterceptEvent;
            OnDoubleClickEvent += Extension_OnDoubleClickEvent;
            OnConnectedEvent += Extension_OnConnectedEvent;
            OnCriticalErrorEvent += Extension_OnCriticalErrorEvent;
            //
            ConsoleBot = new ConsoleBot(this, "ObjectRemove"); // Instantiate a new ConsoleBot
            ConsoleBot.OnMessageReceived += ConsoleBot_OnMessageReceived; //Add ConsoleBot event handler
        }

        public void BotShowAndWelcome()
        {
            ConsoleBot.ShowBot();
            BotWelcome();
        }

        public void BotWelcome()
        {
            ConsoleBot.BotSendMessage("Welcome |");
            ConsoleBot.BotSendMessage("Use /start or /stop or /exit.");
        }

        private void ConsoleBot_OnMessageReceived(object sender, string e)
        {
            switch (e.ToLower() ?? "") // Handle received message
            {
                case "/start":
                    {
                        RemoveEnabled = true;
                        ConsoleBot.BotSendMessage("Started! Click furnis to remove them.");
                        break;
                    }

                case "/stop":
                    {
                        RemoveEnabled = false;
                        ConsoleBot.BotSendMessage("Stopped!");
                        break;
                    }

                default:
                    {
                        BotWelcome();
                        break;
                    }
            }
        }

        private void Extension_OnDataInterceptEvent(object sender, DataInterceptedEventArgs e)
        {
            if (e.Packet.Id == In.FriendRequests.Id) // Show Bot when the initial console load is complete.
            {
                BotShowAndWelcome();
            }
            if (e.Packet.Id == Out.UseFurniture.Id) // Furni clicked.
            {
               if (RemoveEnabled)
                {
                    SendToClientAsync(In.ObjectRemove, e.Packet.ReadInt32().ToString(),false, e.Packet.ReadInt32(),0);
                    e.IsBlocked = true;
                }
            }
        }

        private void Extension_OnConnectedEvent(object sender, HPacket e) // G-Earth is connected.
        {
            BotShowAndWelcome();
        }

        private void Extension_OnDoubleClickEvent(object sender, HPacket e) // G-Earth extension play button clicked.
        {
            if (IsConnected)
            {
                BotShowAndWelcome();
            }
        }

        private void Extension_OnCriticalErrorEvent(object sender, string e) // G-Earth is probably closed or the connection was rejected.
        {
            Environment.Exit(0);
        }
    }
}
