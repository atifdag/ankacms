using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;

namespace AnkaCMS.Service
{
    public interface IUserService : ICrudService<UserModel>
    {
        ListModel<UserModel> List(FilterModelWithMultiParent filterModel);
        MyProfileModel MyProfile();
        void UpdateMyPassword(UpdatePasswordModel model);
        void UpdateMyInformation(UpdateInformationModel model);
    }
}
