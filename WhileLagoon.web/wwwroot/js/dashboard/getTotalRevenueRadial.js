$(document).ready(function () {
    loadRevenueRadialChart();
});

function loadRevenueRadialChart()
{
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetRevenueUserChartData",
        type: 'GET',
        data: 'json',
        success: function (data) {
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>'
                    + data.countCurrentMounth + '</span>';
            } else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>'
                    + data.countCurrentMounth + '</span>';
            }

            document.querySelector("#sectionRevenueCount").append(sectionCurrentCount);
            document.querySelector("#sectionRevenueCount").append("since last month");


            loadRadioBarChart("totalRevenueRadialChart", data);

            $(".chart-spinner").hide();
        }
    });
}

