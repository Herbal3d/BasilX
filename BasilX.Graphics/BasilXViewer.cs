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

#define USEGODOT

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using org.herbal3d.BasilX.Util;

#if USEGODOT
using Godot;
#endif

namespace org.herbal3d.BasilX.Graphics {
    public class BasilXViewer {
        private readonly static string _logHeader = "[BasilXViewer]";

        private BasilXContext _context;

        public BasilXViewer(BasilXContext pContext) {
            _context = pContext;
        }

        public Task Start() {
            _context.log.DebugFormat("{0} Start", _logHeader);

#if USEGODOT
            return Task.Run(() => {
                var sceneTree = new SceneTree();
            });
#else
            return Task.Run(() => {
                using (var game = new CodeOnlyGame()) {
                    game.Run();
                }
            });
#endif
        }
    }
}
