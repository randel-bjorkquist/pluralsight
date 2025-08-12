import requests

response = requests.get('http://api.open-notify.org/astros.json')
json     = response.json()

print(json)

print('The people currently in space are:')

for p in json['people']:
  print(p['name'])

  