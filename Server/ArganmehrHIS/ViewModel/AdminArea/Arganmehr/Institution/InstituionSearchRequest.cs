using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AdminArea.Arganmehr.Institution
{

     public class InstituionSearchRequest
    {
        public InstituionSearchRequest()
        {
            SearchField = InstituionSearchField.Title;
            PageSize = PageSize.Count10;
            PageIndex = 1;
            SortBy = InstituionSortBy.Title;
            Status = InstituionStatus.All;
        }
        [DisplayName("براساس")]
        public InstituionSearchField SearchField { get; set; }
        [DisplayName("عبارت جستجو")]
        public string SearchFieldValue { get; set; }
        public int Total { get; set; }
        [DisplayName("وضعیت موسسه")]
        public InstituionStatus Status { get; set; }
        public int PageIndex { get; set; }
        [DisplayName("تعداد در صفحه")]
        public PageSize PageSize { get; set; }
        [DisplayName("مرتب سازی بر اساس")]
        public InstituionSortBy SortBy { get; set; }
        [DisplayName("در جهت")]
        public SortDirection SortDirection { get; set; }
       
    }
    public enum SortDirection
    {
        [Display(Name = "صعودی")]
        Ascending,
        [Display(Name = "نزولی")]
        Descending
    }

    public enum InstituionSearchField
    {
        [Display(Name = "عنوان موسسه")]
        Title,
        [Display(Name = "آدرس موسسه")]
      Address,
        [Display(Name = "پست الکترونیکی")]
        Email
    }
    public enum InstituionStatus
    {
        [Display(Name = "همه")]
        All ,
        [Display(Name = "مسدود شده")]
        Banned ,
        [Display(Name = "موسسه های فعال")]
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
    public enum InstituionSortBy
    {
       [Display(Name = "عنوان موسسه")]
        Title,
        [Display(Name = "آدرس موسسه")]
        Address,
        [Display(Name = "پست الکترونیکی")]
        Email
    }
}

