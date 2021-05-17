using Geode.Extension;
using Geode.Network;
using Geode.Network.Protocol;
using System;
using System.Windows;

namespace ConsoleBotCSharp
{
    public class Extension
    {
        public MainWindow MainWindowParent;
        public ConsoleBot ConsoleBot;
        public GeodeExtension Ext;
        public Extension(MainWindow MainWindowParent)
        {
            this.MainWindowParent = MainWindowParent; // Set main window.
            Ext = new GeodeExtension("ConsoleBotCSharp", "Geode examples.", "Lilith", true, false); // Instantiate extension
            //Add extension event handlers
            Ext.OnDataInterceptEvent += Extension_OnDataInterceptEvent;
            Ext.OnDoubleClickEvent += Extension_OnDoubleClickEvent;
            Ext.OnConnectedEvent += Extension_OnConnectedEvent;
            Ext.OnCriticalErrorEvent += Extension_OnCriticalErrorEvent;
            //
            Ext.Start(); // Start extension
            ConsoleBot = new ConsoleBot(Ext, "VB example"); // Instantiate a new ConsoleBot
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
            ConsoleBot.BotSendMessage("Use /help to get info.");
        }

        private void ConsoleBot_OnMessageReceived(object sender, string e)
        {
            switch (e.ToLower() ?? "") // Handle received message
            {
                case "/help":
                    {
                        ConsoleBot.BotSendMessage("Commands:");
                        ConsoleBot.BotSendMessage("/look1 and /look2 to change current look.");
                        ConsoleBot.BotSendMessage("/sit to force sit.");
                        ConsoleBot.BotSendMessage("/fx to get light sabber fx.");
                        ConsoleBot.BotSendMessage("/exit to exit extension.");
                        break;
                    }

                case "/look1":
                    {
                        Ext.SendToServerAsync(Ext.Out.UpdateFigureData, "F", "hr-515-45.ch-665-71.lg-3216-73.hd-600-10.fa-3276-72");
                        break;
                    }

                case "/look2":
                    {
                        Ext.SendToServerAsync(Ext.Out.UpdateFigureData, "M", "hr-893-45.ch-235-71.lg-3290-82.hd-180-10.fa-3276-72");
                        break;
                    }

                case "/sit":
                    {
                        Ext.SendToServerAsync(Ext.Out.ChangePosture, 1);
                        break;
                    }

                case "/fx":
                    {
                        Ext.SendToServerAsync(Ext.Out.Chat, ":yyxxabxa", 0, -1);
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
            if (e.Packet.Id == Ext.In.FriendRequests.Id) // Show Bot when the initial console load is complete.
            {
                BotShowAndWelcome();
            }
        }

        private void Extension_OnConnectedEvent(object sender, HPacket e) // G-Earth is connected.
        {
            BotShowAndWelcome();
        }

        private void Extension_OnDoubleClickEvent(object sender, HPacket e) // G-Earth extension play button clicked.
        {
            if (Ext.IsConnected)
            {
                BotShowAndWelcome();
            }
        }

        private void Extension_OnCriticalErrorEvent(object sender, string e) // G-Earth is probably closed or the connection was rejected.
        {
            MessageBox.Show(e, "Critical error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }
    }
}
