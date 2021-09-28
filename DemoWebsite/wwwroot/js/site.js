// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getUsers() {
    var query = "";
    var memberIdInput = document.getElementById("memberIdInput");
    if (memberIdInput && memberIdInput.value !== "") {
        query += "?memberId=" + memberIdInput.value;
    } else {

        var firstNameInput = document.getElementById("memberFirstNameInput");
        if (firstNameInput && firstNameInput.value !== "") {
            query += "?firstName=" + firstNameInput.value;

            if (query !== "") {
                var lastNameInput = document.getElementById("memberLastNameInput");
                if (lastNameInput && lastNameInput.value !== "") {
                    query += "&lastName=" + lastNameInput.value;
                }
            }
        }
    }

    if (query === "") {
        alert("Enter a search option!");
    } else {

        var chk = document.getElementById("useAlternateId");
        if (chk.checked) {
            query += "&useAlternateId=true";
        }

        var x = document.getElementById("testDataMsg");
        x.innerHTML = "Querying backend api...";

        $.ajax({
            url: "/api/RewardCustomer" + query, type: "GET", success: function (result) {
                if (result.length) {

                    var data = "";
                    for (var i = 0; i < result.length; i++) {
                        var item = result[i];

                        var checkItemButton = '<button type="button" class="btn btn-primary btn-sm" onclick="checkEligibleRewards(' + "'" + item.memberId.trim() + "'" + ',' + item.points + ')">Check eligible reward</button>';
                        var checkOrdersButton = '<button type="button" class="btn btn-primary btn-sm" onclick="checkOrders(' + "'" + item.memberId.trim() + "'" + ')">Check reward orders</button>';

                        var row = '<tr><th scope="row">' + item.memberId + '</th><td>' + item.firstName + '</td><td>' + item.lastName + '</td><td>' + item.points + '</td><td>' + checkItemButton + "&nbsp;" + checkOrdersButton + '</td></tr>'
                        data += row;
                    }

                    document.getElementById("testData").innerHTML = data;
                    document.getElementById("eligibleRewards").innerHTML = "";

                    x.innerHTML = "Data is acquired";
                } else {
                    x.innerHTML = JSON.stringify(result);
                }

            }, error: function (err) {
                x.innerHTML = JSON.stringify(err);
            }
        });
    }
}

function checkEligibleRewards(memberId, points) {

    var x = document.getElementById("testDataMsg");
    x.innerHTML = "Querying backend api...";

    $.ajax({
        url: "/api/RewardItem?points=" + points, type: "GET", success: function (result) {
            if (result.length || result.length === 0) {

                var data = "";
                for (var i = 0; i < result.length; i++) {
                    var item = result[i];

                    var supRole = document.getElementById("roleSupervisor")
                    var redeemButton = supRole ? '<button type="button" class="btn btn-primary btn-sm" onclick="redeemRewards(' + "'" + memberId + "'" + ",'" + item.id + "'" + ')">Redeem</button>' : "";

                    var row = '<tr><th scope="row">' + item.name + '</th><td>' + item.points + '</td><td>' + redeemButton + '</td></tr>'
                    data += row;
                }

                document.getElementById("eligibleRewards").innerHTML = data;

                x.innerHTML = "Data is acquired";
            } else {

                x.innerHTML = JSON.stringify(result);
            }

        }, error: function (err) {
            x.innerHTML = JSON.stringify(err);
        }
    });
}

function redeemRewards(memberId, rewardItemId) {

    var x = document.getElementById("testDataMsg");
    x.innerHTML = "Redeem rewards in progress...";

    var body = JSON.stringify({ memberId: memberId, rewardItemId: rewardItemId });

    $.ajax({
        data: body,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/api/Reward", type: "POST", success: function (result) {
            if (result.success) {
                document.getElementById("eligibleRewards").innerHTML = "";
                alert(result.message);
                getUsers();
            } else {
                x.innerHTML = result.message;
            }
        }, error: function (err) {
            x.innerHTML = JSON.stringify(err);
        }
    });
}

function checkOrders(memberId) {
    var x = document.getElementById("testDataMsg");
    x.innerHTML = "Checking member orders..";

    $.ajax({
        url: "/api/Order?memberId=" + memberId, type: "GET", success: function (result) {
            if (result.length || result.length === 0) {

                var data = "";
                for (var i = 0; i < result.length; i++) {
                    var item = result[i];

                    var shipped = item.shipped ? (new Date(item.shipped)).toLocaleString() : "Not shipped";

                    var trackingNumber = item.trackingNumber ?? "";

                    var row = '<tr><th scope="row">' + item.id + '</th><td>' + item.productName + '</td><td>' + shipped + '</td><td>' + trackingNumber + '</td></tr>'
                    data += row;
                }

                document.getElementById("orders").innerHTML = data;

                x.innerHTML = "Data is acquired";
            } else {

                x.innerHTML = JSON.stringify(result);
            }

        }, error: function (err) {
            x.innerHTML = JSON.stringify(err);
        }
    });
}