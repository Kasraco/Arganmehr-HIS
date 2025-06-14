﻿using DataLayer.Context;
using Common.Helpers.Extentions;
using DomainClasses.Entities.Logg;
using DomainClasses.Entities.Users;
using ServiceLayer.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer.Contracts;

namespace ServiceLayer.Logger
{
    public partial class DefaultLogger : ILogger
    {
        #region Fields

        private const int _deleteNumberOfEntries = 1000;

        private readonly IDbSet<Log> _logRepository;
        private readonly IWebHelper _webHelper;
        private readonly IUnitOfWork _dbContext;
        private readonly IWorkContext _workContext;

        private readonly IList<LogContext> _entries = new List<LogContext>();

        #endregion

        #region Ctor

        public DefaultLogger(IUnitOfWork uow, IWebHelper webHelper,  IWorkContext workContext)
        {
            this._dbContext = uow;
            this._logRepository = _dbContext.Set<Log>(); ;
            this._webHelper = webHelper;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        public virtual void DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            _logRepository.Remove(log);
        }

        public virtual void ClearLog()
        {
            try
            {
                _dbContext.ExecuteSqlCommand("TRUNCATE TABLE [Log]");
            }
            catch
            {
                try
                {
                    for (int i = 0; i < 100000; ++i)
                    {
                        if (_dbContext.ExecuteSqlCommand("Delete Top ({0}) From [Log]", false, null, _deleteNumberOfEntries) < _deleteNumberOfEntries)
                            break;
                    }
                }
                catch { }

                try
                {
                    _dbContext.ExecuteSqlCommand("DBCC CHECKIDENT('Log', RESEED, 0)");
                }
                catch
                {
                    try
                    {
                        _dbContext.ExecuteSqlCommand("Alter Table [Log] Alter Column [Id] Identity(1,1)");
                    }
                    catch { }
                }
            }

            _dbContext.SaveChanges();
        }

        public virtual void ClearLog(DateTime toUtc, LogLevel logLevel)
        {
            try
            {
                string sqlDelete = "Delete Top ({0}) From [Log] Where LogLevelId < {1} And CreatedOnUtc <= {2}";

                for (int i = 0; i < 100000; ++i)
                {
                    if (_dbContext.ExecuteSqlCommand(sqlDelete, false, null, _deleteNumberOfEntries, (int)logLevel, toUtc) < _deleteNumberOfEntries)
                        break;
                }

                _dbContext.SaveChanges();
            }
            catch { }
        }

        public virtual List<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc, string message, LogLevel? logLevel, int pageIndex, int pageSize, int minFrequency)
        {
            var query = _logRepository.AsNoTracking();

            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                int logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }
            if (!String.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            if (minFrequency > 0)
                query = query.Where(l => l.Frequency >= minFrequency);

            //query = _logRepository.Expand(query, x => x.Customer);

          //  var log = new PagedList<Log>(query, pageIndex, pageSize);
            return query.ToList();
        }

        public virtual Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;

            var log = _logRepository.Find(logId);
            return log;
        }

        public virtual IList<Log> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var query = from l in _logRepository.AsNoTracking()
                        where logIds.Contains(l.Id)
                        select l;
            var logItems = query.ToList();
            //sort by passed identifiers
            var sortedLogItems = new List<Log>();
            foreach (int id in logIds)
            {
                var log = logItems.Find(x => x.Id == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }
            return sortedLogItems;
        }

        public virtual void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            var context = new LogContext
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                User = user
            };

            InsertLog(context);
        }

        public virtual void InsertLog(LogContext context)
        {
            _entries.Add(context);
            if (_entries.Count == 50)
            {
                Flush();
            }
        }

        public void Flush()
        {
            if (_entries.Count == 0)
                return;

            string ipAddress = "";
            string pageUrl = "";
            string referrerUrl = "";
            var currentCustomer = _workContext.CurrentUser;

            try
            {
                ipAddress = _webHelper.GetCurrentIpAddress();
                pageUrl = _webHelper.GetThisPageUrl(true);
                referrerUrl = _webHelper.GetUrlReferrer();
            }
            catch { }

            
                foreach (var context in _entries)
                {
                    if (context.ShortMessage.IsEmpty() && context.FullMessage.IsEmpty())
                        continue;

                    Log log = null;

                    try
                    {
                        string shortMessage = context.ShortMessage.NaIfEmpty();
                        string fullMessage = context.FullMessage.EmptyNull();
                        string contentHash = null;

                        if (context.HashNotFullMessage || context.HashIpAddress)
                        {
                            contentHash = (shortMessage
                                + (context.HashNotFullMessage ? "" : fullMessage)
                                + (context.HashIpAddress ? ipAddress.EmptyNull() : "")
                            ).Hash(Encoding.Unicode, true);
                        }
                        else
                        {
                            contentHash = (shortMessage + fullMessage).Hash(Encoding.Unicode, true);
                        }

                        log = _logRepository.AsNoTracking().OrderByDescending(x => x.CreatedOnUtc).FirstOrDefault(x => x.ContentHash == contentHash);

                        if (log == null)
                        {
                            log = new Log
                            {
                                Frequency = 1,
                                LogLevel = context.LogLevel,
                                ShortMessage = shortMessage,
                                FullMessage = fullMessage,
                                IpAddress = ipAddress,
                                User = context.User ?? currentCustomer,
                                PageUrl = pageUrl,
                                ReferrerUrl = referrerUrl,
                                CreatedOnUtc = DateTime.UtcNow,
                                ContentHash = contentHash
                            };

                            _logRepository.Add(log);
                        }
                        else
                        {
                            if (log.Frequency < 2147483647)
                                log.Frequency = log.Frequency + 1;

                            log.LogLevel = context.LogLevel;
                            log.IpAddress = ipAddress;
                            log.User = context.User ?? currentCustomer;
                            log.PageUrl = pageUrl;
                            log.ReferrerUrl = referrerUrl;
                            log.UpdatedOnUtc = DateTime.UtcNow;

                            
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Dump();
                    }
                }

                try
                {
                    // FIRE!
                    _dbContext.SaveChanges();
                }
                catch { }
            

            _entries.Clear();
        }

        #endregion

    }
}
