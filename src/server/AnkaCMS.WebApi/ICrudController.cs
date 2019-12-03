using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AnkaCMS.WebApi
{
    public interface ICrudController<T> where T : class, IServiceModel, new()
    {
        [Route("List")]
        [HttpGet]
        ActionResult<ListModel<T>> List();

        [Route("Filter")]
        [HttpPost]
        ActionResult<ListModel<T>> Filter([FromBody] FilterModel filterModel);

        [Route("Detail")]
        [HttpGet]
        ActionResult<DetailModel<T>> Detail([FromQuery] Guid id);

        [Route("Add")]
        [HttpGet]
        ActionResult<AddModel<T>> Add();

        [Route("Add")]
        [HttpPost]
        ActionResult<AddModel<T>> Add([FromBody] AddModel<T> addModel);

        [Route("Update")]
        [HttpGet]
        ActionResult<UpdateModel<T>> Update([FromQuery] Guid id);

        [Route("Update")]
        [HttpPut]
        ActionResult<UpdateModel<T>> Update([FromBody] UpdateModel<T> updateModel);

        [Route("Delete")]
        [HttpDelete]
        ActionResult Delete([FromQuery] Guid id);
    }
}
