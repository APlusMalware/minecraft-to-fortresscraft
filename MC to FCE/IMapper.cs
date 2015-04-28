using Synergy.FCU;
using System;
using System.Collections.Generic;

namespace MC_to_FCE
{
	public interface IMapper
	{
		String FCEDirectory { get; set; }
		IDictionary<UInt16, CubeType> FCECubes { get; set; }
		IDictionary<UInt16, String> UnknownBlocks { get; set; }
        List<String> LoadNameMap(String filepath);
		World ConvertWorld(String sourceDirectory);
	}
}
