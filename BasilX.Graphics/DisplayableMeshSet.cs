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

using org.herbal3d.BasilX.Items;

namespace org.herbal3d.BasilX.Graphics {
    public class DisplayableMeshSet : Displayable {
        public DisplayableMeshSet(string pId, BAuth pAuth, IGraphics pGraphics, BDisplayInfo pDispInfo)
                    : base(pId, pAuth, pGraphics) {
            DType = DisplayableType.meshset;

        }

        public override void ReleaseResources() {
            base.ReleaseResources();
        }
    }
}
