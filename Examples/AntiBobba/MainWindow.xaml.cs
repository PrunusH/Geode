using System;
using System.Text;
using System.Windows;
using Geode.Extension;
using Geode.Network;
using Geode.Network.Protocol;

namespace AntiBobba
{
    //To do: Public chat/whisper must be trimmed to 66 characters and console to 43.
    public partial class MainWindow : Window
    {
        public ConsoleBot ConsoleBot;
        public GeodeExtension Extension;
        public bool IsEnabled = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Extension = new GeodeExtension("AntiBobba", "Geode examples.", "Lilith", true, false); // Instantiate extension
            //Add extension event handlers
            Extension.OnDataInterceptEvent += Extension_OnDataInterceptEvent;
            Extension.OnDoubleClickEvent += Extension_OnDoubleClickEvent;
            Extension.OnConnectedEvent += Extension_OnConnectedEvent;
            Extension.OnCriticalErrorEvent += Extension_OnCriticalErrorEvent;
            //
            Extension.Start(); // Start extension
            ConsoleBot = new ConsoleBot(Extension, "AntiBobba"); // Instantiate a new ConsoleBot
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
            ShowEnabledInfo();
        }

        public void ShowEnabledInfo()
        {
            if (IsEnabled == true)
            {
                ConsoleBot.BotSendMessage("BobbaBlock is enabled, use /stop to stop.");
            }
            else
            {
                ConsoleBot.BotSendMessage("BobbaBlock is disabled, use /start to start.");
            }
        }

        public void StartFilter()
        {
            IsEnabled = true;
            ShowEnabledInfo();
        }

        public void StopFilter()
        {
            IsEnabled = false;
            ShowEnabledInfo();
        }

        public string BypassFilter(string input)
        {
            string[] AllowedWords = new string[] { ":sit", ":stand", ":habnam", ":moonwalk" };
            foreach (string AllowedWord in AllowedWords)
            {
                if (AllowedWord == input)
                {
                    return input;
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char value in input)
            {
                stringBuilder.Append("ѵѫ"); //Alternative: ӵӵ)
                stringBuilder.Append(value);
            }
            stringBuilder.Append("ѵѫ"); //Alternative: ӵӵ
            return stringBuilder.ToString();
        }

        private void ConsoleBot_OnMessageReceived(object sender, string e)
        {
            switch (e.ToLower()) // Handle received message
            {
                case "/start":
                    {
                        StartFilter();
                        break;
                    }

                case "/stop":
                    {
                        StopFilter();
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
            if (Extension.In.FriendRequests.Match(e)) // Show Bot when the initial console load is complete.
            {
                BotShowAndWelcome();
            }
            if (Extension.Out.Chat.Match(e) && IsEnabled) //Public chat
            {
                e.IsBlocked = true;
                Extension.SendToServerAsync(Extension.Out.Chat, BypassFilter(e.Packet.ReadUTF8()), e.Packet.ReadInt32(), e.Packet.ReadInt32());
            }
            if (Extension.Out.Whisper.Match(e) && IsEnabled) //Whisper
            {
                string WhisperOriginal = e.Packet.ReadUTF8();
                string WhisperDestination = WhisperOriginal.Remove(WhisperOriginal.IndexOf(" "));
                string WhisperMessage = WhisperOriginal.Remove(0, WhisperOriginal.IndexOf(" ") + 1);
                string WhisperModded = WhisperDestination + " " + BypassFilter(WhisperMessage);
                e.IsBlocked = true;
                Extension.SendToServerAsync(Extension.Out.Whisper, WhisperModded, e.Packet.ReadInt32());
            }
            if (Extension.Out.SendMsg.Match(e) && IsEnabled) //Console chat
            {
                int MessageDestination = e.Packet.ReadInt32();
                if (MessageDestination == ConsoleBot.BotID)
                {
                    e.Packet.Position = 0; //Restore packet read position if destination is console bot
                }
                else
                {
                    string MessageModded = BypassFilter(e.Packet.ReadUTF8());
                    e.IsBlocked = true;
                    Extension.SendToServerAsync(Extension.Out.SendMsg, MessageDestination, MessageModded);
                }
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
