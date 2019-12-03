using AnkaCMS.Data.BaseEntities;
using System.Collections.Generic;

namespace AnkaCMS.Data.DataEntities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<RoleUserLine> RoleUserLines { get; set; }
        public virtual ICollection<Category> CategoriesCreatedBy { get; set; }
        public virtual ICollection<Category> CategoriesLastModifiedBy { get; set; }
        public virtual ICollection<CategoryLanguageLine> CategoryLanguageLinesCreatedBy { get; set; }
        public virtual ICollection<CategoryLanguageLine> CategoryLanguageLinesLastModifiedBy { get; set; }
        public virtual ICollection<Content> ContentsCreatedBy { get; set; }
        public virtual ICollection<Content> ContentsLastModifiedBy { get; set; }
        public virtual ICollection<ContentLanguageLine> ContentLanguageLinesCreatedBy { get; set; }
        public virtual ICollection<ContentLanguageLine> ContentLanguageLinesLastModifiedBy { get; set; }
        public virtual ICollection<Language> LanguagesCreatedBy { get; set; }
        public virtual ICollection<Language> LanguagesLastModifiedBy { get; set; }
        public virtual ICollection<Menu> MenusCreatedBy { get; set; }
        public virtual ICollection<Menu> MenusLastModifiedBy { get; set; }
        public virtual ICollection<User> UsersCreatedBy { get; set; }
        public virtual ICollection<User> UsersLastModifiedBy { get; set; }
        public virtual ICollection<ParameterGroup> ParameterGroupsCreatedBy { get; set; }
        public virtual ICollection<ParameterGroup> ParameterGroupsLastModifiedBy { get; set; }
        public virtual ICollection<Parameter> ParametersCreatedBy { get; set; }
        public virtual ICollection<Parameter> ParametersLastModifiedBy { get; set; }
        public virtual ICollection<Part> PartsCreatedBy { get; set; }
        public virtual ICollection<Part> PartsLastModifiedBy { get; set; }
        public virtual ICollection<PartLanguageLine> PartLanguageLinesCreatedBy { get; set; }
        public virtual ICollection<PartLanguageLine> PartLanguageLinesLastModifiedBy { get; set; }
        public virtual ICollection<PartContentLine> PartContentLinesCreatedBy { get; set; }
        public virtual ICollection<PartContentLine> PartContentLinesLastModifiedBy { get; set; }
        public virtual ICollection<Permission> PermissionsCreatedBy { get; set; }
        public virtual ICollection<Permission> PermissionsLastModifiedBy { get; set; }
        public virtual ICollection<PermissionMenuLine> PermissionMenuLinesCreatedBy { get; set; }
        public virtual ICollection<PermissionMenuLine> PermissionMenuLinesLastModifiedBy { get; set; }
        public virtual ICollection<Role> RolesCreatedBy { get; set; }
        public virtual ICollection<Role> RolesLastModifiedBy { get; set; }
        public virtual ICollection<RolePermissionLine> RolePermissionLinesCreatedBy { get; set; }
        public virtual ICollection<RolePermissionLine> RolePermissionLinesLastModifiedBy { get; set; }
        public virtual ICollection<RoleUserLine> RoleUserLinesCreatedBy { get; set; }
        public virtual ICollection<RoleUserLine> RoleUserLinesLastModifiedBy { get; set; }
        public virtual ICollection<Session> SessionsCreatedBy { get; set; }
        public virtual ICollection<SessionHistory> SessionHistoriesCreatedBy { get; set; }
    }
}
