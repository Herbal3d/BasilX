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
using System.Collections.Generic;
using System.Text;
using System.Threading;

using org.herbal3d.transport;
using org.herbal3d.BasilX.Util;

namespace org.herbal3d.BasilX {
    class BasilXComm {
        private readonly static string _logHeader = "[BasilXComm]";

        private BasilXContext _context;

        public BasilXComm(BasilXContext pContext) {
            _context = pContext;
        }

        public void Start(CancellationTokenSource pCanceller) {
        }
    }
}
