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

namespace rndWalker.Classes
{
    public static class Wizard
    {
        public static Spell DiamondSkin = new Spell(SNOPowerId.Wizard_DiamondSkin, 15, 0);
        public static Spell Blizzard = new Spell(SNOPowerId.Wizard_Blizzard, 20, 0);
        public static Spell ShockPulse = new Spell(SNOPowerId.Wizard_ShockPulse, 0, 0);
        public static Spell MagicWeapon = new Spell(SNOPowerId.Wizard_MagicWeapon, 25, 0);
        public static Spell EnergyArmor = new Spell(SNOPowerId.Wizard_EnergyArmor, 25, 0);
        public static Spell Hydra = new Spell(SNOPowerId.Wizard_Hydra, 15, 0);
        public static Spell ArcaneOrb = new Spell(SNOPowerId.Wizard_ArcaneOrb, 20, 0);
        public static Spell Teleport = new Spell(SNOPowerId.Wizard_Teleport, 15, 0);

        public static bool AttackUnit(Unit unit, TimeSpan timeout)
        {
            if (unit.Life <= 0)
            {
                return false;
            }

            var startTime = TimeSpan.FromTicks(Environment.TickCount);


            while (unit.Life > 0)
            {
                //if(Hydra.TimeSinceUsedInSeconds() > 15)
                if(!IsHydraAlive())
                    Hydra.use(unit);

                if(!ArcaneOrb.use(unit))
                    ShockPulse.use(unit);

                if (1.0 * Me.Life / Me.MaxLife < 0.3)
                {
                    ShockPulse.use();
                }

                if (1.0 * Me.Life / Me.MaxLife < 0.25)
                {
                    UIElement.Get(0xE1F43DD874E42728).Click();
                }

                Thread.Sleep(130);

                if (TimeSpan.FromTicks(Environment.TickCount).Subtract(startTime) > timeout)
                {
                    return false;
                }
            }

            return true;

        }

        public static bool IsHydraAlive()
        {
            var units = Unit.Get().Where(x => x.Name.Contains("Hydra") || x.AnimationId == SNOAnimId.Wizard_hydraHead_1_attack_03_cone || x.AnimationId == SNOAnimId.Wizard_hydraHead_2_attack_03_cone);
            return units.Any();
        }

        public static void CastSelfSpells()
        {
            if(EnergyArmor.use())
                Thread.Sleep(50);
            if(MagicWeapon.use())
                Thread.Sleep(50);
            DiamondSkin.use();
        }
    }
}
