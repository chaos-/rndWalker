/*
 *  Copyright (C) 2012 k_os <ben.at.hemio.de>, daveroda, rolle3k
 * 
 *  This file is part of rndWalker.
 *
 *  rndWalker is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  rndWalker is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Foobar.  If not, see <http://www.gnu.org/licenses/>. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using D3;
using rndWalker.Common;
using rndWalker.Bots;

namespace rndWalker {
    class rndWalker {
        public static rndWalker Instance { get; set; }
        private Thread thread;
        private Logger logger;
        private int gameCounter = 0;
        private Bot bot;
        private String botClass = "Sarkoth";

        public rndWalker(string[] args) {
            this.thread = new Thread(this.ExecutingThread);
            this.logger = new Logger("rndWalker");

            if (args != null)
                this.botClass = args[0];

            Type botType = Type.GetType("rndWalker.Bots." + this.botClass);
            if (botType != null) {
                bot = (Bot)Activator.CreateInstance(botType);
            } else {
                //TODO needfix: this will output the error and stop the boot from running, but the file is still loaded.
                throw new Exception("Class rndWalker.Bots." + this.botClass + " not found. Exiting.");
            }

            Game.OnTickEvent += new TickEventHandler(Game_OnTickEvent);
        }


        private float oldX = 0, oldY = 0;
        private float lastMoveTick = 0;
        void Game_OnTickEvent(EventArgs e) {
            if (Game.Ingame) {
                if (Me.X != oldX || Me.Y != oldY) {
                    oldX = Me.X;
                    oldY = Me.Y;
                    lastMoveTick = System.Environment.TickCount;
                }
                if (System.Environment.TickCount - lastMoveTick > 15 * 1000 || (Me.WorldId != 0 && Me.Life == 0)) {
                    Console.WriteLine("I died! Exitting game");
                    Bot.ExitGame();
                    // hard reset
                    rndWalker.Instance.Restart();
                }
            } else {
                lastMoveTick = System.Environment.TickCount;
            }
        }

        public void Start() {
            this.thread.Start();
        }

        public void Stop() {
            this.thread.Abort();
        }

        public void Restart() {
            this.thread.Abort();
            this.thread = new Thread(this.ExecutingThread);
            this.thread.Start();
        }

        private void ExecutingThread() {
            Thread.Sleep(500);

            while (true) {
                Thread.Sleep(1000);

                int start = System.Environment.TickCount;

                try {
                    bot.Execute();
                    Thread.Sleep(5000);
                } catch (System.Threading.ThreadAbortException) {
                    this.logger.Log("Bot thread was killed by watchdog.");
                } catch (Exception e) {
                    this.logger.Log(e);
                }

                int diff = System.Environment.TickCount - start;

                Game.Print(string.Format("Run took {0} seconds. Total Runs: {1}", diff / 1000, (++gameCounter)));
                Thread.Sleep(3000);
            }
        }
    }
}