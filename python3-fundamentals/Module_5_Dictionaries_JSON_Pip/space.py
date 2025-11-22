# ---------------------------------------------------------------------------------------------------------------------
#json = { 'number': 4
#        ,'students': [ {'name': 'Sarah Holderness' ,'email': 'sarah@example.com'}
#                      ,{'name': 'Harry Potter'     ,'email': 'harry.potter@example.com'}
#                      ,{'name': 'Hermione Granger' ,'email': 'hermione.granger@example.com'}
#                      ,{'name': 'Ronald Weasley'   ,'email': 'ronald.weasley@example.com'} ]}
#
# ---------------------------------------------------------------------------------------------------------------------
#request = requests.get('https://api.github.com/user')



# ---------------------------------------------------------------------------------------------------------------------
# api: http://api.open-notify.org/astros.json


# ---------------------------------------------------------------------------------------------------------------------
import requests

# ---------------------------------------------------------
print()
print("response = requests.get('http://api.open-notify.org/astros.json')")

response = requests.get('http://api.open-notify.org/astros.json')

# ---------------------------------------------------------
print()

json = response.json()
print("print(json)")
print(json)

# ---------------------------------------------------------
print()

print("people = json['people']")
people = json['people']

#print(type(people))
print("print(people)")
print(people)

# ---------------------------------------------------------
print()

print("print(person)")

for person in people:
  print(person)

# ---------------------------------------------------------
print()

print("print(person['name'])")

for person in people:
  print(person['name'])

#for key, value in people.items():
#  print(f"{key} -> {value}")

# ---------------------------------------------------------------------------------------------------------------------
