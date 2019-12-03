namespace AnkaCMS.Core.CrudBaseModels
{
    /// <summary>
    /// Ekleme işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AddModel<T> where T : class, IServiceModel, new()
    {
        public T Item { get; set; }
        public string Message { get; set; }
    }
}