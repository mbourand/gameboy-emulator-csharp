namespace GBMU.Core;

public class OAMEntry
{
	public byte Y;
	public byte X;
	public byte Tile;
	public byte Flags;

	public OAMEntry(byte x, byte y, byte tile, byte flags)
	{
		Y = y;
		X = x;
		Tile = tile;
		Flags = flags;
	}

	public override string ToString() => $"Y: {Y}, X: {X}, Tile: {Tile}, Flags: {Flags}";
}