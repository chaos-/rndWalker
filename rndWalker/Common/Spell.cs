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

namespace rndWalker.Common {
    public class Spell {
        protected SNOPowerId id;
        protected int primCost = 0;
        protected int secCost = 0;

        public Spell(SNOPowerId _id,int _cost1, int _cost2) {
            id = _id;
            primCost = _cost1;
            secCost = _cost2;
        }


        public bool use(Unit _target) {
            if (Me.PrimaryResource >= primCost && Me.SecondaryResource >= secCost && Me.IsSkillReady(id)) {
                Me.UsePower(id, _target);
                Thread.Sleep(75);
                return true;
            } else {
                return false;
            }
        }

        public bool use(float _x, float _y) {

            if (Me.PrimaryResource >= primCost && Me.SecondaryResource >= secCost && Me.IsSkillReady(id)) {
                Me.UsePower(id, _x, _y, Me.Z);
                Thread.Sleep(75);
                return true;
            } else {
                return false;
            }
        }

        public bool use() {
            if (Me.PrimaryResource >= primCost && Me.SecondaryResource >= secCost && Me.IsSkillReady(id)) {
                Me.UsePower(id);
                Thread.Sleep(75);
                return true;
            } else {
                return false;
            }
        }
    }
}
