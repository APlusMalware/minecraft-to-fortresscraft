using MC_TO_FCE;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MC_to_FCE
{
    public class CubeType
    {
        private UInt16 _cubeType;
        private String _name;
        private Boolean _isSolid;
        private Boolean _isTransparent;
        private Boolean _isHollow;
        private Boolean _isGlass;
        private Boolean _isPassable;
        private static IDictionary<UInt16, CubeType> _cubes;

        public UInt16 TypeId
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
        public Boolean IsOpen
        {
            get
            {
                return _isTransparent || _isHollow;
            }
        }
        public static IDictionary<UInt16, CubeType> Cubes
        {
            get
            {
                return _cubes;
            }
        }


        public CubeType(UInt16 typeId, String name, Boolean isSolid, Boolean isTransparent, Boolean isHollow, Boolean isGlass, Boolean isPassable)
        {
            _cubeType = typeId;
            _name = name;
            _isSolid = isSolid;
            _isTransparent = isTransparent;
            _isHollow = isHollow;
            _isGlass = isGlass;
            _isPassable = isPassable;
        }


        public static void LoadFCETerrainData(String filePath)
        {
            IDictionary<UInt16, CubeType> cubes = new ConcurrentDictionary<UInt16, CubeType>();
            XmlDocument terrainData = new XmlDocument();
            terrainData.Load(filePath);
            XmlNodeList elements = terrainData.GetElementsByTagName("ArrayOfTerrainDataEntry");
            if (elements.Count > 0)
            {
                foreach (XmlNode terrainDataEntry in elements[0])
                {
                    UInt16 typeId = 0;
                    String name = "";
                    Boolean isSolid = false, isTransparent = false, isHollow = false, isGlass = false, isPassable = false;
                    foreach (XmlNode cubeDatum in terrainDataEntry.ChildNodes)
                    {
                        switch (cubeDatum.Name)
                        {
                            case "CubeType":
                                typeId = UInt16.Parse(cubeDatum.InnerText);
                                break;
                            case "Name":
                                name = cubeDatum.InnerText;
                                break;
                            case "isSolid":
                                isSolid = Boolean.Parse(cubeDatum.InnerText);
                                break;
                            case "isTransparent":
                                isTransparent = Boolean.Parse(cubeDatum.InnerText);
                                break;
                            case "isHollow":
                                isHollow = Boolean.Parse(cubeDatum.InnerText);
                                break;
                            case "isGlass":
                                isGlass = Boolean.Parse(cubeDatum.InnerText);
                                break;
                            case "isPassable":
                                isPassable = Boolean.Parse(cubeDatum.InnerText);
                                break;
                        }
                    }
                    CubeType cube = new CubeType(typeId, name, isSolid, isTransparent, isHollow, isGlass, isPassable);
                    cubes.Add(typeId, cube);
                }
            }
            _cubes = new ReadOnlyDictionaryWrapper<UInt16, CubeType>((IDictionary<UInt16, CubeType>) cubes);
        }

    }
}
