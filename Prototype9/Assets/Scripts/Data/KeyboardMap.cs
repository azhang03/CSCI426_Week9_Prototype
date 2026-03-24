using System.Collections.Generic;

public static class KeyboardMap
{
    private static readonly string Row1Keys = "QWERTYUIOP";
    private static readonly string Row2Keys = "ASDFGHJKL";
    private static readonly string Row3Keys = "ZXCVBNM";

    private static readonly string LeftHandKeys = "QWERTASDFGZXCVB";
    private static readonly string RightHandKeys = "YUIOPHJKLNM";

    private static readonly Dictionary<char, int> rowLookup = new Dictionary<char, int>();
    private static readonly Dictionary<char, int> positionLookup = new Dictionary<char, int>();
    private static readonly Dictionary<char, Hand> handLookup = new Dictionary<char, Hand>();

    static KeyboardMap()
    {
        BuildRowAndPositionLookups(Row1Keys, 1);
        BuildRowAndPositionLookups(Row2Keys, 2);
        BuildRowAndPositionLookups(Row3Keys, 3);

        foreach (char c in LeftHandKeys)
            handLookup[c] = Hand.Left;

        foreach (char c in RightHandKeys)
            handLookup[c] = Hand.Right;
    }

    private static void BuildRowAndPositionLookups(string keys, int row)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            rowLookup[keys[i]] = row;
            positionLookup[keys[i]] = i;
        }
    }

    public static bool IsValidKey(char c)
    {
        return rowLookup.ContainsKey(char.ToUpper(c));
    }

    public static int GetRow(char c)
    {
        c = char.ToUpper(c);
        return rowLookup.TryGetValue(c, out int row) ? row : -1;
    }

    public static int GetPositionInRow(char c)
    {
        c = char.ToUpper(c);
        return positionLookup.TryGetValue(c, out int pos) ? pos : -1;
    }

    public static Hand GetHand(char c)
    {
        c = char.ToUpper(c);
        return handLookup.TryGetValue(c, out Hand hand) ? hand : Hand.Left;
    }
}
