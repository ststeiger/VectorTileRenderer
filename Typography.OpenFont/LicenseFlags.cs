
namespace Typography.OpenFont
{

    // https://github.com/wine-mirror/wine/blob/master/include/t2embapi.h
    [System.Flags()]
    internal enum LicenseFlags
     : int
    {
        LICENSE_INSTALLABLE = 0x0000,
        LICENSE_DEFAULT = 0x0000,
        LICENSE_NOEMBEDDING = 0x0002,
        LICENSE_PREVIEWPRINT = 0x0004,
        LICENSE_EDITABLE = 0x0008,
    }


}
