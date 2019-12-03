using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithParent : FilterModel
    {
        public IdCodeName Parent { get; set; }
    }
}