var map;
var airports;
var runningSearch = false;
var markers = [];
var flightPaths = [];
var flightsData = [];
var selectedInterestType;


$(document).ready(function () {

    //$('#date-from-picker').datepicker();

    //$('#date-to-picker').datepicker();

    $("#search-submit").click(function () {
        SearchByAirport($("#from-location").val());
    });

    $("#interest-snow").click(function () {
        SelectInterestType("snow");
        initLayer("snow");
    });

    $("#interest-air-quality").click(function () {
        SelectInterestType("air-quality");
    });

    $("#interest-water-quality").click(function () {
        SelectInterestType("water-quality");
        initLayer("water");
    });

    LoadAirports();

});

function SelectInterestType(interestType) {
    selectedInterestType = interestType;
    $("#selected-interest-type").html(interestType);
};

function DeleteMarkers() {
    //Loop through all the markers and remove
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
};

function DeleteFlightPaths() {
    //Loop through all the markers and remove
    for (var i = 0; i < flightPaths.length; i++) {
        flightPaths[i].setMap(null);
    }
    flightPaths = [];
};

function SearchByAirport(code) {
    $.each(airports, function (key, airport) {

        if (airport.code == code) {


            var Latlng = { lat: parseFloat(airport.lat), lng: parseFloat(airport.lon) };
            DeleteMarkers();
            DeleteFlightPaths();
            var marker = new google.maps.Marker({
                position: Latlng,
                map: map,
                title: airport.name,
                icon: "/assets/img/map-marker-airport.png"
            });
            markers.push(marker);
            runningSearch = true;
            map.panTo(marker.getPosition());
            runningSearch = false;

            window.setTimeout(function () {
                if (runningSearch == false) {
                    runningSearch = true;

                    Search(airport.code);

                }
            }, 3000);
        }
    });

}

function LoadAirports() {

    $.getJSON('/assets/data/airports.json', function (data) {
        airports = data;

        FillFromLocation();
    });

}



function Search(airportCode) {

    ShowLoader();
    var bounds = map.getBounds();
    var ne = bounds.getNorthEast();
    var sw = bounds.getSouthWest();

    $.ajax({
        url: "api/search/" + ne + "/" + sw + "/" + airportCode + "/" + selectedInterestType,
        type: 'GET',
        dataType: "json",
        //data: airports,
        success: function (data) {
            EndSearch();
            ShowSearchResults(data);
        },
        error: function (error) {
            EndSearch();
        }
    });
}

function ShowFlight(flight, multimap) {
    var airportFrom;
    var airportTo;
    var fromMarker;
    var toMarker;

    // Remove markers
    if (multimap == false) {
        DeleteMarkers();
        DeleteFlightPaths();
    }

    // Set flight from to route
    $.each(airports, function (key, airport) {

        if (airport.code == flight.flyFrom) {
            airportFrom = airport;
            var fromLatlng = { lat: parseFloat(airportFrom.lat), lng: parseFloat(airportFrom.lon) };
            fromMarker = new google.maps.Marker({
                position: fromLatlng,
                map: map,
                title: airportFrom.name,
                icon: "/assets/img/map-marker-airport.png"
            });
            flight.fromMarker = fromMarker;
            markers.push(fromMarker);

        }

        if (airport.code == flight.flyTo) {
            airportTo = airport;
            var toLatlng = { lat: parseFloat(airportTo.lat), lng: parseFloat(airportTo.lon) };
            toMarker = new google.maps.Marker({
                position: toLatlng,
                map: map,
                title: airportTo.name,
                icon: "/assets/img/map-marker-airport-arrival.png"
            });
            flight.toMarker = toMarker;
            markers.push(toMarker);


        }
    });

    // Set route
    var flightPlanCoordinates = [
        { lat: parseFloat(airportFrom.lat), lng: parseFloat(airportFrom.lon) },
        { lat: parseFloat(airportTo.lat), lng: parseFloat(airportTo.lon) }
    ];

    var flightPath = new google.maps.Polyline({
        path: flightPlanCoordinates,
        geodesic: true,
        strokeColor: '#82afff',
        strokeOpacity: 1.0,
        strokeWeight: 4
    });
    flight.flightPath = flightPath;
    flightPath.setMap(map);
    flightPaths.push(flightPath);
}

