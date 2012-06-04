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
using rndWalker.Bots;

namespace rndWalker.Classes {
    public static class Monk {
        public static Spell mantraOfHealing = new Spell(SNOPowerId.Monk_MantraOfHealing,  50, 0);
        public static Spell mantraOfEvasion = new Spell(SNOPowerId.Monk_MantraOfEvasion,  50, 0); 
        public static Spell mantraOfConviction = new Spell(SNOPowerId.Monk_MantraOfConviction,  50, 0);

        public static Spell blindingFlash = new Spell(SNOPowerId.Monk_BlindingFlash,  10, 0);
        public static Spell breathOfHeaven = new Spell(SNOPowerId.Monk_BreathOfHeaven,  25, 0);
        public static Spell serenity = new Spell(SNOPowerId.Monk_Serenity,  10, 0);
        public static Spell sevenSidedStrike = new Spell(SNOPowerId.Monk_SevenSidedStrike,  50, 0);
        public static Spell wayOfTheHundredFists = new Spell(SNOPowerId.Monk_WayOfTheHundredFists,  0, 0);
        //public static Spell potion = new Spell(SNOPowerId.Axe_Operate_Gizmo, 30, 0, 0, true);

        public static void drinkPot() {
            // hotfix as this does not work via power
            //1FB50094: E1F43DD874E42728 Root.NormalLayer.game_dialog_backgroundScreenPC.game_potion (Visible: True)
            UIElement.Get(0xE1F43DD874E42728).Click();
        }

        public static bool AttackUnit(Unit _unit, TimeSpan _timeout) {
            if (_unit.Life <= 0) {
                return false;
            }

            TimeSpan startTime = TimeSpan.FromTicks(System.Environment.TickCount);

            if (!UIElement.Get().Any(e => e.Name.Contains("buff Monk_MantraOfHealing")))
                mantraOfHealing.use();
            if (!UIElement.Get().Any(e => e.Name.Contains("buff Monk_MantraOfEvasion")))
                mantraOfEvasion.use();
            if (!UIElement.Get().Any(e => e.Name.Contains("buff Monk_MantraOfConviction")))
                mantraOfConviction.use();

            while (_unit.Life > 0) {
                if (Me.MaxLife - Me.Life > 10000) {
                    breathOfHeaven.use();
                }
                if (1.0 * Me.Life / Me.MaxLife < 0.4) {
                    if (!breathOfHeaven.use() && !serenity.use() && !sevenSidedStrike.use(_unit))
                        if (1.0 * Me.Life / Me.MaxLife < 0.3)
                            drinkPot();
                }

                if (sevenSidedStrike.use(_unit))
                    Thread.Sleep(900);
                else
                    wayOfTheHundredFists.use(_unit);

                blindingFlash.use();

                Thread.Sleep(200);

                if (TimeSpan.FromTicks(System.Environment.TickCount).Subtract(startTime) > _timeout) {
                    return false;
                }
            }

            return true;
        }
    }
}
