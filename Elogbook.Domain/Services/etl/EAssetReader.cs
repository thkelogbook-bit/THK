using Elogbook.Core.Extensions;
using Elogbook.Core.Models.ETL;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain.Services.etl
{
    public class EAssetReader : Reader<PMIS_A_ASSET>
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public EAssetReader(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public override IEnumerator<PMIS_A_ASSET> Read(int page, DateTime lastrun, int size)
        {
            return null;
        }

        public override IEnumerator<PMIS_A_ASSET> Read(int page, int jobId, int size)
        {
            var assets = _unitOfWork.ElogbookRepository<PMIS_A_ASSET>()
                            .GetQuery(x => x.JobId == jobId)
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
    }
}
