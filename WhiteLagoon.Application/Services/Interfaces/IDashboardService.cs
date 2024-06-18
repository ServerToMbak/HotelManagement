using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<RadialBarChartDto> GetTotalBookingRadialChartData();
        Task<RadialBarChartDto> GetRevenueUserChartData();
        Task<RadialBarChartDto> GetRegisteredUserChartData();
        Task<LineChartDto> GetMemberAndBookingLineChartData();
        Task<PieChartDto> GetBookingPieChartData();
    }
}