function ShowSearchResults(data) {

    // Store flights
    flightsData = data.flights;

    // Show first flight
    //ShowFlight(data.flights[0]);

    // Remove previous result items
    $(".search-result-item").remove();

    // Load flight to search result
    $.each(flightsData, function (key, flight) {

        ShowFlight(flight, true);

        var flightElement = $("<div class='search-result-item'></div>");

        var flightFrom = $("<div class='flight-from'><strong>From: </strong> " + flight.flyFrom + " - "+ flight.cityFrom + "</div>");

        var flightTo = $("<div class='flight-to'><strong>To: </strong> " + flight.flyTo + " - "+ flight.cityTo + "</div>");

        //var duration = $("<div class='duration'><strong>Duration:</strong>"++"</div>");

        var price = $("<div class='price'><strong>Price:</strong> " + flight.price + " €</div>");

        var bookingButton = $('<a href="' + flight.deep_link + '" target="_blank" ><button type="button" class="btn btn-info" >Book now</button></a>');

        var departure = $("<div class='departure'><strong>Departure: </strong>" + flight.local_departure + "</div>");

        var arrival = $("<div class='arrival'><strong>Arrival: </strong>" + flight.local_arrival + "</div>");

        $(flightElement).append(flightFrom);
        $(flightElement).append(flightTo);
        $(flightElement).append(departure);
        $(flightElement).append(arrival);
        //$(flightElement).append(duration);
        $(flightElement).append(price);
        $(flightElement).append(bookingButton);
        $("#search-result-items").append(flightElement);

        

        flightElement.mouseenter(function () {
            flight.fromMarker.setIcon("/assets/img/map-marker-airport-hover.png");
            flight.toMarker.setIcon("/assets/img/map-marker-airport-arrival-hover.png");
            flight.flightPath.setStrokeColor('#000000');
            //flight.flightPath.setMap(map);
        });

        flightElement.mouseleave(function () {
            flight.fromMarker.setIcon("/assets/img/map-marker-airport.png");
            flight.toMarker.setIcon("/assets/img/map-marker-airport-arrival.png");
            flight.flightPath.setStrokeColor('#82afff');
            //flight.flightPath.setMap(map);
        });
    });


}

function EndSearch() {
    runningSearch = false;
    HideLoader();
}

function ShowLoader() {
    $('.loader').show();
}

function HideLoader() {
    $('.loader').hide();
}

function DisplayAirports() {

    $.getJSON('/assets/data/airports.json', function (data) {

        $.each(data, function (key, airport) {
            //console.log(val);
            var Latlng = { lat: parseFloat(airport.lat), lng: parseFloat(airport.lon) };
            var marker = new google.maps.Marker({
                position: Latlng,
                map: map,
                title: airport.name
            });
        });
    });
}

function FillFromLocation() {

    $.each(airports, function (key, airport) {
        //console.log(airport);
        $('#from-location').append($('<option>',
            {
                value: airport.code,
                text: airport.code + " - " + airport.name
            }));
    });

}

function initMap() {

    //var myLatlng = { lat: -25.363, lng: 131.044 };

    //Define OSM as base layer in addition to the default Google layers
    var osmMapType = new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {
            return "http://tile.openstreetmap.org/" +
                zoom + "/" + coord.x + "/" + coord.y + ".png";
        },
        tileSize: new google.maps.Size(256, 256),
        isPng: true,
        alt: "OpenStreetMap",
        name: "OSM",
        maxZoom: 19
    });

    //Define custom WMS tiled layer
    


    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 50.0947614, lng: 14.4402693 },
        zoom: 6,
        styles: [
            {
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#f5f5f5"
                    }
                ]
            },
            {
                "elementType": "labels.icon",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#616161"
                    }
                ]
            },
            {
                "elementType": "labels.text.stroke",
                "stylers": [
                    {
                        "color": "#f5f5f5"
                    }
                ]
            },
            {
                "featureType": "administrative.land_parcel",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#bdbdbd"
                    }
                ]
            },
            {
                "featureType": "poi",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#eeeeee"
                    }
                ]
            },
            {
                "featureType": "poi",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#757575"
                    }
                ]
            },
            {
                "featureType": "poi.park",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#e5e5e5"
                    }
                ]
            },
            {
                "featureType": "poi.park",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#9e9e9e"
                    }
                ]
            },
            {
                "featureType": "road",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#ffffff"
                    }
                ]
            },
            {
                "featureType": "road.arterial",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#757575"
                    }
                ]
            },
            {
                "featureType": "road.highway",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#dadada"
                    }
                ]
            },
            {
                "featureType": "road.highway",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#616161"
                    }
                ]
            },
            {
                "featureType": "road.local",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#9e9e9e"
                    }
                ]
            },
            {
                "featureType": "transit.line",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#e5e5e5"
                    }
                ]
            },
            {
                "featureType": "transit.station",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#eeeeee"
                    }
                ]
            },
            {
                "featureType": "water",
                "elementType": "geometry",
                "stylers": [
                    {
                        "color": "#c9c9c9"
                    }
                ]
            },
            {
                "featureType": "water",
                "elementType": "labels.text.fill",
                "stylers": [
                    {
                        "color": "#9e9e9e"
                    }
                ]
            }
        ],
        mapTypeId: 'OSM',
        mapTypeControlOptions: {
            mapTypeIds: ['OSM', google.maps.MapTypeId.ROADMAP, google.maps.MapTypeId.SATELLITE, google.maps.MapTypeId.HYBRID, google.maps.MapTypeId.TERRAIN],
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU
        }
    });

    //var marker = new google.maps.Marker({
    //    position: myLatlng,
    //    map: map,
    //    title: 'Click to zoom'
    //});

    //map.addListener('bounds_changed', function () {
    //    // 3 seconds after the center of the map has changed, pan back to the
    //    // marker.

    //    if (runningSearch == false) {
    //        runningSearch = true;
    //        window.setTimeout(function () {
    //            Search();
    //        }, 1000);
    //    }
    map.mapTypes.set('OSM', osmMapType);
    map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
    //map.data.loadGeoJson(
    //    '/assets/data/dummy-coordinates.json');
    //});

    //marker.addListener('click', function () {
    //    map.setZoom(8);
    //    map.setCenter(marker.getPosition());
    //});



    $.ajax({
        url: "api/search/satellite",
        type: 'GET',
        dataType: "json",
        success: function (data) {
            var Latlng = { lat: parseFloat(data[0]), lng: parseFloat(data[1]) };
            satelliteMarker = new google.maps.Marker({
                position: Latlng,
                map: map,
                title: "Sentinel 2B",
                icon: "/assets/img/satellite.png"
            });
        },
        error: function (error) {
        }
    });


};

