
    'use strict';
    angular
        .module('mvc.services')
        .factory('userService',)

    userService.$inject = ['$http'];

    function userService($http) {
        this.getData = getData;

        function getData() { }
    }
