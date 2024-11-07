window.createApexChart = (element, options) => {
    var opt = JSON.parse(JSON.stringify(options));
    var chart = new ApexCharts(document.getElementById(element), opt);

    chart.render();
}

window.createApexChartPie = (element, options) => {
    var options = JSON, parse(JSON.stringify(options));
    var chart = new ApexCharts(document.getElementById(element), options);

    chart.render();
}

window.updateApexChart = (element, seriesData) => {
    var chart = document.getElementById(element);

    if (chart) {
        ApexCharts.exec(element, 'updateSeries', seriesData, true);
        ApexCharts.exec(element, 'render');

    }
};

window.destroyApexChart = (element) => {    
    var chart = document.getElementById(element);

    if (chart) {
        ApexCharts.exec(element, 'destroy');
    }
}

window.updateChartOptions = (element, options) => {
    var opt = JSON.parse(JSON.stringify(options));
    var chart = document.getElementById(element);

    if (chart) {
        ApexCharts.exec(element, 'updateOptions', opt);
        ApexCharts.exec(element, 'render');
    }
}