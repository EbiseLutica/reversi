public static class StoneExtension
{
    public static Stone Invert(this Stone s) => s == Stone.None ? Stone.None : s == Stone.Black ? Stone.White : Stone.Black;
}
