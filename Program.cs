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
using System.Diagnostics;

namespace rndWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // use only the first core to fix near-deadlocks of parallel access to d3 vars
                Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)1; 
                AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
                rndWalker.Instance = new rndWalker(args);
                rndWalker.Instance.Start();
            }
            catch (Exception e)
            {
                D3.Game.Print("{c:FFFF0000}"+e.ToString()+"{/c}");
            }
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs ea)
        {
            try
            {
                rndWalker.Instance.Stop();
            }
            catch (Exception e)
            {
                D3.Game.Print("{c:FFFF0000}" + e.ToString() + "{/c}");
            }
        }
    }
}
