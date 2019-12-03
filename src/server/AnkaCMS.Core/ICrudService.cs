using AnkaCMS.Core.CrudBaseModels;
using System;

namespace AnkaCMS.Core
{
    /// <summary>
    /// CRUD işlemleri yapan sınıflar için arayüz
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrudService<T> where T : class, IServiceModel, new()
    {
        /// <summary>
        /// Filtreleme yaparak birden çok satır içeren liste modelini döndürür.
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        ListModel<T> List(FilterModel filterModel);

        /// <summary>
        /// ID parametresi alarak tek satır içeren detay modelini döndürür.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DetailModel<T> Detail(Guid id);

        /// <summary>
        /// Ekleme işlemi için gerekli modeli hazırlar.
        /// </summary>
        /// <returns></returns>
        AddModel<T> Add();

        /// <summary>
        /// Ekleme işlemini yaparak sonucu modelle döndürür.
        /// </summary>
        /// <param name="addModel"></param>
        AddModel<T> Add(AddModel<T> addModel);

        /// <summary>
        /// Güncellemesi yapılacak gerekli modeli hazırlar.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UpdateModel<T> Update(Guid id);

        /// <summary>
        /// Güncelleme işlemini yaparak sonucu modelle döndürür.
        /// </summary>
        /// <param name="updateModel"></param>
        UpdateModel<T> Update(UpdateModel<T> updateModel);

        /// <summary>
        /// Silme işlemini yapar.
        /// </summary>
        /// <param name="id"></param>
        void Delete(Guid id);
    }
}
