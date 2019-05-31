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
using System.Text;
using System.Collections.Generic;
using System.Threading;

using org.herbal3d.cs.CommonEntitiesUtil;

namespace org.herbal3d.BasilX.Util {
    public class BasilXContext {

        // There is only one context
        public static BasilXContext Instance;

        public BasilXContext() {
            BasilXContext.Instance = this;
        }

        // Globals for some things that just are global
        public Params parms;
        public BLogger log;

        // Application wide cancellation flag
        public CancellationTokenSource cancellation;

        public string version;
        // A command is added to the pre-build events that generates BuildDate resource:
        //        echo %date% %time% > "$(ProjectDir)\Resources\BuildDate.txt"
        public string buildDate;
        // A command is added to the pre-build events that generates last commit resource:
        //        git rev-parse HEAD > "$(ProjectDir)\Resources\GitCommit.txt"
        public string gitCommit;

    }

}
