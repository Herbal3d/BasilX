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
using org.herbal3d.BasilX.Util;
using OMV = OpenMetaverse;

namespace org.herbal3d.BasilX.Graphics {
    public class InstanceMeshSet : Instance {

        public InstanceMeshSet(string pId, BAuth pAuth, Displayable pDisplayable)
                                    : base(pId, pAuth, pDisplayable) {

            _type = InstanceType.meshset;

            DefineProperties(new List<PropertyDefnBase>() {
                new PropertyDefn<OMV.Vector3>("_Position") {
                    getter = () => { return GPos; },
                    setter = (val) => { GPos = val;
                        // TODO: Update position in the displayed version
                    }
                },
                new PropertyDefn<OMV.Quaternion>("_Rotation") {
                    getter = () => { return GRot; },
                    setter = (val) => { GRot = val;
                        // TODO: Update rotation in the displayed version
                    }
                },
                new PropertyDefn<CoordSystem>("_PosCoordSystem") {
                    getter = () => { return GPosCoordSystem; },
                    setter = (val) => { GPosCoordSystem = val;
                        // TODO: Update coordSystem in the displayed version
                    }
                },
                new PropertyDefn<RotationSystem>("_RotCoordSystem") {
                    getter = () => { return GRotCoordSystem; },
                    setter = (val) => { GRotCoordSystem = val;
                        // TODO: Update coordSystem in the displayed version
                    }
                },
            });

            SetReady();
        }

        public override void ReleaseResources() {
            if (_displayable != null) {
                _displayable.Graphics.RemoveFromWorld(this);
            }
            base.ReleaseResources();
        }

    }
}
