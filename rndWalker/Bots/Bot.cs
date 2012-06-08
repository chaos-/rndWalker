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

//TODO this file needs cleanup.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using D3;
using rndWalker.Common;

namespace rndWalker.Bots {
    public abstract class Bot {
        abstract public void Execute();

        // 0002FA62: 00000001(00000000) # {c:ffffffff}Salvage{/c} = 0x1D19D9E

        protected int getInventoryFreeSpace() {
            var items = Unit.Get().Where(i => i.ItemContainer == Container.Inventory);
            int occupiedSlots = 0;
            foreach (Unit i in items) {
                occupiedSlots += i.ItemSizeX * i.ItemSizeY;
            }
            return 6 * 10 - occupiedSlots;
        }

        protected void closeInventory() {
            // 303EF3AC: 368FF8C552241695 Root.NormalLayer.inventory_dialog_mainPage.inventory_button_exit (Visible: True)
            var btn = UIElement.Get(0x368FF8C552241695);
            if (btn.Visible)
                btn.Click();
        }

        protected bool needsRepair() {
            // 1EFC3224: BD8B3C3679D4F4D9 Root.NormalLayer.DurabilityIndicator (Visible: True)
            var indicator = UIElement.Get(0xBD8B3C3679D4F4D9);
            return (indicator != default(UIElement) && indicator.Visible);
        }

        /// <summary>
        /// will wait up to 60 seconds for the game to start. does nothing if already ingame
        /// </summary>
        protected void startGame() {
            // 27DFC2E4: 51A3923949DC80B7 Root.NormalLayer.BattleNetCampaign_main.LayoutRoot.Menu.PlayGameButton (Visible: True)
            // it's both the resume and the start game button
            while (Game.Ingame == false) {
                UIElement resumeGame = UIElement.Get(0x51A3923949DC80B7);

                if (resumeGame != default(UIElement)) {
                    resumeGame.Click();

                    for (int i = 0; i < 20 && Game.Ingame == false; ++i) {
                        Thread.Sleep(1000);
                    }
                }
            }

            Thread.Sleep(2000);
        }

