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

using BasilType = org.herbal3d.basil.protocol.BasilType;
using OMV = OpenMetaverse;

namespace org.herbal3d.BasilX.Items {

    // Rather than passing the lower level communication data structure,
    //    this wraps the contents for the rest of BasilX.
    public class BDisplayInfo {

        public OMV.Vector3 AabbUpperFrontLeft;
        public OMV.Vector3 AabbLowerBackRight;
        public string DisplayableType;  // string version of Displayable.DisplayableType
        public Dictionary<string, string> AssetAttributes = new Dictionary<string, string>();

        public BDisplayInfo() {
            DisplayableType = "UNKNOWN";
        }

        // Return a converted version of this info suitable for the Basil protocol.
        public BasilType.DisplayableInfo ToMessage() {
            BasilType.DisplayableInfo ret = new BasilType.DisplayableInfo();
            if (AabbUpperFrontLeft != null && AabbLowerBackRight != null) {
                ret.Aabb = new BasilType.AaBoundingBox() {
                    UpperFrontLeft = new BasilType.Vector3() {
                        X = AabbUpperFrontLeft.X,
                        Y = AabbUpperFrontLeft.Y,
                        Z = AabbUpperFrontLeft.Z,
                    },
                    LowerBackRight = new BasilType.Vector3() {
                        X = AabbLowerBackRight.X,
                        Y = AabbLowerBackRight.Y,
                        Z = AabbLowerBackRight.Z,
                    }
                };
            }
            if (!String.IsNullOrEmpty(DisplayableType)) {
                ret.DisplayableType = DisplayableType;
            }
            if (AssetAttributes != null && AssetAttributes.Count > 0) {
                ret.Asset.Add(AssetAttributes);
            }
            return ret;
        }

    }
}
