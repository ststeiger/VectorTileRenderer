using System;

namespace ClipperLib
{
	[Flags]
	internal enum Protects
	{
		ipNone = 0x0,
		ipLeft = 0x1,
		ipRight = 0x2,
		ipBoth = 0x3
	}
}
