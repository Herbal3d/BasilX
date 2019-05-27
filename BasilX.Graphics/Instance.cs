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
    public abstract class Instance : BItem {
        static readonly string _logHeader = "[Instance]";

        public enum InstanceType {
            UNKNOWN,
            meshset
        };

        public OMV.Vector3 GPos = new OMV.Vector3();
        public OMV.Quaternion GRot = new OMV.Quaternion();
        public CoordSystem GPosCoordSystem = 0;
        public RotationSystem GRotCoordSystem = 0;

        protected Displayable _displayable;   // the displayable for this instance
        protected InstanceType _type;

        public Instance(string pId, BAuth pAuth, Displayable pDisplayable)
                                : base(pId, pAuth) {
            _displayable = pDisplayable;
            _type = InstanceType.UNKNOWN;

            DefineProperties(new List<PropertyDefnBase>() {
                new PropertyDefn<InstanceType>("_InstanceType") {
                    getter = () => { return _type; }
                },
                new PropertyDefn<BItemState>("_DisplayableState") {
                    getter = () => { return _displayable.State; }
                },
                new PropertyDefn<OMV.Vector3>("_Position") {
                    getter = () => { return GPos; }
                },
                new PropertyDefn<OMV.Quaternion>("_Rotation") {
                    getter = () => { return GRot; }
                },
            });
        }

        public override void ReleaseResources() {
            base.ReleaseResources();
        }

        // Wait until our underlying displayable is READY and then place an
        //    instance of it into the world.
        public async void PlaceInWorld() {
            if (_displayable != null) {
                try {
                    await _displayable.WhenReady();
                    _displayable.Graphics.PlaceInWorld(this);
                }
                catch (Exception e) {
                    BasilXContext.Instance.log.ErrorFormat("{0} PlaceInWorld exception: {1}", _logHeader, e);
                    SetFailed();
                }
            }
        }

        // Remove my instance from the displayed world
        public void RemoveFromWorld() {
            if (_displayable != null) {
                _displayable.Graphics.RemoveFromWorld(this);
            }
        }
    }
}
