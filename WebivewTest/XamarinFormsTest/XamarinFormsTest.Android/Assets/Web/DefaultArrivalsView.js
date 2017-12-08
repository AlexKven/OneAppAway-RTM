var ShownArrivals = new Array();

function addArrival(trip, stop, route, prevRoute, routeName, prevRouteName, scheduledArrivalTime, predictedArrivalTime, vehicle, destination, frequencyMinutes, scheduledVehicleLocation, knownVehicleLocation, orientation, confidence, dropOffOnly)
{
    var div;
    var arrivalIndex = findArrivalIndex(trip, route);
    if (arrivalIndex == -1)
        div = document.createElement("div");
    else
        div = ShownArrivals[arrivalIndex].element;
    ShownArrivals.push({ route: route, trip: trip, element: div });
    div.textContent = routeName + predictedArrivalTime;
    $("body").append(div);
}

function findArrivalIndex(trip, route)
{
    for (i = 0; i < ShownArrivals.length; i++)
    {
        if (ShownArrivals[i].route == route && ShownArrivals[i].trip == trip)
            return i;
    }
    return -1;
}