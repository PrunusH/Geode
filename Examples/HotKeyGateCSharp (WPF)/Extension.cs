using Geode.Extension;
using Geode.Network;
using Geode.Network.Protocol;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ConsoleBotCSharp
{
    [Module("HotKeyGate", "Lilith", "Geode examples.", true, false)]
    public class Extension : GService
    {
        public MainWindow MainWindowParent;
        public ConsoleBot ConsoleBot;
        int RemainingNewGates = 0;
        List<int> CurrentGatesIDs = new List<int>();

        public Extension(MainWindow MainWindowParent)
        {
            this.MainWindowParent = MainWindowParent; // Set main window.
            //Add extension event handlers
            OnDataInterceptEvent += Extension_OnDataInterceptEvent;
            OnDoubleClickEvent += Extension_OnDoubleClickEvent;
            OnConnectedEvent += Extension_OnConnectedEvent;
            OnCriticalErrorEvent += Extension_OnCriticalErrorEvent;
            //
            //Add gates hotkeys
            Key[] BindingKeys = { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };
            foreach (Key BindingKey in BindingKeys)
            {
                new HotKey(BindingKey, KeyModifier.Shift, OnHotKeyHandler);
            }
            //
            ConsoleBot = new ConsoleBot(this, "HotKeyGate"); // Instantiate a new ConsoleBot
            ConsoleBot.OnMessageReceived += ConsoleBot_OnMessageReceived; //Add ConsoleBot event handler
        }

        private void OnHotKeyHandler(HotKey hotKey) // HotKey pressed
        {
            try
            {
                int KeyNumber = int.Parse(hotKey.Key.ToString().Remove(0, 1));
                if (KeyNumber == 0 && CurrentGatesIDs.Count == 10)
                {
                    SendToServerAsync(Out.EnterOneWayDoor, CurrentGatesIDs[CurrentGatesIDs.Count - 1]);
                }
                else
                {
                    SendToServerAsync(Out.EnterOneWayDoor, CurrentGatesIDs[KeyNumber - 1]);
                }
            }
            catch { }
        }

        public void BotShowAndWelcome()
        {
            ConsoleBot.ShowBot();
            BotWelcome();
        }

        public void BotWelcome()
        {
            ConsoleBot.BotSendMessage("Welcome |");
            ConsoleBot.BotSendMessage("Use /newgate COUNT to select gates or /exit to exit.");
        }

        private void ConsoleBot_OnMessageReceived(object sender, string e)
        {
            switch (e.ToLower() ?? "") // Handle received message
            {
                case string s when s.StartsWith("/newgate "): // New gate command requested
                    {
                        try
                        {
                            int NewGatesCount = int.Parse(s.Remove(0, 9));
                            if (!(NewGatesCount > 0 && NewGatesCount <= 10))
                            {
                                throw new Exception("Gates count out of index.");
                            }
                            CurrentGatesIDs.Clear();
                            RemainingNewGates = NewGatesCount;
                            if (RemainingNewGates == 1)
                            {
                                ConsoleBot.BotSendMessage("Now select " + NewGatesCount.ToString() + " gate by clicking on it.");
                            }
                            else
                            {
                                ConsoleBot.BotSendMessage("Now select " + NewGatesCount.ToString() + " gates by clicking on them.");
                            }
                        }
                        catch
                        {
                            ConsoleBot.BotSendMessage("New gate count should be a number from 1 to 10.");
                        }

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
            if (e.Packet.Id == Out.EnterOneWayDoor.Id)  // OneWayDoor clicked
            {
                if (RemainingNewGates > 0)
                {
                    CurrentGatesIDs.Add(e.Packet.ReadInt32());
                    RemainingNewGates -= 1;
                    switch (RemainingNewGates)
                    {
                        case 0:
                            ConsoleBot.BotSendMessage("Ready! Now use Shift + Number to open a gate.");
                            break;
                        case 1:
                            ConsoleBot.BotSendMessage("You have " + RemainingNewGates.ToString() + " gate remaining.");
                            break;
                        default:
                            ConsoleBot.BotSendMessage("You have " + RemainingNewGates.ToString() + " gates remaining.");
                            break;
                    }
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
