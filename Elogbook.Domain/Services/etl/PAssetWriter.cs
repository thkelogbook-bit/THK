using Elogbook.Core.Models.ETL;
using Elogbook.Data;
using System;
using System.Collections.Generic;

namespace Elogbook.Domain.Services.etl
{
    public class PAssetWriter : Writer<PMIS_A_ASSET>
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public PAssetWriter(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public override void Write(List<PMIS_A_ASSET> data)
        {
            Console.WriteLine("write item: " + data.Count);
            _unitOfWork.ElogbookRepository<PMIS_A_ASSET>().AddRange(data);
            _unitOfWork.Complete();
        }
    }
}
