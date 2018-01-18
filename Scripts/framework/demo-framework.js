angular.module("demo", ["ngRoute", "dndLists", "btorfs.multiselect" , "isteven-multi-select"])
    .config(function ($routeProvider, $sceProvider) {
        $sceProvider.enabled(false);
        var Vurl = "/Plugins/Widgets.MegaMenu/advanced/advanced-frame.html";
        var url = window.location.origin + Vurl;

        $routeProvider
            .when('/simple', {
                templateUrl: 'simple/simple-frame.html',
                controller: 'SimpleDemoController'
            })
            .when('/nested', {
                templateUrl: 'nested/nested-frame.html',
                controller: 'NestedListsDemoController'
            })
            .when('/types', {
                template: "<span><a href='#/advanced'>back to advanced...</a></span>"
            })
            .when('/advanced', {
                templateUrl: url,
                controller: 'AdvancedDemoController'
            })
            .when('/multi', {
                templateUrl: 'multi/multi-frame.html',
                controller: 'MultiDemoController'
            })
            .otherwise({ redirectTo: '/advanced' });
    })

 .directive('navigation', function ($rootScope, $location) {
     return {
         template: '<li ng-repeat="option in options" ng-class="{active: isActive(option)}">' +
                   '    <a ng-href="{{option.href}}">{{option.label}}</a>' +
                   '</li>',
         link: function (scope, element, attr) {
             scope.options = [
                 { label: "Nested Containers", href: "#/nested" },
                 { label: "Simple Demo", href: "#/simple" },
                 { label: "Item Types", href: "#/types" },
                 { label: "Advanced Demo", href: "#/advanced" },
                 { label: "Multiselection", href: "#/multi" },
                 { label: "Github", href: "https://github.com/marceljuenemann/angular-drag-and-drop-lists" }
             ];

             scope.isActive = function (option) {
                 return option.href.indexOf(scope.location) === 1;
             };

             $rootScope.$on("$locationChangeSuccess", function (event, next, current) {
                 scope.location = $location.path();
             });
         }
     };
 });

