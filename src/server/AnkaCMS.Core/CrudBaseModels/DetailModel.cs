namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// Detay gösterme işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DetailModel<T> where T : class, IServiceModel, new()
    {
        public T Item { get; set; }
        public string Message { get; set; }
    }
}