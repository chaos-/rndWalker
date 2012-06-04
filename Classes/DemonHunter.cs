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

using rndWalker.Common;
using D3;

namespace rndWalker.Classes {
    public static class DemonHunter {
        public static Spell hungeringArrow = new Spell(SNOPowerId.DemonHunter_HungeringArrow, 0, 0);
        public static Spell impale = new Spell(SNOPowerId.DemonHunter_Impale, 10, 0);

        public static Spell potion = new Spell(SNOPowerId.Axe_Operate_Gizmo, 0, 0);


        public static bool AttackUnit(Unit _unit, TimeSpan _timeout) {
            if (_unit.Life <= 0) {
                return false;
            }

            TimeSpan startTime = TimeSpan.FromTicks(System.Environment.TickCount);

            while (_unit.Life > 0) {

                if (1.0 * Me.Life / Me.MaxLife < 0.4) {
                    potion.use(Unit.Get().First(i => i.Name.Contains("Health Potion") && i.ItemContainer == Container.Inventory));
                }

                if (!impale.use(_unit))
                    hungeringArrow.use(_unit);

                Thread.Sleep(200);

                if (TimeSpan.FromTicks(System.Environment.TickCount).Subtract(startTime) > _timeout) {
                    return false;
                }
            }

            return true;
        }
    }
}