        /// <summary>
        /// revives if dead, returns immediately otherwise
        /// </summary>
        protected void revive() {
            if (Me.Life > 0)
                return;

            // 20DDD3F4: BFAAF48BA9316742 Root.NormalLayer.deathmenu_dialog.dialog_main.button_revive_at_checkpoint (Visible: True)
            var btn = UIElement.Get(0xBFAAF48BA9316742);
            
            // wait for button to appear
            while (btn == default(UIElement) || !btn.Visible) {
                Thread.Sleep(300);
                btn = UIElement.Get(0xBFAAF48BA9316742);
            }
            // no need to wait for the cd, button can be clicked even if while a human cannot
            btn.Click();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// buy window needs to be open. repair tab needs not be selected though
        /// </summary>
        protected void repairAll() {
            // 1EFC12BC: 80F5D06A035848A5 Root.NormalLayer.shop_dialog_mainPage.repair_dialog.RepairAll (Visible: True)
            var btn = UIElement.Get(0x80F5D06A035848A5);
            if (btn != default(UIElement)) {
                btn.Click();
            }
        }

        /// <summary>
        /// skips conversation pc with npc
        /// </summary>
        protected void skipConversation() {
            // 1F05DE84: 942F41B6B5346714 Root.NormalLayer.conversation_dialog_main.button_close (Visible: True)
            var advKey = UIElement.Get(0x942F41B6B5346714);
            while (advKey.Visible) {
                advKey.Click();
                Thread.Sleep(45);
            }
        }

        /*// <summary>
        /// dont use! buggy
        /// </summary>
        protected void skipNpcConversation() {
            // 2D3E702C: C06278A08ADCF3AA Root.NormalLayer.playinghotkeys_dialog_backgroundScreen.playinghotkeys_conversation_advance (Visible: True)
            var advKey = UIElement.Get(0xC06278A08ADCF3AA);
            while (advKey.Visible) {
                advKey.Click();
                Thread.Sleep(45);
            }
        }*/

        /// <summary>
        /// changes quest to the one with the given index. may be slow so try to give the button hash in advance
        /// </summary>
        /// <param name="_listIndex"></param>
        protected void changeQuest(uint _listIndex) {

            changeQuest(_listIndex, 0);
        }

        protected void changeQuest(uint _listIndex, ulong _btn) {
            if (Game.Ingame) {
                ExitGame();
                while (Game.Ingame) {
                    Thread.Sleep(687);
                }
            }

            // 23E844FC: C4A9CC94C0A929B Root.NormalLayer.BattleNetCampaign_main.LayoutRoot.Menu.ChangeQuestButton (Visible: True)
            UIElement changeQuest = UIElement.Get(0xC4A9CC94C0A929B);
            changeQuest.Click();
            Thread.Sleep(469);

            var button = default(UIElement);

            if (_btn == 0) {
                button = UIElement.Get().Where(x => x.Name.EndsWith(string.Format("_item{0}", _listIndex))).FirstOrDefault();
            } else {
                button = UIElement.Get(_btn);
            }
            button.Click();
            Thread.Sleep(478);


            // 2BBDBAC4: 1AE2209980AAEA69 Root.NormalLayer.BattleNetQuestSelection_main.LayoutRoot.OverlayContainer.SelectQuestButton
            UIElement.Get(0x1AE2209980AAEA69).Click(); // select quest
            Thread.Sleep(730);

            // 2440DBEC: B4433DA3F648A992 Root.TopLayer.BattleNetModalNotifications_main.ModalNotification.Buttons.ButtonList.OkButton (Visible: True)
            var ok = UIElement.Get(0xB4433DA3F648A992);
            if (ok != null && ok.Visible) { // unfortunately this is always true...
                ok.Click();
            }

            Thread.Sleep(1000);
        }

        public static void ExitGame() {
            UIElement ui = UIElement.Get(0x5DB09161C4D6B4C6);

            if (ui != null) {
                ui.Click();
            }
        }

        protected void SkipSequence() {
            UIElement skipSequence = UIElement.Get(0x2289FE26DA955A81);
            UIElement confirmButton = UIElement.Get(0x891D21408238D18E);

            for (int i = 0; i < 10 && skipSequence.Visible == false; i++) {
                Thread.Sleep(500);
            }

            if (skipSequence.Visible == false) {
                throw new Exception("Skip Sequence UIElement is not visible!");
            }

            skipSequence.Click();

            for (int i = 0; i < 10 && confirmButton.Visible == false; i++) {
                Thread.Sleep(500);
            }

            if (skipSequence.Visible == false) {
                throw new Exception("Confirm Button UIElement is not visible!");
            }

            confirmButton.Click();

            for (int i = 0; i < 10 && confirmButton.Visible == true; i++) {
                Thread.Sleep(500);
            }

            if (confirmButton.Visible == true) {
                throw new Exception("Confirm Button UIElement is still visible!");
            }

            // Wait out sequence post effect..
            Thread.Sleep(5500);
        }

        protected bool walk(float _x, float _y) {
            return walk(_x, _y, false, 0);
        }

        /// <summary>
        /// walk to given (x,y) position.
        /// </summary>
        /// <param name="_x">X</param>
        /// <param name="_y">Y</param>
        /// <param name="_waitTillThere">whether to wait until toon is there</param>
        /// <returns></returns>
        protected bool walk(float _x, float _y, bool _waitTillThere) {
            return walk(_x, _y, _waitTillThere, 3);
        }

        protected bool walk(float _x, float _y, bool _waitTillThere, uint _countOut) {
            Me.UsePower(SNOPowerId.Walk, _x, _y, Me.Z);
            uint count = 0;
            if (_waitTillThere) {
                while (GetDistance(_x, _y) > 7) {
                    if (Me.Mode != UnitMode.Running) {
                        Me.UsePower(SNOPowerId.Walk, _x, _y, Me.Z);
                        if (count++ >= _countOut) {
                            return false;
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            return true;
        }

        protected void interact(Unit _u, bool _walkThere) {
            if (_u.Type == UnitType.Gizmo
                || _u.Type == UnitType.Monster
                || _u.Type == UnitType.Item) {
                if (_walkThere) {
                    walk(_u.X, _u.Y, true);
                    Thread.Sleep(300);
                }
                Me.UsePower(_u.Type == UnitType.Monster ? SNOPowerId.Axe_Operate_NPC : SNOPowerId.Axe_Operate_Gizmo, _u);
                if (_walkThere) {
                    Thread.Sleep(700);
                }
            }
        }

        protected void interact(string _name, bool _walkThere) {
            var unit = Unit.Get().FirstOrDefault(x => x.Name.Contains(_name));
            interact(unit, _walkThere);
            Thread.Sleep(213);
        }

        protected void clickUI(string _name, bool _visible) {
            var e = UIElement.Get().FirstOrDefault(x => x.Name.Contains(_name));
            e.Click();
        }

        /// <summary>
        /// waypoint menu needs to be open. if unsure use 'interact("Waypoint");' before this
        /// </summary>
        /// <param name="_listpos"></param>
        protected void useWaypoint(uint _listpos) {
            clickUI(string.Format("{0}.wrapper.button", _listpos), true);
        }

        protected bool GoTown() {
            if (Me.InTown == true) { return true; }

            if (Me.UsePower(SNOPowerId.UseStoneOfRecall) == true) {
                Thread.Sleep(550);

                while (Me.Mode == UnitMode.Casting
                        || Me.Mode == UnitMode.Warping) {
                    Thread.Sleep(250);
                }

                if (Me.InTown == true) {
                    return true;
                }
            }
            return false;
        }

        protected bool TakePortal() {
            if (Me.InTown == false) { return false; }

            Unit[] units = Unit.Get();

            var unit = (from u in units where u.Type == UnitType.Gizmo && u.ActorId == SNOActorId.hearthPortal select u).FirstOrDefault();

            if (unit == default(Unit)) {
                return false;
            }

            for (int i = 0; i < 3; i++) {
                Me.UsePower(SNOPowerId.Axe_Operate_Gizmo, unit);
                Thread.Sleep(500);

                if (Me.InTown == false) {
                    break;
                }
            }

            return Me.InTown == false;
        }


        protected Unit[] waitForMobs(uint _timeout) {
            var mobs = Unit.Get().Where(x => x.Type == UnitType.Monster && ((uint)x.MonsterType == 0 || (uint)x.MonsterType == 4) && x.Mode != UnitMode.Warping && x.Life > 0
                && x.GetAttributeInteger(UnitAttribute.Is_NPC) == 0 && x.GetAttributeInteger(UnitAttribute.Is_Helper) == 0
                && x.GetAttributeInteger(UnitAttribute.Invulnerable) == 0).ToArray();
            uint count = 0;
            while (mobs.Length <= 0 && count < 2 * _timeout) {
                Thread.Sleep(500);
                mobs = Unit.Get().Where(x => x.Type == UnitType.Monster && ((uint)x.MonsterType == 0 || (uint)x.MonsterType == 4) && x.Mode != UnitMode.Warping && x.Life > 0
                    && x.GetAttributeInteger(UnitAttribute.Is_NPC) == 0 && x.GetAttributeInteger(UnitAttribute.Is_Helper) == 0
                    && x.GetAttributeInteger(UnitAttribute.Invulnerable) == 0).ToArray();
                ++count;
            }
            return mobs;
        }

        /// <summary>
        /// buggy?
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_timeout"></param>
        /// <returns></returns>
        protected Unit waitForUnit(string _name, uint _timeout) {
            var portal = Unit.Get().Where(x =>/* x.Type == UnitType.Gizmo && */x.Name.Contains(_name)).ToArray();

            uint count = 0;
            while (portal.Length <= 0 && count < 2 * _timeout) {
                Thread.Sleep(500);
                portal = Unit.Get().Where(x =>/* x.Type == UnitType.Gizmo && */x.Name.Contains(_name)).ToArray();
                ++count;

            }
            return portal.FirstOrDefault();
        }

        protected void waitForArea(uint _lvlArea) {
            while ((uint)Me.LevelArea != _lvlArea) {
                Thread.Sleep(567);
            }
        }

        protected void killThese(Unit[] _units) {
            //_units = _units.OrderBy(u1 => GetDistance(u1.X, u1.Y, Me.X, Me.Y)).ToArray();
            while (_units.Any()) {
                _units = _units.Where(u => u.Valid).ToArray();
                if (!_units.Any())
                    break;
                _units = _units.OrderBy(u1 => GetDistance(u1.X, u1.Y, Me.X, Me.Y)).ToArray();
                if (_units[0].Valid && _units[0].Life > 0) {
                    Attack.AttackUnit(_units[0]);
                    Thread.Sleep(154);
                }
            }
        }

        protected void killAll() {
            var mobs = waitForMobs(0);
            while (mobs.Length > 0) {
                killThese(mobs);
                mobs = waitForMobs(0);
            }
        }

        public static float GetDistance(float x, float y) {
            return GetDistance(x, y, Me.X, Me.Y);
        }

        public static float GetDistance(float x, float y, float x2, float y2) {
            return (float)Math.Sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2));
        }
    }
}
