using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_to_FCE
{
    public class CubeData
    {
        private UInt16 _cubeType;
        private String _name;
        private Boolean _isSolid;
        private Boolean _isTransparent;
        private Boolean _isHollow;
        private Boolean _isGlass;
        private Boolean _isPassable;

        public UInt16 CubeType
        {
            get
            {
                return _cubeType;
            }
        }
        public String Name
        {
            get
            {
                return _name;
            }
        }
        public Boolean IsSolid
        {
            get
            {
                return _isSolid;
            }
        }
        public Boolean IsTransparent
        {
            get
            {
                return _isTransparent;
            }
        }
        public Boolean IsHollow
        {
            get
            {
                return _isHollow;
            }
        }
        public Boolean IsGlass
        {
            get
            {
                return _isGlass;
            }
        }
        public Boolean IsPassable
        {
            get
            {
                return _isPassable;
            }
        }

        public CubeData(UInt16 cubeType, String name, Boolean isSolid, Boolean isTransparent, Boolean isHollow, Boolean isGlass, Boolean isPassable)
        {
            _cubeType = cubeType;
            _name = name;
            _isSolid = isSolid;
            _isTransparent = isTransparent;
            _isHollow = isHollow;
            _isGlass = isGlass;
            _isPassable = isPassable;
        }
    }
}
