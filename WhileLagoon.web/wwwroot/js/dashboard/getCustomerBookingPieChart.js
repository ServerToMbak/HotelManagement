$(document).ready(function () {
    loadCustomerBookingPieChart();
});

function loadCustomerBookingPieChart()
{
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetBookingPieChartData",
        type: 'GET',
        data: 'json',
        success: function (data) {


            loadPieChart("customerBookingsPieChart", data);

            $(".chart-spinner").hide();
        }
    });
}

function loadPieChart(id,data) {
    var chartColors = getChartColorsArray(id);

    options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: 'donut',/* you can meake this property pie too.*/
            width: 380,
        },
        stroke: {
            show: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: '#fff',
                useSeriesColors: true
            },
        }
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);

    chart.render();
}