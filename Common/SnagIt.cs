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
using rndWalker.Bots;

namespace rndWalker.Common {
    public static class SnagIt {
        public static void SnagItems() {
            var items = Unit.Get().Where(x => x.Type == UnitType.Item && x.ItemContainer == Container.Unknown && CheckItemSnag(x));
            items = items.OrderBy(i => Bot.GetDistance(i.X, i.Y));

            foreach (Unit u in items) {
                int count = 0;
                while (u.Valid == true && u.ItemContainer != Container.Inventory && count < 10) {
                    Me.UsePower(u.Type == UnitType.Gizmo || u.Type == UnitType.Item ? SNOPowerId.Axe_Operate_Gizmo : SNOPowerId.Axe_Operate_NPC, u); //Move.Interact(u);
                    Thread.Sleep(300);
                    ++count;
                }
            }
        }

        public static void SalvageItems() {
            var items = Unit.Get().Where(x => x.Type == UnitType.Item && x.ItemContainer == Container.Inventory && CheckItemSalvage(x));

            foreach (Unit u in items) {
                u.SalvageItem();
                Thread.Sleep(200);
            }
        }

        public static void SellItems() {
            var items = Unit.Get().Where(x => x.Type == UnitType.Item && x.ItemContainer == Container.Inventory && CheckItemSell(x));

            foreach (Unit u in items) {
                u.SellItem();
                Thread.Sleep(200);
            }
        }

        public static void StashItems() {
            var items = Unit.Get().Where(i => i.ItemContainer == Container.Inventory && CheckItemStash(i));
            foreach (Unit i in items) {
                var p = i.GetItemFreeSpace(Container.Stash);
                if (p.X == -1 || p.Y == -1) {
                    continue;
                }
                i.MoveItem(Container.Stash, p.X, p.Y);
                Thread.Sleep(670);
            }
        }

        public static bool CheckItemStash(Unit i) {
            return i.ItemQuality >= UnitItemQuality.Rare4
                    || i.Name.Contains("Topaz (30)")  || i.Name.Contains("Amethyst (30)") || i.Name.Contains("Emerald (30)") || i.Name.Contains("Ruby (30)")
                    || i.Name.Contains("Essence (100)") || i.Name.Contains("Tear (100)") || i.Name.Contains("Hoof (100)") || i.Name.Contains("Plan") ;
        }

        public static bool CheckItemSnag(Unit unit) {
            return unit.ActorId == SNOActorId.GoldCoins
                || unit.ActorId == SNOActorId.GoldLarge
                || unit.ActorId == SNOActorId.GoldMedium
                || unit.ActorId == SNOActorId.GoldSmall
                || unit.Name.Contains("Topaz") // gems
                || unit.Name.Contains("Amethyst")
                || unit.Name.Contains("Emerald")
                || unit.Name.Contains("Ruby")
                || unit.Name.Contains("Book ") // crafting materials
                || unit.Name.Contains("Tome")
                || unit.Name.Contains("Plan")
                || unit.Name.Contains("Mythic") // Health potions
                || unit.ItemQuality >= UnitItemQuality.Magic1;
        }

        public static bool CheckItemSalvage(Unit unit) {
            return !unit.Name.Contains("Book ") // crafting materials
                && !unit.Name.Contains("Tome")
                && !unit.Name.Contains("Plan")
                && !unit.Name.Contains("Essence")
                && !unit.Name.Contains("Tear")
                && !unit.Name.Contains("Hoof")
                && !unit.Name.Contains("Brimstone")
                && unit.ItemQuality < UnitItemQuality.Rare4
                && unit.ItemQuality >= UnitItemQuality.Magic1
                && unit.ItemLevelRequirement == 60;
        }

        public static bool CheckItemSell(Unit unit) {
            return !unit.Name.Contains("Book ") // crafting materials
                && !unit.Name.Contains("Tome")
                && !unit.Name.Contains("Plan")
                && !unit.Name.Contains("Essence")
                && !unit.Name.Contains("Tear")
                && !unit.Name.Contains("Hoof")
                && !unit.Name.Contains("Brimstone")
                && unit.ItemQuality < UnitItemQuality.Rare4
                && unit.ItemQuality >= UnitItemQuality.Magic1
                && unit.ItemLevelRequirement < 60;
        }
    }
}
