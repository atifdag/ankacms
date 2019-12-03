using System.Collections.Generic;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// Filtre işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    public class FilterModelWithMultiParent : FilterModel
    {
        public List<IdCodeName> Parents { get; set; }
    }
}