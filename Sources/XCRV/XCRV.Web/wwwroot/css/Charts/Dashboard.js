$(function () {
    
   // $(window).on("load", function () {
        

        var userCount = 0;
        var cibCount = 0;
        var complainCount = 0;
        var loanActivationCount = 0;
        var loanProcessCount = 0;
        var loanProcessAmountCount = 0;
        var userCtx = $("#customerActivation-chart");
        var cibCtx = $("#cib-chart");
        var complainCtx = $("#complain-chart");
        var loanActivationCtx = $("#loanActivation-chart");
        var loanProcessCtx = $("#loanProcess-chart");
        var loanProcessAmountCtx = $("#loanProcessAmount-chart");


        ///////////// for summary data /////////////////////

        $.ajax({
            url: "/Home/GetSummaryDataForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {
                
                summaryData = response.responseJSON.data;
                //$("#myTask").html(summaryData.MyTask);
                $("#customerActivation").html(summaryData.TotalCustomerActivation);
                $("#activeLoan").html(summaryData.TotalLoanActivation);
                $("#loanDelivered").html(summaryData.TotalLoanProcess);
                $("#disbursedAmount").html((summaryData.TotalLoanProcessAmount / 1000).toFixed(2)+"k");
            }
        });

        /////////////  chart for Customer Activation /////////////////////

        $.ajax({
            url: "/Home/CountUserActivitionForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {

                userCount = response.responseJSON.data;

                // Chart Options
                var userChartOptions = {
                    responsive: true,
                    maintainAspectRatio: false,
                    responsiveAnimationDuration: 500,
                };

                // Chart Data
                var userChartData = {
                    labels: ["Today", "Last 7 days", "Last 30 days"],
                    datasets: [{
                        label: "Customer Activation",
                        data: [userCount.DailyUserActivation, userCount.WeeklyUserActivation, userCount.MonthlyUserActivation],
                        backgroundColor: ['#666EE8', '#28D094', '#7a49a5'],
                    }]
                };

                var userChartconfig = {
                    type: 'doughnut',

                    // Chart Options
                    options: userChartOptions,

                    data: userChartData
                };

                // Create the chart
                new Chart(userCtx, userChartconfig);
            }
        });


        /////////////  chart for Complain status/////////////////////

        $.ajax({
            url: "/Home/CountComplainStatusForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {

                ///////////////////////////////

                complainCount = response.responseJSON.data;
                // Chart Options
                var complainChartOptions = {
                    // Elements options apply to all of the options unless overridden in a dataset
                    // In this case, we are setting the border of each bar to be 2px wide and green
                    elements: {
                        rectangle: {
                            borderWidth: 2,
                            borderColor: 'rgb(0, 255, 0)',
                            borderSkipped: 'bottom'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    responsiveAnimationDuration: 500,
                    legend: {
                        position: 'top',
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }],
                        yAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }]
                    },
                    title: {
                        display: false,
                        text: 'Complain Status'
                    }
                };

                // Chart Data FF7D4D
                var complainChartData = {
                    labels: ["Today", "Last 7 days", "Last 30 days"],
                    datasets: [{
                        label: "Pending",
                        data: [complainCount.DailyPendingComplain
                            , complainCount.WeeklyPendingComplain
                            , complainCount.MonthlyPendingComplain],
                        backgroundColor: "#FF7D4D",
                        hoverBackgroundColor: "rgba(40,208,148,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "Received",
                        data: [complainCount.DailyReceivedComplain
                            , complainCount.WeeklyReceivedComplain
                            , complainCount.MonthlyReceivedComplain],
                        backgroundColor: "#CE6967",
                        hoverBackgroundColor: "rgba(255,73,97,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "In Progress",
                        data: [complainCount.DailyInprogressComplain
                            , complainCount.WeeklyInprogressComplain
                            , complainCount.MonthlyInprogressComplain],
                        backgroundColor: "#6967ce",
                        hoverBackgroundColor: "rgba(105,103,206,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "Resolved",
                        data: [complainCount.DailyResolvedComplain
                            , complainCount.WeeklyResolvedComplain
                            , complainCount.MonthlyResolvedComplain],
                        backgroundColor: "#67CE69",
                        hoverBackgroundColor: "rgba(255,73,97,.9)",
                        borderColor: "transparent"
                    }]
                };

                var complainChartconfig = {
                    type: 'bar',

                    // Chart Options
                    options: complainChartOptions,

                    data: complainChartData
                };

                // Create the chart
                new Chart(complainCtx, complainChartconfig);

            }
        });

        /////////// chart for CIB Status/////////////////////

        $.ajax({
            url: "/Home/CountCIBStatusForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {

                cibCount = response.responseJSON.data;


                // Chart Options
                var cibChartOptions = {
                    // Elements options apply to all of the options unless overridden in a dataset
                    // In this case, we are setting the border of each horizontal bar to be 2px wide and green
                    elements: {
                        rectangle: {
                            borderWidth: 2,
                            borderColor: 'rgb(0, 255, 0)',
                            borderSkipped: 'left'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    responsiveAnimationDuration: 500,
                    legend: {
                        position: 'top',
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }],
                        yAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }]
                    },
                    title: {
                        display: false,
                        text: 'CIB Status'
                    }
                };

                // Chart Data
                var cibChartData = {
                    labels: ["Today", "Last 7 days", "Last 30 days"],
                    datasets: [{
                        label: "PENDING",
                        data: [cibCount.DailyPendingCIB, cibCount.WeeklyPendingCIB, cibCount.MonthlyPendingCIB],
                        backgroundColor: "#FF7D4D",
                        hoverBackgroundColor: "rgba(40,208,148,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "INPROGRESS",
                        data: [cibCount.DailyInProgressCIB, cibCount.WeeklyInProgressCIB, cibCount.MonthlyInProgressCIB],
                        backgroundColor: "#666EE8",
                        hoverBackgroundColor: "rgba(255,73,97,.9)",
                        borderColor: "transparent"
                        }, {
                            label: "COMPLETED",
                        data: [cibCount.DailyCompletedCIB, cibCount.WeeklyCompletedCIB, cibCount.MonthlyCompletedCIB],
                        backgroundColor: "#28D094",
                            hoverBackgroundColor: "rgba(255,73,97,.9)",
                            borderColor: "transparent"
                        }, {
                            label: "FAILED",
                        data: [cibCount.DailyFailedCIB, cibCount.WeeklyFailedCIB, cibCount.MonthlyFailedCIB],
                        backgroundColor: "#FF4961",
                            hoverBackgroundColor: "rgba(255,73,97,.9)",
                            borderColor: "transparent"
                        }]
                };

                var cibChartconfig = {
                    type: 'bar',

                    // Chart Options
                    options: cibChartOptions,

                    data: cibChartData
                };

                // Create the chart
                new Chart(cibCtx, cibChartconfig);

            }
        });

        ///////////  chart for loan activation /////////////////////

        $.ajax({
            url: "/Home/CountLoanActivationForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {

                ///////////////////////////////

                loanActivationCount = response.responseJSON.data;
                // Chart Options
                var loanActivationChartOptions = {
                    // Elements options apply to all of the options unless overridden in a dataset
                    // In this case, we are setting the border of each bar to be 2px wide and green
                    elements: {
                        rectangle: {
                            borderWidth: 2,
                            borderColor: 'rgb(0, 255, 0)',
                            borderSkipped: 'bottom'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    responsiveAnimationDuration: 500,
                    legend: {
                        position: 'top',
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }],
                        yAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }]
                    },
                    title: {
                        display: false,
                        text: 'Loan Activation'
                    }
                };

                // Chart Data
                var loanActivationChartData = {
                    labels: ["Today", "Last 7 days", "Last 30 days"],
                    datasets: [{
                        label: "Initiated",
                        data: [loanActivationCount.DailyInitiatedLoan
                            , loanActivationCount.WeeklyInitiatedLoan
                            , loanActivationCount.MonthlyInitiatedLoan],
                        backgroundColor: "#FF4961",
                        hoverBackgroundColor: "rgba(40,208,148,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "Active",
                        data: [loanActivationCount.DailyActiveLoan
                            , loanActivationCount.WeeklyActiveLoan
                            , loanActivationCount.MonthlyActiveLoan],
                        backgroundColor: "#28D094",
                        hoverBackgroundColor: "rgba(255,73,97,.9)",
                        borderColor: "transparent"
                    }]
                };

                var loanActivationChartconfig = {
                    type: 'bar',

                    // Chart Options
                    options: loanActivationChartOptions,

                    data: loanActivationChartData
                };

                // Create the chart
                new Chart(loanActivationCtx, loanActivationChartconfig);

            }
        });

        /////////////  chart for loan process /////////////////////

        $.ajax({
            url: "/Home/CountLoanProcessForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {

                ///////////////////////////////

                loanProcessCount = response.responseJSON.data;
                // Chart Options
                var loanProcessChartOptions = {
                    // Elements options apply to all of the options unless overridden in a dataset
                    // In this case, we are setting the border of each bar to be 2px wide and green
                    elements: {
                        rectangle: {
                            borderWidth: 2,
                            borderColor: 'rgb(0, 255, 0)',
                            borderSkipped: 'bottom'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    responsiveAnimationDuration: 500,
                    legend: {
                        position: 'top',
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }],
                        yAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }]
                    },
                    title: {
                        display: false,
                        text: 'No Of Loan Process'
                    }
                };

                // Chart Data
                var loanProcessChartData = {
                    labels: ["Today", "Last 7 days", "Last 30 days"],
                    datasets: [{
                        label: "Delivered",
                        data: [loanProcessCount.DailyDeliveredLoan
                            , loanProcessCount.WeeklyDeliveredLoan
                            , loanProcessCount.MonthlyDeliveredLoan],
                        backgroundColor: "#28D094",
                        hoverBackgroundColor: "rgba(40,208,148,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "Undelivered",
                        data: [loanProcessCount.DailyUndeliveredLoan
                            , loanProcessCount.WeeklyUndeliveredLoan
                            , loanProcessCount.MonthlyUndeliveredLoan],
                        backgroundColor: "#FF4961",
                        hoverBackgroundColor: "rgba(255,73,97,.9)",
                        borderColor: "transparent"
                    }]
                };

                var loanProcessChartconfig = {
                    type: 'bar',

                    // Chart Options
                    options: loanProcessChartOptions,

                    data: loanProcessChartData
                };

                // Create the chart
                new Chart(loanProcessCtx, loanProcessChartconfig);

            }
        });

        /////////////  chart for loan process amount /////////////////////

        $.ajax({
            url: "/Home/LoanProcessAmountForDashBoard",
            type: "GET",
            //async: false,
            //  dataType: "json",
            complete: function (response) {

                ///////////////////////////////

                loanProcessAmountCount = response.responseJSON.data;
                // Chart Options
                var loanProcessAmountChartOptions = {
                    // Elements options apply to all of the options unless overridden in a dataset
                    // In this case, we are setting the border of each bar to be 2px wide and green
                    elements: {
                        rectangle: {
                            borderWidth: 2,
                            borderColor: 'rgb(0, 255, 0)',
                            borderSkipped: 'bottom'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    responsiveAnimationDuration: 500,
                    legend: {
                        position: 'top',
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }],
                        yAxes: [{
                            display: true,
                            gridLines: {
                                color: "#f3f3f3",
                                drawTicks: false,
                            },
                            scaleLabel: {
                                display: true,
                            }
                        }]
                    },
                    title: {
                        display: false,
                        text: 'Loan Process Amount'
                    }
                };

                // Chart Data
                var loanProcessAmountChartData = {
                    labels: ["Today", "Last 7 days", "Last 30 days"],
                    datasets: [{
                        label: "Delivered",
                        data: [loanProcessAmountCount.DailyDeliveredAmount
                            , loanProcessAmountCount.WeeklyDeliveredAmount
                            , loanProcessAmountCount.MonthlyDeliveredAmount],
                        backgroundColor: "#28D094",
                        hoverBackgroundColor: "rgba(40,208,148,.9)",
                        borderColor: "transparent"
                    }, {
                        label: "Undelivered",
                        data: [loanProcessAmountCount.DailyUndeliveredAmount
                            , loanProcessAmountCount.WeeklyUndeliveredAmount
                            , loanProcessAmountCount.MonthlyUndeliveredAmount],
                        backgroundColor: "#FF4961",
                        hoverBackgroundColor: "rgba(255,73,97,.9)",
                        borderColor: "transparent"
                    }]
                };

                var loanProcessAmountChartconfig = {
                    type: 'bar',

                    // Chart Options
                    options: loanProcessAmountChartOptions,

                    data: loanProcessAmountChartData
                };

                // Create the chart
                new Chart(loanProcessAmountCtx, loanProcessAmountChartconfig);

            }
        });




   // })


})