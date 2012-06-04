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

namespace rndWalker.Bots
{
    class RoyalCryptsMinions : Bot
    {
        public RoyalCryptsMinions()
        {
        }

        override public void Execute()
        {
            startGame();

            if (Me.LevelArea != SNOLevelArea.A1_trDun_Level07B)
            {
                //throw new Exception("Toon needs to start in 'The Royal Crypts'!");
                return;
            }

            walk(345, 503, true);
            interact("Ornate Door", true);

            // find some skels
            var ironDoor = Unit.Get().FirstOrDefault(x => x.Type == UnitType.Gizmo && x.Name.Contains("Iron Gate") && GetDistance(Me.X, Me.Y, x.X, x.Y) < 100);
            interact(ironDoor,true);

            walk(293f, 500f, true);

            // kill skeletons
            var skels = waitForMobs(30);
            //Game.Print(string.Format("found {0} skels",skels.Length)); //dbg msg

            killAll();

            SnagIt.SnagItems();

            walk(321f, 522f, true);
            walk(345, 496, true);

            interact("Crypt of the Skeleton King",true);

            waitForArea(0x4D4D);
            Thread.Sleep(587);
            SkipSequence();

            walk(316, 427, true);
            walk(284, 430, true);
            walk(250, 428, true);

            waitForMobs(30);
            killAll();

            SnagIt.SnagItems();

            ExitGame();
            //Thread.Sleep(10000);
        }
    }
}
