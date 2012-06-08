/*
 *  Copyright (C) 2012 k_os <ben.at.hemio.de>
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

namespace rndWalker.Bots {
    class Sarkoth : Bot {
        public void repair() {
            if (needsRepair() || getInventoryFreeSpace() <= 6) {
                Thread.Sleep(1000);
                GoTown();
                walk(2966, 2825, true);
                walk(2941.5f, 2850.7f, true);
                interact("Salvage", true);
                Thread.Sleep(700);
                SnagIt.SalvageItems();
                closeInventory();
                walk(2940, 2813, true);
                walk(2895, 2785, true);
                // 0002B8E3: 00000001(00000000) # {c:ffffffff}Tashun the Miner{/c} = 0x2F424FF
                interact(Unit.Get().First(u => (uint)u.ActorId == 0x2B8E3), true);
                Thread.Sleep(500);
                repairAll();
                SnagIt.SellItems();

                walk(2933, 2789, true);
                walk(2969, 2791, true);
                // 0001FD60: 00000002(00000000) # Stash = 0x2DAC507
                interact("Stash", true);
                // stash all rares
                SnagIt.StashItems();
                walk(2977, 2799, true);
                TakePortal();
            }
        }

        override public void Execute() {
            if (Me.LevelArea != SNOLevelArea.Axe_Bad_Data || (uint)Me.LevelArea != 0x163FD || GetDistance(1991, 2653) > 10) {
                if (Game.Ingame) {
                    ExitGame();
                    while (Game.Ingame) Thread.Sleep(383);
                }
                startGame();
                while (!Game.Ingame) Thread.Sleep(412);
                if ((uint)Me.LevelArea != 0x163FD) {
                    Thread.Sleep(5000);
                }
            }

            Thread.Sleep(600);

            repair();

            Thread.Sleep(600);

            walk(1995, 2603, true);
            walk(2025, 2563, true);
            walk(2057, 2528, true);
            walk(2081, 2487, false);
            var cellar = Unit.Get().FirstOrDefault(u => u.ActorId == SNOActorId.g_Portal_Square_Blue && u.Name.Contains("Dank Cellar"));
            if (cellar == default(Unit)) {
                ExitGame();
                while (Game.Ingame) Thread.Sleep(527);
                return;
            }
            walk(2081, 2487, true);
            walk(2066, 2477, true);

            interact(cellar, true);

            repair(); // first one is too quickly after joining sometimes

            walk(108, 158, true);
            walk(129, 143, true);
            // ranged chars will want to attack from here. Sarkoth will stand still as long as the toon does not enter the room
            walk(120, 109, false);
            killAll();
            SnagIt.SnagItems();
            Thread.Sleep(500);
            killAll(); //?!
            SnagIt.SnagItems();
            ExitGame();
            while (Game.Ingame) Thread.Sleep(383);
        }
    }
}
