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
using System.Reflection;
using System.Threading;

using org.herbal3d.BasilX.Util;

using org.herbal3d.cs.CommonEntitiesUtil;

namespace org.herbal3d.BasilX {
    public class BasilX {
        private static readonly string _logHeader = "[BasilX]";

        private static BasilX _instance;
        protected BasilXContext _context;

        private string Invocation() {
            StringBuilder buff = new StringBuilder();
            buff.AppendLine("Invocation: BasilX <parameters>");
            buff.AppendLine("   Possible parameters are (negate bool parameters by prepending 'no'):");
            string[] paramDescs = _context.parms.ParameterDefinitions.Select(pp => { return pp.ToString(); }).ToArray();
            buff.AppendLine(String.Join(Environment.NewLine, paramDescs));
            return buff.ToString();
        }

        static void Main(string[] args) {
            _instance = new BasilX();
            _instance.Start(args);
            return;
        }

        public void Start(string[] args) {
            // Create an application level 'context' to pass around.
            // This is not for global variables but for utilities needed by all (logging, parameters, ...)
            _context = new BasilXContext() {
                log = new LoggerConsole(),
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                // A command is added to the pre-build events that generates BuildDate resource:
                //        echo %date% %time% > "$(ProjectDir)\Resources\BuildDate.txt"
                buildDate = Properties.Resources.BuildDate.Trim(),
                // A command is added to the pre-build events that generates last commit resource:
                //        git rev-parse HEAD > "$(ProjectDir)\Resources\GitCommit.txt"
                gitCommit = Properties.Resources.GitCommit.Trim()
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

            if (_context.parms.P<bool>(Params.PVerbose)) {
                _context.log.SetVerbose(_context.parms.P<bool>(Params.PVerbose));
            }

            if (!_context.parms.P<bool>(Params.PQuiet)) {
                System.Console.WriteLine("BasilX v" + _context.version
                            + " built " + _context.buildDate
                            + " commit " + _context.gitCommit
                            );
            }

            // Appliction level cancellation
            var canceller = new CancellationTokenSource();

            // Start the graphics system
            var viewer  = new BasilXViewer(_context);
            viewer.Start(canceller);

            // Start the communication system
            var comm = new BasilXComm(_context);
            comm.Start(canceller);

            while (!canceller.IsCancellationRequested) {
                Thread.Sleep(100);
            }
        }
    }
}
