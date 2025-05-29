using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DomainClasses;
using DomainClasses.Entities.Common;
using DomainClasses.Entities.Users;

namespace ViewModel.AdminArea.User
{
    public class UserSearchRequest
    {
        public UserSearchRequest()
        {
            SearchField = UserSearchField.UserName;
            PageSize = PageSize.Count10;
            PageIndex = 1;
            SortBy = UserSortBy.UserName;
            Status = UserStatus.All;
        }
        [DisplayName("براساس")]
        public UserSearchField SearchField { get; set; }
        [DisplayName("عبارت جستجو")]
        public string SearchFieldValue { get; set; }
        public int Total { get; set; }
        [DisplayName("وضعیت کاربر")]
        public UserStatus Status { get; set; }
        public int PageIndex { get; set; }
        [DisplayName("تعداد در صفحه")]
        public PageSize PageSize { get; set; }
        [DisplayName("مرتب سازی بر اساس")]
        public UserSortBy SortBy { get; set; }
        [DisplayName("در جهت")]
        public SortDirection SortDirection { get; set; }
        public long[] RoleIds { get; set; }
    }
    public enum SortDirection
    {
        [Display(Name = "صعودی")]
        Ascending,
        [Display(Name = "نزولی")]
        Descending
    }

    public enum UserSearchField
    {
        [Display(Name = "نام کاربری")]
        UserName,
        [Display(Name = "ایمیل")]
        Email,
        [Display(Name = "نام")]
        FirstName,
        [Display(Name = "نام خانوادگی")]
        LastName,
        [Display(Name = "آخرین آی پی")]
        LastLoginIp
    }
    public enum UserStatus
    {
        [Display(Name = "همه")]
        All ,
        [Display(Name = "مسدود شده")]
        Banned ,
        [Display(Name = "اکانت های فعال")]
        Active ,
        [Display(Name = "حذف شده ها")]
        Deleted 
    }

    public enum PageSize
    {
        [Display(Name = "10")]
        Count10 = 10,
        [Display(Name = "20")]
        Count20 = 20,
        [Display(Name = "30")]
        Count30 = 30,
        [Display(Name = "40")]
        Count40 = 40,
        [Display(Name = "50")]
        Count50 = 50,
        [Display(Name = "همه")]
        All = 1
    }
    public enum UserSortBy
    {
        [Display(Name = "نام کاربری")]
        UserName,
        [Display(Name = "نام")]
        FirstName,
        [Display(Name = "نام خانوادگی")]
        LastName,
        [Display(Name = "آخرین ورود")]
        LastLogingDate
    }
}
