namespace Orikivo.Drawing
{
    public class CardTemplate
    {
        public string Name;
        public string AvatarUrl;
        public ulong Exp;
        public ulong NextLevelExp;
        public int Level;
        public int Ascent;
        public string Activity;
        public DiscordStatus Status;
    }

    public class CardConfig
    {
        public FontFace NameFont;
        public CardImageSize AvatarSize;
    }

    public enum DiscordStatus
    {
        Online = 1,
        Idle = 2,
        Busy = 4,
        Offline = 8
    }

    public enum CardImageSize
    {
        Small = 16,
        Large = 32
    }
}
