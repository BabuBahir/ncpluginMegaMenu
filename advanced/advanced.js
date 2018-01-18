angular.module("demo").controller("AdvancedDemoController", function ($scope) {

    $scope.dragoverCallback = function (index, external, type, callback) {
        $scope.logListEvent('dragged over', index, external, type);
        // Invoke callback to origin for container types.
        if (type == 'container' && !external) {
            console.log('Container being dragged contains ' + callback() + ' items');
        }
        return index < 10; // Disallow dropping in the third row.
    };

    $scope.dropCallback = function (index, item, external, type) {
        $scope.logListEvent('dropped at', index, external, type);
        // Return false here to cancel drop. Return true if you insert the item yourself.
        return item;
    };

    $scope.logEvent = function (message) {
        //console.log(message);
    };

    $scope.logListEvent = function (action, index, external, type) {
        var message = external ? 'External ' : '';
        message += type + ' element was ' + action + ' position ' + index;
        //console.log(message);
    };

    // Initialize model
    $scope.model = [[]];
    var id = 10;
    var topMenuCategories = $scope.topMenuCategoriesJson;

    angular.forEach(['move'], function (effect, i) {
        var container = { items: [], effectAllowed: effect };
        for (var k = 0; k < topMenuCategories.length; ++k) {
            container.items.push({ label: topMenuCategories[k].label, Id: topMenuCategories[k].Id });
        }
        $scope.model[i % $scope.model.length].push(container);
    });

    $scope.$watch('model', function (model) {
        var inneritems = $scope.model[0][0].items;
        $scope.modelAsJson = angular.toJson(inneritems, true);
    }, true);


    $scope.$watch('modelAsJson', function (newValue, oldValue) {
        //console.log($scope.modelAsJson);
        $("#DisplayorderJson").val($scope.modelAsJson);
    });

    $scope.$watch("topMenuCategoriesJson", function (topMenuCategoriesJson) {
        // console.log(topMenuCategoriesJson )
    });

    $scope.$watch("outputallCategories", function (outputallCategories) {

        // [] , ["1","3"]
        var CategoryArray = (outputallCategories.map(a => a.id));
        console.log(CategoryArray);
        if (CategoryArray.length) {
            $("#outputAllCategories").val(CategoryArray);
        } else {
            $("#outputAllCategories").val('');
        }
    });
});
