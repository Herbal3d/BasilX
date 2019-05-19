// Copyright 2019 Robert Adams
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using org.herbal3d.cs.CommonEntitiesUtil;

namespace org.herbal3d.BasilX {
    public class BasilXContext {
        // Globals for some things that just are global
        public Params parms;
        public BLogger log;

        public string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        // A command is added to the pre-build events that generates BuildDate resource:
        //        echo %date% %time% > "$(ProjectDir)\Resources\BuildDate.txt"
        public string buildDate = Properties.Resources.BuildDate.Trim();
        // A command is added to the pre-build events that generates last commit resource:
        //        git rev-parse HEAD > "$(ProjectDir)\Resources\GitCommit.txt"
        public string gitCommit = Properties.Resources.GitCommit.Trim();

    }

    public class BasilX {
        private static readonly string _logHeader = "[BasilX]";

        private BasilXContext _context;

        private string Invocation() {
            StringBuilder buff = new StringBuilder();
            buff.AppendLine("Invocation: BasilX <parameters>");
            buff.AppendLine("   Possible parameters are (negate bool parameters by prepending 'no'):");
            string[] paramDescs = _context.parms.ParameterDefinitions.Select(pp => { return pp.ToString(); }).ToArray();
            buff.AppendLine(String.Join(Environment.NewLine, paramDescs));
            return buff.ToString();
        }

        static void Main(string[] args) {
            BasilX basilTest = new BasilX();
            basilTest.Start(args);
            return;
        }

        public void Start(string[] args) {
            _context = new BasilXContext {
                log = new LoggerConsole()
            };
            _context.parms = new Params(_context);

            // A single parameter of '--help' outputs the invocation parameters
            if (args.Length > 0 && args[0] == "--help") {
                System.Console.Write(Invocation());
                return;
            }

            // 'Params' initializes to default values.
            // Override default values with command line parameters.
            try {
                // Note that trailing parameters will be put into "Extras" parameter
                _context.parms.MergeCommandLine(args, null, "Extras");
            }
            catch (Exception e) {
                _context.log.ErrorFormat("ERROR: bad parameters: " + e.Message);
                _context.log.ErrorFormat(Invocation());
                return;
            }

            if (_context.parms.P<bool>("Verbose")) {
                _context.log.SetVerbose(_context.parms.P<bool>("Verbose"));
            }

            if (!_context.parms.P<bool>("Quiet")) {
                System.Console.WriteLine("BasilX v" + _context.version
                            + " built " + _context.buildDate
                            + " commit " + _context.gitCommit
                            );
            }

            var viewer  = new BasilXViewer(_context);
            var canceller = new CancellationTokenSource();
            viewer.Start(canceller);

            while (!canceller.IsCancellationRequested) {
                Thread.Sleep(100);
            }
        }
    }
}
