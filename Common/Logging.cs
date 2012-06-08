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
using System.Reflection;
using System.IO;

namespace rndWalker.Common {
    public class Logger {
        public string Name { get; set; }

        private string path;

        public Logger(string name) {
            this.Name = name;

            this.path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logs";

            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            this.path += "\\" + DateTime.Today.ToShortDateString() + ".txt";
        }

        public void Log(string message) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}] {1}: {2}{3}", DateTime.Now.ToShortTimeString(), this.Name, message, System.Environment.NewLine);
            File.AppendAllText(this.path, sb.ToString());

            Console.Write(sb.ToString());
        }

        public void Log(string message, params object[] args) {
            this.Log(String.Format(message, args));
        }

        public void Log(Exception e) {
            this.Log(String.Format("{0}{1}", e.ToString(), System.Environment.NewLine));
        }

        public void Log(Exception e, string message) {
            this.Log(String.Format("{0}{1}{2}{1}", message, System.Environment.NewLine, e.ToString()));
        }

        public void Log(Exception e, string format, params object[] args) {
            this.Log(e, String.Format(format, args));
        }
    }
}
