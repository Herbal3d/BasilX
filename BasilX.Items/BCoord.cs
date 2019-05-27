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

namespace org.herbal3d.BasilX.Items {
    public enum CoordSystem {
        WSG86 = 0,      // WSG84 earch coordinates
        CAMERA,         // relative to camera frame (-1..1, zero center)
        CAMERAABS,      // abs coords relative to camera position
        VIRTUAL,        // zero based un-rooted coordinates
        MOON,           // Earth moon coordinates
        MARS,           // Mars coordinates
        REL1,           // Mutually agreed base coordinates
        REL2,
        REL3
    }
    public enum RotationSystem {
        WORLDR = 0,     // world relative
        FORR,           // frame-of-referene relative
        CAMERAR         // camera relative
    }
    
    // Class to hold a bunch of coordinate calculation routimes
    public class BCoord {
    }
}
