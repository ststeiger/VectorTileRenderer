
namespace Typography.OpenFont
{


    // https://github.com/wine-mirror/wine/blob/master/include/t2embapi.h
    public enum TrueTypeEmbeddability
        : int
    {
        EMBED_PREVIEWPRINT = 1,
        EMBED_EDITABLE = 2,
        EMBED_INSTALLABLE = 3,
        EMBED_NOEMBEDDING = 4,


        EMBED_NOTTRUETYPE = 666
    }


}
