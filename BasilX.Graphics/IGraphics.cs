using System;
using System.Collections.Generic;
using System.Text;

namespace org.herbal3d.BasilX.Graphics {
    public interface IGraphics {
        void PlaceInWorld(Instance pInstance);

        void RemoveFromWorld(Instance pInstance);
    }

}
