using Common.Caching;
using Common.Helpers.Extentions;
using DataLayer.Context;
using DomainClasses.Entities.AM;
using DomainClasses.Extensions;
using ServiceLayer.Contracts.AM.CA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.EFServiecs.AM.CA
{
    public class CentralAdminService : ICentralAdminService
    {
        private const string CM_ALL_KEY = "Arganmehr.centralmanagment.all";
        private const string CM_PATTERN_KEY = "Arganmehr.centralmanagment.";
        private const string CM_BY_ID_KEY = "Arganmehr.centralmanagment.id-{0}";
        private const string CM_BY_ID_TITLE = "Arganmehr.centralmanagment.Title-{0}";
        private const string CM_BY_ID_NAME = "Arganmehr.centralmanagment.Name-{0}";


        private readonly IDbSet<CentralAdmin> _CM;
        private readonly ICacheManagerKRB _cacheManager;
        private readonly IUnitOfWork _uow;
        private bool? _isSingleCentralAdminMode = null;
        public CentralAdminService(IUnitOfWork uow, ICacheManagerKRB cacheManager)
		{
            this._uow = uow;
			this._cacheManager = cacheManager;
            this._CM = _uow.Set<CentralAdmin>();
		}


        public bool ExistCentralAdmin()
        {
            return _CM.Any();
        }
        
        	public virtual void DeleteCentralAdmin(CentralAdmin CentralAdmin)
		{
			if (CentralAdmin == null)
				throw new ArgumentNullException("CentralAdmin");

			var allCentralAdmins = GetAllCentralAdmins();
			if (allCentralAdmins.Count == 1)
				throw new Exception("You cannot delete the only configured CentralAdmin.");

            _CM.Remove(CentralAdmin);

			_cacheManager.RemoveByPattern(CM_PATTERN_KEY);

		}

		/// <summary>
		/// Gets all centralmanagment
		/// </summary>
		/// <returns>CentralAdmin collection</returns>
		public virtual IList<CentralAdmin> GetAllCentralAdmins()
		{
			string key = CM_ALL_KEY;
			return _cacheManager.Get(key, () =>
			{
                var query = _CM.AsQueryable();
                  //  .Expand(x => x.Language);

				var centralmanagment = query.ToList();
				return centralmanagment;
			});
		}

		/// <summary>
		/// Gets a CentralAdmin 
		/// </summary>
		/// <param name="CentralAdminId">CentralAdmin identifier</param>
		/// <returns>CentralAdmin</returns>
        /// 
        public CentralAdmin GetFirstCentralAdmin()
        {
           

            string key = string.Format(CM_BY_ID_KEY, 0);
            return _cacheManager.Get(key, () =>
            {
                if (_CM.Count() > 0)
                    return _CM.First();
                return null;
            });
        }

		public virtual CentralAdmin GetCentralAdminById(int CentralAdminId)
		{
			if (CentralAdminId == 0)
				return null;

            string key = string.Format(CM_BY_ID_KEY, CentralAdminId);
            return _cacheManager.Get(key, () => 
            {
                return _CM.Find(CentralAdminId); 
            });
		}

        public CentralAdmin GetCentralAdminByTitle(string CMTitle)
        {
            if (CMTitle.IsEmpty())
                return null;

            string key = string.Format(CM_BY_ID_TITLE, CMTitle);
            return _cacheManager.Get(key, () =>
            {
                var cms = _CM.Where(x => x.Title.Contains(CMTitle));
                if (cms.Count() > 0)
                    return cms.First();
                return null;
            });
        }

        public CentralAdmin GetCentralAdminByName(string Name)
        {
            if (Name.IsEmpty())
                return null;

            string key = string.Format(CM_BY_ID_NAME, Name);
            return _cacheManager.Get(key, () =>
            {
                var cms = _CM.Where(x => x.Name.Contains(Name));
                if (cms.Count() > 0)
                    return cms.First();
                return null;
            });
        }


		/// <summary>
		/// Inserts a CentralAdmin
		/// </summary>
		/// <param name="CentralAdmin">CentralAdmin</param>
		public virtual void InsertCentralAdmin(CentralAdmin CentralAdmin)
		{
			if (CentralAdmin == null)
				throw new ArgumentNullException("CentralAdmin");

            _CM.Add(CentralAdmin);

			_cacheManager.RemoveByPattern(CM_PATTERN_KEY);

		}

		/// <summary>
		/// Updates the CentralAdmin
		/// </summary>
		/// <param name="CentralAdmin">CentralAdmin</param>
		public virtual void UpdateCentralAdmin(CentralAdmin CentralAdmin)
		{
			if (CentralAdmin == null)
				throw new ArgumentNullException("CentralAdmin");

            //var ca = _CM.Find(CentralAdmin.Id);
           
            //ca.CreateDate = CentralAdmin.CreateDate;
            //ca.EstablishmentDate = CentralAdmin.EstablishmentDate;
            //ca.Logo = CentralAdmin.Logo;
            //ca.Name = CentralAdmin.Name;
            //ca.PWSLinkServer = CentralAdmin.PWSLinkServer;
            //ca.SecureUrl = CentralAdmin.SecureUrl;
            //ca.SslEnabled = CentralAdmin.SslEnabled;
            //ca.Title = CentralAdmin.Title;
            //ca.Url = CentralAdmin.Url;
            _CM.Remove(CentralAdmin);
            _uow.SaveAllChanges();

            _CM.Add(CentralAdmin);
            _uow.SaveAllChanges();

			_cacheManager.RemoveByPattern(CM_PATTERN_KEY);

		}

		/// <summary>
		/// True if there's only one CentralAdmin. Otherwise False.
		/// </summary>
		public virtual bool IsSingleCentralAdminMode()
		{
			if (!_isSingleCentralAdminMode.HasValue)
			{
                _isSingleCentralAdminMode = (_CM.AsQueryable().Count() <= 1);
			}

			return _isSingleCentralAdminMode.Value;
		}

		/// <summary>
		/// True if the CentralAdmin data is valid. Otherwise False.
		/// </summary>
		/// <param name="CentralAdmin">CentralAdmin entity</param>
		public virtual bool IsCentralAdminDataValid(CentralAdmin CentralAdmin)
		{
			if (CentralAdmin == null)
				throw new ArgumentNullException("CentralAdmin");

			if (CentralAdmin.Url.IsEmpty())
				return false;

			try
			{
				var uri = new Uri(CentralAdmin.Url);
				var domain = uri.DnsSafeHost.EmptyNull().ToLower();

				switch (domain)
				{
					case "www.yourCentralAdmin.com":
					case "yourCentralAdmin.com":
					case "www.mein-shop.de":
					case "mein-shop.de":
						return false;
					default:
						return CentralAdmin.Url.IsWebUrl();
				}
			}
			catch (Exception)
			{
				return false;
			}
		}




        public void SeedDatabase()
        {
            CentralAdmin cm = this.GetCentralAdminByName("جندی شاپور");
            if(cm==null)
            {
                cm = new CentralAdmin
                {
                    CreateDate = DateTime.Now,
                    EstablishmentDate = DateTime.Now,
                    Logo = null,
                    Name = "علوم پزشکی جندی شاپور اهواز",
                    SecureUrl = null,
                    SslEnabled = false,
                    Title = "جندی شاپور اهواز",
                    Url = null
                };
                this.InsertCentralAdmin(cm);
            }
            _uow.SaveChanges();
            
        }


    }
}
