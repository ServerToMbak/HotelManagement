$(document).ready(function () {
    loadCustomerBookingLineChart();
});

function loadCustomerBookingLineChart()
{
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetMemberAndBookingLineChartData",
        type: 'GET',
        data: 'json',
        success: function (data) {


            loadLineChart("newMembersAndBookingsLineChart", data);

            $(".chart-spinner").hide();
        }
    });
}

function loadLineChart(id,data) {
    var chartColors = getChartColorsArray(id);

    options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            height: 350, /* you can meake this property pie too.*/
            type: 'line',
        },
        stroke: {
            curve: 'smooth',
            width: 2,
        },

        markers: {
            size: 3,
            strokeWidth: 0,
            hover: {
                size: 7
            }
        },

        xaxis: {
            categories: data.categories,
            labels: {
                style: {
                    colors: "#ddd",
                }
            }
        },

        yaxis: {
            labels: {
                style:{
                    colors: "#fff",
                }
            }
        },

        legend: {
            labels: {
                colors: "#fff",
            },
        },

        tooltip: {
            theme: 'dark'
        }
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);

    chart.render();
}