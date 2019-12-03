namespace AnkaCMS.Core.Enums
{
    public enum ImageResizeOption
    {
        /// <summary>
        /// verilen genişlik ve yüksekliğe resmi sığdıryor ama boşluk kalabiliyor
        /// </summary>
        Fit = 1,
        /// <summary>
        /// resmi çözünürlüğe bakmayarak verilen genişliğe tam dolduruyor
        /// </summary>
        Stretch = 2,

        /// <summary>
        /// resmi verilen boyutlara göre keser
        /// </summary>
        Cut = 3,
        /// <summary>
        /// verilen resmi küçültür fazlalıkları keser
        /// </summary>
        Scale = 4
    }
}
