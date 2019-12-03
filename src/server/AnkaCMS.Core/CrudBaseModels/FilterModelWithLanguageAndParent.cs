using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithLanguageAndParent : FilterModelWithLanguage
    {
        public IdCodeName Parent { get; set; }
    }
}