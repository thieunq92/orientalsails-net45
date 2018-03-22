var moduleAddSeriesBookings = angular.module("moduleAddSeriesBookings", [])
    .directive('convertToNumber', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return parseInt(val, 10);
                });
                ngModel.$formatters.push(function (val) {
                    return '' + val;
                });
            }
        };
    });

var moduleViewActivities = angular.module("moduleViewActivities", []);
var moduleDocumentView = angular.module("moduleDocumentView", []);

angular.module("myApp",
    ["moduleAddSeriesBookings",
    "moduleViewActivities",
    "moduleDocumentView"]);