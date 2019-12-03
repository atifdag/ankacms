using AnkaCMS.Core.ValueObjects;
using System.Collections.Generic;

namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListModel<T> where T : class, IServiceModel, new()
    {
        public Paging Paging { get; set; }
        public List<T> Items { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
    }
}