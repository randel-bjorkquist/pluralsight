# ----------------------------------------------------------------------------------------------------------------------
breakfast = ['Egg Sandwich', 'Bagel', 'Coffee']
lunch     = ['BLT', 'PB&J', 'Turkey Sandwich']
dinner    = ['Soup', 'Salad', 'Spaghetti', 'Taco']

print("Breakfast Menu:", breakfast)
print("Lunch Menu:", lunch)
print("Dinner Menu:", dinner)
print()

menus = [ ['Egg Sandwich', 'Bagel', 'Coffee']
         ,['BLT', 'PB&J', 'Turkey Sandwich']
         ,['Soup', 'Salad', 'Spaghetti', 'Taco']]

print(menus)
print("Breakfast Menu:" ,menus[0])
print("Lunch Menu:"     ,menus[1])
print("Dinner Menu:"    ,menus[2])
print()

breakfast_bagel = menus[0][1]
print("Breakfast:", breakfast_bagel)

lunch_blt = menus[1][0]
print("lunch:", lunch_blt)

dinner_taco = menus[2][3]
print("Dinner:", dinner_taco)
print()

# ----------------------------------------------------------------------------------------------------------------------
menus = { "Breakfast": ['Egg Sandwich', 'Bagel', 'Coffee']
         ,"Lunch": ['BLT', 'PB&J', 'Turkey Sandwich']
         ,"Dinner": ['Soup', 'Salad', 'Spaghetti', 'Taco']}
print(menus)
print()

print("Breakfast Menu:", menus.get('Breakfast'))
print("Lunch Menu:    ", menus.get('Lunch'))
print("Dinner Menu:   ", menus.get('Dinner'))
print()

# ----------------------------------------------------------------------------------------------------------------------
menus = { "Breakfast": ['Egg Sandwich', 'Bagel', 'Coffee']
         ,"Lunch": ['BLT', 'PB&J', 'Turkey Sandwich']
         ,"Dinner": ['Soup', 'Salad', 'Spaghetti', 'Taco']}

#for menu in menus:
#  print(f"{menu} Menu: {menus[menu]}")

for key, value in menus.items():
  print(f"{key} Menu:\t {value}")

# ----------------------------------------------------------------------------------------------------------------------
print()

# ----------------------------------------------------------------------------------------------------------------------
person = { "name": "Sarah Smith"
          ,"city": "Orlando"
          ,"state": "FL"
          ,"age": 100 }

print( person.get('name')
      ,'is'
      ,person.get('age')
      ,'years old' )

# ----------------------------------------------------------------------------------------------------------------------

# ----------------------------------------------------------------------------------------------------------------------
