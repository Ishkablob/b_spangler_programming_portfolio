"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""

from flask import Flask, render_template, request
import requests, jsonify, json
app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app

def string_filter(input):
    filterWords = ['find', 'danger']
    if len(filterWords) == 0:
        return input;
    else:
        output = "";
        splitWords = input.split();
        for word in splitWords:
            if word in filterWords:
                output = output + word + " ";
        return output;

def geolocator(street_add, city, state):
    resp = requests.get('https://api.geocod.io/v1.3/geocode?street=' + street_add + '&city=' + city + '&state=' + state
                        + '&api_key=ee1aba55ad4ea1abadbeeaa55c4254adac45e8c');
    if resp.status_code == 200:
        responseJson = json.loads(resp.text);
        lat = responseJson["results"][0]["location"]["lat"];
        long = responseJson["results"][0]["location"]["lng"];
        coords = (lat, long);
        return coords;
    return (-1, -1);

def weatherhazards(lat, long):
    resp = requests.get('https://www.metaweather.com/api/location/search/?lattlong=' + lat + ',' + long);
    if resp.status_code == 200:

        responseJson = json.loads(resp.text);
        woeid = responseJson[00]["woeid"];

        resp2 = requests.get('https://www.metaweather.com/api/location/' + str(woeid) + '/');
        if resp2.status_code == 200:
            responseJson = json.loads(resp2.text);

            hazardscore = 0;

            if responseJson["consolidated_weather"][0]["weather_state_abbr"] == "h":
                hazardscore += 2;
            elif responseJson["consolidated_weather"][0]["weather_state_abbr"] == "t":
                hazardscore += 3;
            elif responseJson["consolidated_weather"][0]["weather_state_abbr"] == "hr":
                hazardscore += 1;

            if responseJson["consolidated_weather"][0]['wind_speed'] >= 25:
                hazardscore += 1;
            if responseJson["consolidated_weather"][0]['wind_speed'] >= 38:
                hazardscore += 2;
            if responseJson["consolidated_weather"][0]['wind_speed'] >= 50:
                hazardscore += 3;
            if responseJson["consolidated_weather"][0]['wind_speed'] >= 65:
                hazardscore += 5;

            if responseJson["consolidated_weather"][0]['the_temp'] <= 0.0 or responseJson["consolidated_weather"][0]['the_temp'] >= 32.2:
                hazardscore += 1;
            if responseJson["consolidated_weather"][0]['the_temp'] <= -9.5 or responseJson["consolidated_weather"][0]['the_temp'] >= 40.5:
                hazardscore += 2;
            if responseJson["consolidated_weather"][0]['the_temp'] <= -17.78 or responseJson["consolidated_weather"][0]['the_temp'] >= 48.9:
                hazardscore += 4;
            if responseJson["consolidated_weather"][0]['the_temp'] <= -23.3 or responseJson["consolidated_weather"][0]['the_temp'] >= 54.4:
                hazardscore += 6;

            if responseJson["consolidated_weather"][0]['visibility'] <= .25:
                hazardscore += 1;
            if responseJson["consolidated_weather"][0]['visibility'] <= .125:
                hazardscore += 4;
            if responseJson["consolidated_weather"][0]['visibility'] <= .1:
                hazardscore += 10;

            return hazardscore;



@app.route('/')
def main():
    return render_template("/index.html")

@app.route('/string_filter', methods = ['GET', 'POST'])
def string_filter_tryit():
    inputform = request.form
    string_input = ''
    if request.method == 'POST':
        string_input = inputform["input_string_input"]
    filtered_string = string_filter(string_input)
    return render_template("/string_filter.html", filtered_string = filtered_string)

@app.route('/geolocate', methods = ['GET', 'POST'])
def geolocator_tryit():
    inputform = request.form
    street_add = ''
    city = ''
    state = ''
    if request.method == 'POST':
        street_add = inputform["input_street_add"]
        city = inputform["input_city"]
        state = inputform["input_state"]
    coordinates = geolocator(street_add, city, state)
    lat = coordinates[0]
    long = coordinates[1]
    return render_template("/geolocator.html", lat = lat, long = long)

@app.route('/weather_hazards', methods = ['GET', 'POST'])
def weather_hazards_tryit():
    inputform = request.form
    index = 0
    lat = ''
    lng = ''
    if request.method == 'POST':
        lat = inputform["input_lat"]
        lng = inputform["input_long"]
    index = weatherhazards(lat, lng)
    return render_template("/weather_hazards.html", index = index)

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', 'localhost')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    app.run(HOST, PORT)
