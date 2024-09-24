using System.Collections.Generic;
using Think9.Models;
using Think9.Repository;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class DonationService : BaseService<DonationEntity>
    {
        public DonationRepository DonationRepository = new DonationRepository();

        public dynamic GetListByFilter(DonationEntity filter, PageInfoEntity pageInfo)
        {
            string _where = " where 1=1";
            if (!string.IsNullOrEmpty(filter.Name))
            {
                _where += " and Name=@Name";
            }
            return GetPageByFilter(filter, pageInfo, _where);
        }

        public IEnumerable<DonationEntity> GetSumPriceTop(int num)
        {
            return DonationRepository.GetSumPriceTop(num);
        }

        public DonationEntity GetConsoleNumShow()
        {
            return DonationRepository.GetConsoleNumShow();
        }
    }
}