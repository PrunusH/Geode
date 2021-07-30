﻿using System;
using System.Windows;
using Geode.Extension;
using Geode.Network;
using Geode.Network.Protocol;

namespace ConsoleBotCSharp
{
    public partial class MainWindow : Window
    {
        public ConsoleBot ConsoleBot;
        public GeodeExtension Extension;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Extension = new GeodeExtension("ConsoleBotCSharp", "Geode examples.", "Lilith", true, false); // Instantiate extension
            //Add extension event handlers
            Extension.OnDataInterceptEvent += Extension_OnDataInterceptEvent;
            Extension.OnDoubleClickEvent += Extension_OnDoubleClickEvent;
            Extension.OnConnectedEvent += Extension_OnConnectedEvent;
            Extension.OnCriticalErrorEvent += Extension_OnCriticalErrorEvent;
            //
            Extension.Start(); // Start extension
            ConsoleBot = new ConsoleBot(Extension, "CSharp example"); // Instantiate a new ConsoleBot
            ConsoleBot.OnMessageReceived += ConsoleBot_OnMessageReceived; //Add ConsoleBot event handler
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Extension.IsConnected)
            {
                ConsoleBot.HideBot(); // Hide bot before app closes
            }
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
            switch (e.ToLower()) // Handle received message
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
                        Extension.SendToServerAsync(Extension.Out.UpdateFigureData, "F", "hr-515-45.ch-665-71.lg-3216-73.hd-600-10.fa-3276-72");
                        break;
                    }

                case "/look2":
                    {
                        Extension.SendToServerAsync(Extension.Out.UpdateFigureData, "M", "hr-893-45.ch-235-71.lg-3290-82.hd-180-10.fa-3276-72");
                        break;
                    }

                case "/sit":
                    {
                        Extension.SendToServerAsync(Extension.Out.ChangePosture, 1);
                        break;
                    }

                case "/fx":
                    {
                        Extension.SendToServerAsync(Extension.Out.Chat, ":yyxxabxa", 0, -1);
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
            if (e.Packet.Id == Extension.In.FriendRequests.Id) // Show Bot when the initial console load is complete.
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
            if (Extension.IsConnected)
            {
                BotShowAndWelcome();
            }
        }

        private void Extension_OnCriticalErrorEvent(object sender, string e) // Extension critical error.
        {
            MessageBox.Show(e + ".", "Critical error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }
    }
}   