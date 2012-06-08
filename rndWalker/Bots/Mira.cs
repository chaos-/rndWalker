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
using System.IO;
using System.Xml.Serialization;

using D3;

using rndWalker.Common;

namespace rndWalker.Bots {
    class MiraBot : Bot {
        public MiraBot() {
        }

        override public void Execute() {
            Console.WriteLine("Now Running `MiraBot`");

            // changeQuest(3); works, but is slow
            // 2437D89C: F60E9296FAEC1B2C Root.NormalLayer.BattleNetQuestSelection_main.LayoutRoot.OverlayContainer.QuestMenu.NavigationMenuList._content._stackpanel._item2 (Visible: True)
            // 24383F34: F60E9396FAEC1CDF Root.NormalLayer.BattleNetQuestSelection_main.LayoutRoot.OverlayContainer.QuestMenu.NavigationMenuList._content._stackpanel._item3 (Visible: True)

            while ((uint)Me.QuestId != 0x011A1D || Me.QuestStep != -1) {
                changeQuest(2, 0xF60E9296FAEC1B2C);
                Thread.Sleep(1000);
                startGame();
            }
            
            // ---------------------------------------------------------
            //    long walk....
            // ---------------------------------------------------------
            walk(2965, 2833, true);
            walk(2950, 2800, false);

            interact("Deckard Cain", true);
            walk(2974, 2797, true);
            walk(3003, 2795, true);
            walk(3031, 2771, true);
            walk(3016, 2730, true);
            walk(2974, 2709, true);
            walk(2950, 2666, true);
            walk(2945, 2637, true);
            walk(2940, 2616, false);
            var haedrig = Unit.Get().Where(x => (uint)x.ActorId == 0xFE0C).FirstOrDefault();
            interact(haedrig, true);
            skipConversation();

            // ---------------------------------------------------------
            //    repair at radek
            // ---------------------------------------------------------
            if (needsRepair()) {
                //  0002B8D8: 00000001(00000000) # {c:ffffffff}Radek the Fence{/c} = 0x1BB715
                var radek = Unit.Get().Where(x => (uint)x.ActorId == 0x0002B8D8).FirstOrDefault();
                interact(radek, true);
                Thread.Sleep(300);
                repairAll();
                Thread.Sleep(100);
                closeInventory();
                //sell?!
            }
            walk(2899, 2591, true);
            walk(2859, 2596, true);
            walk(2827, 2605, true);
            walk(2798, 2619, true);

            // ---------------------------------------------------------
            //    enter the cellar
            // ---------------------------------------------------------
            //var portal = waitForUnit("Cellar of the Damned", 30);
            Thread.Sleep(1000);
            interact("Cellar of the Damned", true);
            waitForArea(0x144A6);
            walk(176, 88, true);
            walk(177, 120, true);
            walk(171, 152, true);

            // ---------------------------------------------------------
            //    destroy the door (replace command below if not melee)
            // ---------------------------------------------------------
            //var door = Unit.Get().Where(x => x.Name.Contains("Sturdy Boarded Door")).ToArray();
            var door = Unit.Get().Where(x => x.ActorId == SNOActorId.trDun_Blacksmith_CellarDoor_Breakable).FirstOrDefault();
            walk(door.X, door.Y, false);
            Thread.Sleep(500);
            Me.UsePower(SNOPowerId.Weapon_Melee_Instant, door);
            //Me.UsePower(SNOPowerId.Weapon_Ranged_Projectile, door);
            Thread.Sleep(600);
            walk(143, 144, false);
            waitForMobs(10);
            killAll();
            if (Me.X > 110) {
                walk(110, 148, true);
                killAll();
            }
            walk(70, 145, true);
            killAll();

            // ---------------------------------------------------------
            //    wait for mira
            // ---------------------------------------------------------
            var mira = Unit.Get().Where(x => x.ActorId == SNOActorId.ZombieFemale_A_BlacksmithA).ToArray();
            while (mira.Length <= 0) {
                Thread.Sleep(500);
                mira = Unit.Get().Where(x => x.ActorId == SNOActorId.ZombieFemale_A_BlacksmithA).ToArray();
                if (GetDistance(70, 145) > 12)
                    walk(60, 145, true);
            }
            killThese(mira);
            Thread.Sleep(500);
            SnagIt.SnagItems();
            SnagIt.SnagItems();

            // ---------------------------------------------------------
            //    finish quest step and quit game
            // ---------------------------------------------------------
            interact("Haedrig", true);
            skipConversation();
            while (Me.QuestStep != 35)
                Thread.Sleep(500);
            Thread.Sleep(1000);
            ExitGame();
        }
    }
}
