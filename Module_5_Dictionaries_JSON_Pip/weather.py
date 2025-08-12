# ----------------------------------------------------------------------------------------------------------------------
# API Key: 480cd1cb03594610a0614306251208
#
import requests

# ----------------------------------------------------------------------------------------------------------------------
city          = 'Orlando'
url           = f"https://api.weatherapi.com/v1/current.json?key=480cd1cb03594610a0614306251208&q={city}&aqi=no"
response      = requests.get(url)
weather_json  = response.json()

print('print(weather_json)')
print(weather_json)
print()

# ----------------------------------------------------------------------------------------------------------------------
location = weather_json['location']['name']
print('weather_json[''location''][''name'']')
print(location)
print()

current_weather = weather_json['current']
print('current_weather = weather_json[''current'']')
print(current_weather)
print()

print('current_weather[''temp_f'']')
print(current_weather['temp_f'])
print()


# ----------------------------------------------------------------------------------------------------------------------
#current_weather = weather_json.get('current')
#current_temp    = current_weather.get('temp_f')
current_temp    = weather_json.get('current').get('temp_f')
print(f"The current temperature is {current_temp}*F", end=' ')

#current_condition = current_weather.get('condition')
#current_condition_description = current_condition.get('text')
current_condition_description = current_weather.get('condition').get('text')
print(f"and is {current_condition_description}")

print()
print(f"The current weather in '{location}' is {current_condition_description} and {current_temp} degrees.")