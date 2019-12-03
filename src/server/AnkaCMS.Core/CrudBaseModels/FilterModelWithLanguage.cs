using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithLanguage : FilterModel
    {
        public IdCodeName Language { get; set; }
    }
}