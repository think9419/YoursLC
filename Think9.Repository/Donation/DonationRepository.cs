using Dapper;
using System.Collections.Generic;
using System.Linq;
using Think9.Models;

namespace Think9.Repository
{
    public class DonationRepository : BaseRepository<DonationEntity>//, IDonationRepository
    {
        /// <summary>
        /// 获取捐赠排行榜
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<DonationEntity> GetSumPriceTop(int num)
        {
            using (var conn = dbContext.GetConnection())
            {
                string sql = @"SELECT any_value(Id) Id,`Name`,any_value(SUM(Price)) Price FROM sys_donation
                            GROUP BY `Name`
                            ORDER BY Price desc
                            LIMIT 0,@num";
                return conn.Query<DonationEntity>(sql, new { num = num });
            }
        }

        /// <summary>
        /// 获取控制台显示数字
        /// </summary>
        /// <returns></returns>
        public DonationEntity GetConsoleNumShow()
        {
            using (var conn = dbContext.GetConnection())
            {
                string sql = @"SELECT
                            (SELECT SUM(Price) TotalPrice FROM sys_donation) TotalPrice,
                            (SELECT COUNT(1) TotalNum from sys_donation) TotalNum,
                            (SELECT MAX(CAST(Price as DECIMAL(15,2))) MaxPrice FROM sys_donation) MaxPrice,
                            (SELECT COUNT(1) PeopleNum FROM( SELECT `Name` FROM sys_donation
                            GROUP BY `Name`) a) PeopleNum";
                return conn.Query<DonationEntity>(sql).FirstOrDefault();
            }
        }
    }
}