function initLayer(layer)
{
    map.overlayMapTypes.clear();

    var snowLayer = new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {
            var proj = map.getProjection();
            var zfactor = Math.pow(2, zoom);
            // get Long Lat coordinates
            var top = proj.fromPointToLatLng(new google.maps.Point(coord.x * 512 / zfactor, coord.y * 512 / zfactor));
            var bot = proj.fromPointToLatLng(new google.maps.Point((coord.x + 1) * 512 / zfactor, (coord.y + 1) * 512 / zfactor));

            //create the Bounding box string
            var bbox = (top.lng()) + "," +
                (bot.lat()) + "," +
                (bot.lng()) + "," +
                (top.lat());

            //base WMS URL
            var url = "https://services.sentinel-hub.com/ogc/wms/6a68bd00-219d-4a4b-a71f-86849a656cc7";
            url += "?REQUEST=GetMap"; //WMS operation
            url += "&SERVICE=WMS";    //WMS service
            url += "&VERSION=1.1.1";  //WMS version  
            url += "&LAYERS=SNOWDATA"; //WMS layers
            url += "&FORMAT=image/png"; //WMS format
            url += "&SRS=EPSG:4326";     //set WGS84 
            url += "&BBOX=" + bbox;      // set bounding box
            url += "&WIDTH=512";         //tile size in google
            url += "&HEIGHT=512";



            return url;                 // return URL for the tile

        },
        tileSize: new google.maps.Size(512, 512)
    });

    var waterLayer = new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {
            var proj = map.getProjection();
            var zfactor = Math.pow(2, zoom);
            // get Long Lat coordinates
            var top = proj.fromPointToLatLng(new google.maps.Point(coord.x * 512 / zfactor, coord.y * 512 / zfactor));
            var bot = proj.fromPointToLatLng(new google.maps.Point((coord.x + 1) * 512 / zfactor, (coord.y + 1) * 512 / zfactor));

            //create the Bounding box string
            var bbox =     (top.lng()) + "," +
                           (bot.lat()) + "," +
                           (bot.lng()) + "," +
                           (top.lat());

            //base WMS URL
            var url = "https://services.sentinel-hub.com/ogc/wms/c46728de-87fb-4b67-b632-9a202ba5ba4d";
            url += "?REQUEST=GetMap"; //WMS operation
            url += "&SERVICE=WMS";    //WMS service
            url += "&VERSION=1.1.1";  //WMS version  
            url += "&LAYERS=WATERDATA"; //WMS layers
            url += "&FORMAT=image/png" ; //WMS format
            url += "&SRS=EPSG:4326";     //set WGS84 
            url += "&BBOX=" + bbox;      // set bounding box
            url += "&WIDTH=512";         //tile size in google
            url += "&HEIGHT=512";
            
            return url;                 // return URL for the tile

        },
        tileSize: new google.maps.Size(512, 512)
    });

    if (layer == "snow") {
        map.overlayMapTypes.push(snowLayer);
    }
    if (layer == "water")
    {
        map.overlayMapTypes.push(waterLayer);
    }
    
}