using Elogbook.Core.Extensions;
using Elogbook.Core.Models.ETL;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services.etl
{
    public class PAssetReader : Reader<A_ASSET>
    {
        #region Private fields

        private readonly PmisUnitOfWork _pmisUnitOfWork;

        #endregion

        #region Contructor

        public PAssetReader(PmisUnitOfWork pmisUnitOfWork)
        {
            _pmisUnitOfWork = pmisUnitOfWork;
        }

        #endregion

        public override IEnumerator<A_ASSET> Read(int page, DateTime lastrun, int size)
        {
            var assets = _pmisUnitOfWork.PmisRepository<A_ASSET>()
                            .GetQuery(x => x.SITEID != null)
                            .OrderBy(t => t.ASSETID)
                            .ToPagedList(page, size);

            if (assets.Any())
            {
                var dt = assets.AsEnumerable().GetEnumerator();
                dt.Reset();
                return dt;
            }

            return null;
        }

        public override IEnumerator<A_ASSET> Read(int page, int jobId, int size)
        {
            return null;
        }
    }
}
