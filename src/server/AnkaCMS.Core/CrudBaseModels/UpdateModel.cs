namespace AnkaCMS.Core.CrudBaseModels
{

    /// <summary>
    /// Güncelleme işlemlerinde kullanılacak jenerik sınıf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateModel<T> where T : class, IServiceModel, new()
    {
        public T Item { get; set; }
        public string Message { get; set; }
    }
}