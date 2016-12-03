/**
 * Data Access service JavaScript Library
 */

DataAccess = {
    username: "",
    getAllDisciplines: function (success, error) {
        $.ajax({
            url: '/Disciplines/GetAll',
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    success(error);
                }
            }
        });
    },
    getAllPeople: function (success, error) {
        $.ajax({
            url: '/People/GetAll',
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    error(result);
                }
            }
        });
    },
    getAllPermissions: function (success, error) {
        $.ajax({
            url: '/Permissions/GetAll',
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    success(error);
                }
            }
        });
    },
    getAllTimes: function (success, error) {
        $.ajax({
            url: '/Times/GetAll',
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    success(error);
                }
            }
        });
    },
    getTimesByUser: function (pk, success, error) {
        $.ajax({
            url: '/Times/GetTimesByPerson',
            async: true,
            data: pk,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    success(error);
                }
            }
        });
    },
    getAllUsers: function (success, error) {
        $.ajax({
            url: '/Users/GetAll',
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    success(error);
                }
            }
        });
    },
    getAllRelays: function (success, error) {
        $.ajax({
            url: '/Relays/GetAll',
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    error(error);
                }
            }
        });
    },
    
    insertRelay: function (data, success, error) {
        $.ajax({
            url: '/Relays/Create',
            data: data,
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    error(result);
                }
            }
        });
    },
    insertRelayDiscipline: function (data, success, error, complete) {
        $.ajax({
            url: '/RelaysDisciplines/Create',
            data: data,
            async: true,
            dataType: 'html',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    error(result);
                }
            },
            complete: function () {
                if (defined(complete)) {
                    complete();
                }
            }
        });
    },

    updateRelay: function (data, success, error) {
        $.ajax({
            url: '/Relays/Edit/' + data.Pk || 0,
            data: data,
            async: true,
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    error(result);
                }
            }
        });
    },
    updateRelayDiscipline: function (data, success, error, complete) {
        $.ajax({
            url: '/RelaysDisciplines/Edit/' + data.Pk || 0,
            data: data,
            async: true,
            dataType: 'html',
            type: 'post',
            success: function (result) {
                if (defined(success)) {
                    success(result);
                }
            },
            error: function (result) {
                if (defined(error)) {
                    error(result);
                }
            },
            complete: function () {
                if (defined(complete)) {
                    complete();
                }
            }
        });
    }
};