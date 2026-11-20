using Elogbook.Core.Models.ETL;
using Elogbook.Data;
using System;
using System.Collections.Generic;

namespace Elogbook.Domain.Services.etl
{
    public class EAssetWriter : Writer<AmDevice>
    {
        #region Private fields

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public EAssetWriter(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public override void Write(List<AmDevice> data)
        {
            Console.WriteLine("write item: " + data.Count);
            _unitOfWork.ElogbookRepository<AmDevice>().AddRange(data);
            _unitOfWork.Complete();
        }
    }
}
