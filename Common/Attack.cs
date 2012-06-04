/*
 *  Copyright (C) 2012 k_os <ben.at.hemio.de>, rolle3k
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

using D3;

namespace rndWalker.Common
{
    public static class Attack
    {
        private static TimeSpan defaultTimeout = TimeSpan.FromSeconds(20);

        public delegate bool UnitCheckCallback(Unit unit);

        static public bool AttackUnit(Unit unit, TimeSpan timeout)
        {
            if (unit.Type == UnitType.Monster
                && unit.GetAttributeInteger(UnitAttribute.Is_NPC) == 0
                && unit.GetAttributeInteger(UnitAttribute.Is_Helper) == 0
                && unit.GetAttributeInteger(UnitAttribute.Invulnerable) == 0)
            {
                switch(Me.SNOId)
                {
                    case SNOActorId.Barbarian_Male:
                    case SNOActorId.Barbarian_Female:
                        return Classes.Barbarian.AttackUnit(unit, timeout);
                    case SNOActorId.WitchDoctor_Male:
                    case SNOActorId.WitchDoctor_Female:
                        return Classes.WitchDoctor.AttackUnit(unit, timeout);
                    case SNOActorId.Wizard_Male:
                    case SNOActorId.Wizard_Female:
                        return Classes.Wizard.AttackUnit(unit, timeout);
                    case SNOActorId.Demonhunter_Male:
                    case SNOActorId.Demonhunter_Female:
                        return Classes.DemonHunter.AttackUnit(unit, timeout);
                    case SNOActorId.Monk_Male:
                    case SNOActorId.Monk_Female:
                        return Classes.Monk.AttackUnit(unit, timeout);
                    default:
                        break;
                }
            }
            return false;
        }

        static public bool AttackUnit(Unit unit)
        {
            return AttackUnit(unit, defaultTimeout);
        }
    }
}
